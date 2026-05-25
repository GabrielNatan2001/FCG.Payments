using MassTransit;
using Microsoft.Extensions.Logging;

namespace FCG.Payments.Infrastructure.Messaging;

public class ConsumeFaultObserver : IConsumeObserver
{
    private readonly ILogger<ConsumeFaultObserver> _logger;

    public ConsumeFaultObserver(ILogger<ConsumeFaultObserver> logger) => _logger = logger;

    public Task PreConsume<T>(ConsumeContext<T> context) where T : class
    {
        _logger.LogDebug("PreConsume {MessageType}", typeof(T).Name);
        return Task.CompletedTask;
    }

    public Task PostConsume<T>(ConsumeContext<T> context) where T : class => Task.CompletedTask;

    public Task ConsumeFault<T>(ConsumeContext<T> context, Exception exception) where T : class
    {
        _logger.LogError(
            exception,
            "Falha ao consumir {MessageType} | MessageId: {MessageId}",
            typeof(T).Name,
            context.MessageId);

        return Task.CompletedTask;
    }
}
