using FCG.Payments.Application.Messaging.Events;
using FCG.Payments.Application.Pagamentos.Services;
using MassTransit;
using Microsoft.Extensions.Logging;

namespace FCG.Payments.Application.Messaging.Consumers;

public class OrderPlacedConsumer : IConsumer<OrderPlacedEvent>
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

    public async Task Consume(ConsumeContext<OrderPlacedEvent> context)
    {
        _logger.LogInformation(
            "OrderPlacedEvent recebido | OrderId: {OrderId} | UserId: {UserId} | GameId: {GameId}",
            context.Message.OrderId,
            context.Message.UserId,
            context.Message.GameId);

        await _service.Execute(context.Message);
    }
}
