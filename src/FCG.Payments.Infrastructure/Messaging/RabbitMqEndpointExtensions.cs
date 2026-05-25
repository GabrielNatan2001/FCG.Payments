using MassTransit;

namespace FCG.Payments.Infrastructure.Messaging;

internal static class RabbitMqEndpointExtensions
{
    /// <summary>
    /// Usa fila e binding já criados manualmente no RabbitMQ.
    /// Não declara exchange nem binding; apenas consome da fila informada.
    /// </summary>
    public static void UsePreExistingQueue(this IRabbitMqReceiveEndpointConfigurator endpoint)
    {
        endpoint.ConfigureConsumeTopology = false;
    }
}
