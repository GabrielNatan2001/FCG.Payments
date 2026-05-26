using FCG.Payments.Application.Messaging.Events;

namespace FCG.Payments.Application.Messaging.Consumers;

public interface IOrderPlacedMessage
{
    Task Consumir(OrderPlacedEvent dados);
}
