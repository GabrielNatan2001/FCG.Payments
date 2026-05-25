namespace FCG.Payments.Domain.Pagamentos.Entities;

public class PagamentoEntity
{
    public Guid Id { get; private set; } = Guid.NewGuid();
    public Guid OrderId { get; private set; }
    public Guid UserId { get; private set; }
    public Guid GameId { get; private set; }
    public decimal Price { get; private set; }
    public string Status { get; private set; } = string.Empty;
    public DateTime ProcessedAtUtc { get; private set; }

    protected PagamentoEntity() { }

    private PagamentoEntity(Guid orderId, Guid userId, Guid gameId, decimal price, string status)
    {
        OrderId = orderId;
        UserId = userId;
        GameId = gameId;
        Price = price;
        Status = status;
        ProcessedAtUtc = DateTime.UtcNow;
    }

    public static PagamentoEntity Criar(Guid orderId, Guid userId, Guid gameId, decimal price, string status) =>
        new(orderId, userId, gameId, price, status);
}
