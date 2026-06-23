using FCG.Payments.Domain.Common;
using FCG.Payments.Domain.Pagamentos.Entities;

namespace FCG.Payments.Domain.Tests;

public class PagamentoEntityTests
{
    [Fact]
    public void Criar_ComDadosValidos_DevePreencherPropriedades()
    {
        var orderId = Guid.NewGuid();
        var userId = Guid.NewGuid();
        var gameId = Guid.NewGuid();

        var pagamento = PagamentoEntity.Criar(orderId, userId, gameId, 59.99m, PaymentStatus.Approved);

        Assert.NotEqual(Guid.Empty, pagamento.Id);
        Assert.Equal(orderId, pagamento.OrderId);
        Assert.Equal(userId, pagamento.UserId);
        Assert.Equal(gameId, pagamento.GameId);
        Assert.Equal(59.99m, pagamento.Price);
        Assert.Equal(PaymentStatus.Approved, pagamento.Status);
        Assert.True(Math.Abs((pagamento.ProcessedAtUtc - DateTime.UtcNow).TotalSeconds) <= 5);
    }
}
