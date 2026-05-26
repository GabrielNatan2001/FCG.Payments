using FCG.Payments.Application.Messaging;
using FCG.Payments.Application.Messaging.Events;
using FCG.Payments.Domain.Common;
using FCG.Payments.Domain.Pagamentos.Entities;
using FCG.Payments.Domain.Pagamentos.Interfaces;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace FCG.Payments.Application.Pagamentos.Services;

public class ProcessarPagamentoService
{
    private readonly IPagamentoRepository _repository;
    private readonly IMessageBus _messageBus;
    private readonly PaymentProcessedPublisherConfig _publisherConfig;
    private readonly ILogger<ProcessarPagamentoService> _logger;

    public ProcessarPagamentoService(
        IPagamentoRepository repository,
        IMessageBus messageBus,
        IOptions<PaymentProcessedPublisherConfig> publisherConfig,
        ILogger<ProcessarPagamentoService> logger)
    {
        _repository = repository;
        _messageBus = messageBus;
        _publisherConfig = publisherConfig.Value;
        _logger = logger;
    }

    public async Task Execute(OrderPlacedEvent order)
    {
        var status = order.Price <= 0 ? PaymentStatus.Rejected : PaymentStatus.Approved;

        var pagamento = PagamentoEntity.Criar(order.OrderId, order.UserId, order.GameId, order.Price, status);
        await _repository.Adicionar(pagamento);
        await _repository.SalvarAlteracoes();

        var processedAt = DateTime.UtcNow;
        var paymentEvent = new PaymentProcessedEvent(
            order.OrderId,
            order.UserId,
            order.GameId,
            status,
            processedAt);

        _messageBus.Publish(_publisherConfig.Exchange, _publisherConfig.CatalogRoutingKey, paymentEvent);
        _messageBus.Publish(_publisherConfig.Exchange, _publisherConfig.NotificationsRoutingKey, paymentEvent);

        _logger.LogInformation(
            "Pagamento processado | OrderId: {OrderId} | Status: {Status} | Price: {Price}",
            order.OrderId,
            status,
            order.Price);
    }
}
