# FCG.Payments

Worker responsável por **processar pagamentos** de forma assíncrona. Consome `OrderPlacedEvent` (disparado pelo Catalog), simula a aprovação/rejeição do pagamento e publica `PaymentProcessedEvent` para o Catalog e Notifications.

## Projetos

| Projeto | Descrição |
|---|---|
| `FCG.Payments.Worker` | Consumer e publisher de eventos de pagamento |
| `FCG.Payments.Application` | Casos de uso e consumidores |
| `FCG.Payments.Infrastructure` | EF Core (PostgreSQL) e RabbitMQ |
| `FCG.Payments.Domain` | Entidades de pagamento |

## Imagem Docker

`gabrielnatan2001/fcg-worker-payments:latest`

## Regra de simulação

| Condição | Resultado |
|---|---|
| `Price <= 0` | `Rejected` |
| Demais casos | `Approved` |

## Variáveis de ambiente

| Variável (Docker/K8s) | appsettings | Obrigatória | Descrição | Exemplo |
|---|---|---|---|---|
| `ASPNETCORE_ENVIRONMENT` | — | Sim | Ambiente de execução | `Production` |
| `ConnectionStrings__DefaultConnection` | `ConnectionStrings:DefaultConnection` | Sim | PostgreSQL (`fcg_payments`) | `Host=postgres;Port=5432;Database=fcg_payments;Username=postgres;Password=postgres` |
| `MessageBusConfigs__Host` | `MessageBusConfigs:Host` | Sim | URI do RabbitMQ | `amqp://admin:admin@rabbitmq:5672/` |
| `MessageBusConfigs__RetryCount` | `MessageBusConfigs:RetryCount` | Não | Tentativas de reconexão | `5` |
| `Workers__OrderPlaced__Ativo` | `Workers:OrderPlaced:Ativo` | Sim | Habilita o consumer de pedidos | `true` |
| `Workers__OrderPlaced__Exchange` | `Workers:OrderPlaced:Exchange` | Sim | Exchange do pedido | `fcg.order.placed` |
| `Workers__OrderPlaced__RoutingKey` | `Workers:OrderPlaced:RoutingKey` | Sim | Routing key do pedido | `payments.order-placed` |
| `Publishers__PaymentProcessed__Exchange` | `Publishers:PaymentProcessed:Exchange` | Sim | Exchange do pagamento processado | `fcg.payment.processed` |
| `Publishers__PaymentProcessed__CatalogRoutingKey` | `Publishers:PaymentProcessed:CatalogRoutingKey` | Sim | Routing key para o Catalog | `catalog.payment-processed` |
| `Publishers__PaymentProcessed__NotificationsRoutingKey` | `Publishers:PaymentProcessed:NotificationsRoutingKey` | Sim | Routing key para Notifications | `notifications.payment-processed` |

## Executar localmente

```bash
dotnet ef database update --project src/FCG.Payments.Infrastructure --startup-project src/FCG.Payments.Worker
dotnet run --project src/FCG.Payments.Worker
```

Para subir a stack completa com Docker, use o [FCG.Infra](../FCG.Infra/README.md).

## Deploy

Manifests Kubernetes em `k8s/`. Instruções completas no [README do FCG.Infra](../FCG.Infra/README.md).
