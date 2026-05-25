using FCG.Payments.Domain.Pagamentos.Entities;

namespace FCG.Payments.Domain.Pagamentos.Interfaces;

public interface IPagamentoRepository
{
    Task Adicionar(PagamentoEntity pagamento);
    Task<int> SalvarAlteracoes();
}
