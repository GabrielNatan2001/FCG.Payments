using FCG.Payments.Application.Messaging;
using FCG.Payments.Application.Messaging.Consumers;
using FCG.Payments.Application.Messaging.Events;
using Microsoft.Extensions.Options;

namespace FCG.Payments.Worker;

public class OrderPlacedWorker : BackgroundService
{
    public readonly IServiceProvider _serviceProvider;
    private readonly ILogger<OrderPlacedWorker> _logger;
    private readonly OrderPlacedWorkerConfig _config;
    private const string NomeWorker = "WORKER-PAYMENTS-ORDER-PLACED";

    public OrderPlacedWorker(
        IServiceProvider serviceProvider,
        ILogger<OrderPlacedWorker> logger,
        IOptions<OrderPlacedWorkerConfig> config)
    {
        _logger = logger;
        _serviceProvider = serviceProvider;
        _config = config.Value;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        try
        {
            if (!_config.Ativo)
            {
                _logger.LogInformation("[WORKER][{Nome}] - Esta desativada.", NomeWorker);
                return;
            }

            _logger.LogInformation("[WORKER][{Nome}] Iniciado.", NomeWorker);

            using var scope = _serviceProvider.CreateScope();
            var _messageBus = scope.ServiceProvider.GetRequiredService<IMessageBus>();
            await _messageBus.Subscribe<OrderPlacedEvent>(
                _config.Exchange,
                _config.RoutingKey,
                ProcessaMensagem,
                stoppingToken);

            await Task.Delay(-1, stoppingToken);
        }
        catch (Exception ex)
        {
            _logger.LogError("[WORKER][{Nome}][EXCEPTION]: {Exception}", NomeWorker, ex.ToString());
        }
    }

    private async Task ProcessaMensagem(OrderPlacedEvent dados)
    {
        try
        {
            using var scope = _serviceProvider.CreateScope();
            var message = scope.ServiceProvider.GetRequiredService<IOrderPlacedMessage>();
            await message.Consumir(dados);
        }
        catch (Exception ex)
        {
            _logger.LogError("[WORKER][{Nome}][EXCEPTION]: {Exception}", NomeWorker, ex.ToString());
        }
    }
}
