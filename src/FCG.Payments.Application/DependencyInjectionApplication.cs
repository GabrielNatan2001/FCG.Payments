using FCG.Payments.Application.Messaging.Consumers;
using FCG.Payments.Application.Pagamentos.Services;
using Microsoft.Extensions.DependencyInjection;

namespace FCG.Payments.Application;

public static class DependencyInjectionApplication
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddScoped<ProcessarPagamentoService>();
        services.AddScoped<OrderPlacedConsumer>();
        return services;
    }
}
