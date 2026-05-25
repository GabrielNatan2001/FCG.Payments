using FCG.Payments.Domain.Pagamentos.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FCG.Payments.Infrastructure.Data.Mappings;

public class PagamentoMapping : IEntityTypeConfiguration<PagamentoEntity>
{
    public void Configure(EntityTypeBuilder<PagamentoEntity> builder)
    {
        builder.ToTable("pagamentos");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).HasColumnName("id");
        builder.Property(x => x.OrderId).HasColumnName("order_id").IsRequired();
        builder.Property(x => x.UserId).HasColumnName("user_id").IsRequired();
        builder.Property(x => x.GameId).HasColumnName("game_id").IsRequired();
        builder.Property(x => x.Price).HasColumnName("price").HasColumnType("numeric(10,2)").IsRequired();
        builder.Property(x => x.Status).HasColumnName("status").HasMaxLength(20).IsRequired();
        builder.Property(x => x.ProcessedAtUtc).HasColumnName("processed_at").IsRequired();
        builder.HasIndex(x => x.OrderId).IsUnique();
    }
}
