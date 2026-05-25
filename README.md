# FCG.Payments.Worker

Worker que consome `OrderPlacedEvent`, simula pagamento e publica `PaymentProcessedEvent`.

## Regra de simulação

- `Price <= 0` → `Rejected`
- Caso contrário → `Approved`

## Executar

```bash
dotnet ef database update --project src/FCG.Payments.Infrastructure --startup-project src/FCG.Payments.Worker
dotnet run --project src/FCG.Payments.Worker
```
