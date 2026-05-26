using FCG.Payments.Application.Messaging;
using FCG.Payments.Domain.Pagamentos.Interfaces;
using FCG.Payments.Infrastructure.Data;
using FCG.Payments.Infrastructure.Data.Repositories;
using FCG.Payments.Infrastructure.Messaging;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace FCG.Payments.Infrastructure;

public static class DependencyInjectionInfrastructure
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<AppDbContext>(options =>
            options.UseNpgsql(configuration.GetConnectionString("DefaultConnection")));

        services.AddScoped<IPagamentoRepository, PagamentoRepository>();

        services.Configure<PaymentProcessedPublisherConfig>(
            configuration.GetSection("Publishers:PaymentProcessed"));

        services.AddSingleton<IMessageBus, MessageBus>();

        return services;
    }
}
