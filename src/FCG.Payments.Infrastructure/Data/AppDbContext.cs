using FCG.Payments.Domain.Pagamentos.Entities;
using Microsoft.EntityFrameworkCore;

namespace FCG.Payments.Infrastructure.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<PagamentoEntity> Pagamentos => Set<PagamentoEntity>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
        base.OnModelCreating(modelBuilder);
    }
}
