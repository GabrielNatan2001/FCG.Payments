using FCG.Payments.Application.Messaging.Events;
using FCG.Payments.Application.Pagamentos.Services;
using Microsoft.Extensions.Logging;

namespace FCG.Payments.Application.Messaging.Consumers;

public class OrderPlacedConsumer : IOrderPlacedMessage
{
    private readonly ProcessarPagamentoService _service;
    private readonly ILogger<OrderPlacedConsumer> _logger;

    public OrderPlacedConsumer(
        ProcessarPagamentoService service,
        ILogger<OrderPlacedConsumer> logger)
    {
        _service = service;
        _logger = logger;
    }

    public async Task Consumir(OrderPlacedEvent dados)
    {
        _logger.LogInformation(
            "OrderPlacedEvent recebido | OrderId: {OrderId} | UserId: {UserId} | GameId: {GameId}",
            dados.OrderId,
            dados.UserId,
            dados.GameId);

        await _service.Execute(dados);
    }
}
