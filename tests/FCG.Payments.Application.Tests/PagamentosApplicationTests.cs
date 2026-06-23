using FCG.Payments.Application.Messaging;
using FCG.Payments.Application.Messaging.Events;
using FCG.Payments.Application.Pagamentos.Services;
using FCG.Payments.Domain.Common;
using FCG.Payments.Domain.Pagamentos.Entities;
using FCG.Payments.Domain.Pagamentos.Interfaces;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;

namespace FCG.Payments.Application.Tests;

public class ProcessarPagamentoServiceTests
{
    private readonly Mock<IPagamentoRepository> _repository = new();
    private readonly Mock<IMessageBus> _messageBus = new();
    private readonly Mock<ILogger<ProcessarPagamentoService>> _logger = new();
    private readonly PaymentProcessedPublisherConfig _publisherConfig = new()
    {
        Exchange = "fcg.payment.processed",
        CatalogRoutingKey = "catalog.payment-processed",
        NotificationsRoutingKey = "notifications.payment-processed"
    };

    private ProcessarPagamentoService CreateService() =>
        new(_repository.Object, _messageBus.Object, Options.Create(_publisherConfig), _logger.Object);

    [Theory]
    [InlineData(50, PaymentStatus.Rejected)]
    [InlineData(49.99, PaymentStatus.Rejected)]
    [InlineData(51, PaymentStatus.Approved)]
    [InlineData(100, PaymentStatus.Approved)]
    public async Task Execute_DeveAplicarRegraDePreco(decimal price, string expectedStatus)
    {
        PagamentoEntity? pagamentoSalvo = null;
        _repository.Setup(r => r.Adicionar(It.IsAny<PagamentoEntity>()))
            .Callback<PagamentoEntity>(p => pagamentoSalvo = p);
        _repository.Setup(r => r.SalvarAlteracoes()).ReturnsAsync(1);

        var order = new OrderPlacedEvent(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), price, DateTime.UtcNow);

        await CreateService().Execute(order);

        Assert.NotNull(pagamentoSalvo);
        Assert.Equal(expectedStatus, pagamentoSalvo!.Status);
        Assert.Equal(price, pagamentoSalvo.Price);
    }

    [Fact]
    public async Task Execute_DevePersistirEPublicarParaCatalogENotifications()
    {
        _repository.Setup(r => r.SalvarAlteracoes()).ReturnsAsync(1);
        var order = new OrderPlacedEvent(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), 100m, DateTime.UtcNow);

        await CreateService().Execute(order);

        _repository.Verify(r => r.Adicionar(It.IsAny<PagamentoEntity>()), Times.Once);
        _repository.Verify(r => r.SalvarAlteracoes(), Times.Once);
        _messageBus.Verify(m => m.Publish(
            _publisherConfig.Exchange,
            _publisherConfig.CatalogRoutingKey,
            It.IsAny<PaymentProcessedEvent>()), Times.Once);
        _messageBus.Verify(m => m.Publish(
            _publisherConfig.Exchange,
            _publisherConfig.NotificationsRoutingKey,
            It.IsAny<PaymentProcessedEvent>()), Times.Once);
    }
}

public class OrderPlacedConsumerTests
{
    [Fact]
    public async Task Consumir_DeveProcessarPagamento()
    {
        var repository = new Mock<IPagamentoRepository>();
        repository.Setup(r => r.SalvarAlteracoes()).ReturnsAsync(1);

        var messageBus = new Mock<IMessageBus>();
        var loggerService = new Mock<ILogger<ProcessarPagamentoService>>();
        var loggerConsumer = new Mock<ILogger<FCG.Payments.Application.Messaging.Consumers.OrderPlacedConsumer>>();
        var config = Options.Create(new PaymentProcessedPublisherConfig
        {
            Exchange = "fcg.payment.processed",
            CatalogRoutingKey = "catalog.payment-processed",
            NotificationsRoutingKey = "notifications.payment-processed"
        });

        var service = new ProcessarPagamentoService(
            repository.Object, messageBus.Object, config, loggerService.Object);
        var consumer = new FCG.Payments.Application.Messaging.Consumers.OrderPlacedConsumer(
            service, loggerConsumer.Object);

        var order = new OrderPlacedEvent(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), 100m, DateTime.UtcNow);

        await consumer.Consumir(order);

        repository.Verify(r => r.Adicionar(It.IsAny<PagamentoEntity>()), Times.Once);
        messageBus.Verify(m => m.Publish(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<PaymentProcessedEvent>()), Times.Exactly(2));
    }
}
