using FCG.Payments.Application.Messaging.Events;
using FCG.Payments.Application.Pagamentos.Services;
using MassTransit;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;

namespace FCG.Payments.Infrastructure.Messaging;

internal static class RabbitMqBusConfiguration
{
    public static void ConfigureHost(IRabbitMqBusFactoryConfigurator cfg, IConfiguration configuration)
    {
        var rabbit = configuration.GetSection("RabbitMq");
        cfg.Host(
            rabbit["Host"] ?? "localhost",
            ushort.Parse(rabbit["Port"] ?? "5672"),
            rabbit["VirtualHost"] ?? "/",
            h =>
            {
                h.Username(rabbit["Username"] ?? "guest");
                h.Password(rabbit["Password"] ?? "guest");
            });

        cfg.DeployPublishTopology = false;
    }

    public static void ConfigureConsumerAndPublish(
        IRabbitMqBusFactoryConfigurator cfg,
        IBusRegistrationContext context,
        IConfiguration configuration)
    {
        var queues = configuration.GetSection("RabbitMq:Queues");
        var orderPlacedQueue = queues["PaymentsOrderPlaced"] ?? "payments.order-placed";

        cfg.Publish<PaymentProcessedEvent>(p => p.ExchangeType = ExchangeType.Fanout);

        cfg.ReceiveEndpoint(orderPlacedQueue, e =>
        {
            e.UsePreExistingQueue();

            e.Handler<OrderPlacedEvent>(async consumeContext =>
            {
                var service = consumeContext.GetServiceOrCreateInstance<ProcessarPagamentoService>();
                var logger = consumeContext.GetServiceOrCreateInstance<ILogger<ProcessarPagamentoService>>();

                logger.LogInformation(
                    "Mensagem recebida na fila {Queue} | OrderId: {OrderId}",
                    orderPlacedQueue,
                    consumeContext.Message.OrderId);

                await service.Execute(consumeContext.Message);
            });
        });
    }
}
