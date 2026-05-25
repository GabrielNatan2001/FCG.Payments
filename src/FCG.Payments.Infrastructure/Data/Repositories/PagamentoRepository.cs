using FCG.Payments.Domain.Pagamentos.Entities;
using FCG.Payments.Domain.Pagamentos.Interfaces;

namespace FCG.Payments.Infrastructure.Data.Repositories;

public class PagamentoRepository : IPagamentoRepository
{
    private readonly AppDbContext _context;

    public PagamentoRepository(AppDbContext context) => _context = context;

    public async Task Adicionar(PagamentoEntity pagamento) =>
        await _context.Pagamentos.AddAsync(pagamento);

    public async Task<int> SalvarAlteracoes() =>
        await _context.SaveChangesAsync();
}
