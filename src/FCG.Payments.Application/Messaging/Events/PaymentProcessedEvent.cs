using MassTransit;

namespace FCG.Payments.Application.Messaging.Events;

[EntityName("fcg.payment.processed")]
public record PaymentProcessedEvent(Guid OrderId, Guid UserId, Guid GameId, string Status, DateTime ProcessedAtUtc);
