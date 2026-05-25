using MassTransit;

namespace FCG.Payments.Application.Messaging.Events;

[EntityName("fcg.order.placed")]
public record OrderPlacedEvent(Guid OrderId, Guid UserId, Guid GameId, decimal Price, DateTime PlacedAtUtc);
