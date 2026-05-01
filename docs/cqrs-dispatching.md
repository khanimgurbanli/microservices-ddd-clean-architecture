# CQRS Dispatching Abstraction

Bu repository-də CQRS (MediatR) istifadə oluna bilər və ya tamamilə söndürülə bilər. Bu keçid üçün API-lar birbaşa MediatR-a deyil, `IRequestDispatcher` abstraksiyasına bağlanır.

## Məqsəd

- Controller və digər “adapter” qatları (HTTP, consumer-lər) CQRS-in konkret implementasiyasına birbaşa bağlı qalmasın.
- CQRS çıxarılanda (və ya söndürüləndə) sistem compile/runtime xətası vermədən işləməyə davam etsin.
- Domain və repository/service interfeysləri yalnız domain modelləri ilə işləsin.

## Abstraksiya

- `Shared.Abstractions.Dispatching.IRequestDispatcher`
  - `Send<TResponse>(object request, CancellationToken ct = default)`

## Implementasiyalar

### CQRS aktiv (UseCqrs=true)

- `Order.API.Infrastructure.Dispatching.MediatRRequestDispatcher`
- `Stock.API.Infrastructure.Dispatching.MediatRRequestDispatcher`

Bu variantda `IMediator` vasitəsilə request-lər mövcud handler-lərə yönləndirilir.

### CQRS söndürülmüş (UseCqrs=false)

- `Order.API.Infrastructure.Dispatching.DirectRequestDispatcher`
- `Stock.API.Infrastructure.Dispatching.DirectRequestDispatcher`

Bu variantda request-lər `switch`/pattern-matching ilə birbaşa domain-usecase servis metodlarına yönləndirilir və response mapping adapter səviyyəsində tamamlanır.

## Konfiqurasiya

Hər iki API üçün `UseCqrs` konfiqurasiya açarı istifadə olunur.

- Aktiv etmək: `UseCqrs=true`
- Söndürmək: `UseCqrs=false`

Environment variable kimi də verilə bilər:

- `UseCqrs=true|false`

## CQRS-i tam çıxarmaq

CQRS-in söndürülməsi üçün runtime tərəfdə yalnız `UseCqrs=false` kifayətdir.

CQRS paketlərini (MediatR) codebase-dən tam silmək qərarı verildikdə:

- Controller/adapter qatında dəyişiklik tələb olunmur (yenə `IRequestDispatcher` istifadə olunur).
- CQRS handler-lərini (Application/Commands, Application/Queries) silmək mümkündür, amma bu halda `DirectRequestDispatcher`-in routing-i saxlanmalıdır (o, sistemin CQRS-siz işləməsini təmin edir).

## Layer qaydaları (xülasə)

- Domain: aggregate root/entity/value object + business rules
- Application: use-case contract-ları (interfaces), request/response modelləri, validation, transformasiya/mapping
- Infrastructure: repository implementasiyaları, messaging/consumers, DB, CQRS dispatching implementasiyası

