using FCG.Payments.Application.Messaging.Events;
using FCG.Payments.Domain.Common;
using FCG.Payments.Domain.Pagamentos.Entities;
using FCG.Payments.Domain.Pagamentos.Interfaces;
using MassTransit;
using Microsoft.Extensions.Logging;

namespace FCG.Payments.Application.Pagamentos.Services;

public class ProcessarPagamentoService
{
    private readonly IPagamentoRepository _repository;
    private readonly IPublishEndpoint _publishEndpoint;
    private readonly ILogger<ProcessarPagamentoService> _logger;

    public ProcessarPagamentoService(
        IPagamentoRepository repository,
        IPublishEndpoint publishEndpoint,
        ILogger<ProcessarPagamentoService> logger)
    {
        _repository = repository;
        _publishEndpoint = publishEndpoint;
        _logger = logger;
    }

    public async Task Execute(OrderPlacedEvent order)
    {
        var status = order.Price <= 0 ? PaymentStatus.Rejected : PaymentStatus.Approved;

        var pagamento = PagamentoEntity.Criar(order.OrderId, order.UserId, order.GameId, order.Price, status);
        await _repository.Adicionar(pagamento);
        await _repository.SalvarAlteracoes();

        var processedAt = DateTime.UtcNow;
        await _publishEndpoint.Publish(new PaymentProcessedEvent(
            order.OrderId,
            order.UserId,
            order.GameId,
            status,
            processedAt));

        _logger.LogInformation(
            "Pagamento processado | OrderId: {OrderId} | Status: {Status} | Price: {Price}",
            order.OrderId,
            status,
            order.Price);
    }
}
