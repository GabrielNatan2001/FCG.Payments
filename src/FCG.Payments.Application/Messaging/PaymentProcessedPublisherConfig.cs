namespace FCG.Payments.Application.Messaging;

public class PaymentProcessedPublisherConfig
{
    public string Exchange { get; set; } = string.Empty;
    public string CatalogRoutingKey { get; set; } = string.Empty;
    public string NotificationsRoutingKey { get; set; } = string.Empty;
}
