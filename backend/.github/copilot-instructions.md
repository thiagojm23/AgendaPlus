# AgendaPlus Backend - Copilot Instructions

## Build, Test, and Run Commands

### Build
```bash
dotnet build
```

### Run
```bash
dotnet run --project WebApi
```

No test projects currently exist in the solution.

## Architecture

This is a .NET 9 application following **Clean Architecture** with CQRS and MediatR patterns:

### Layer Structure

- **AgendaPlus.Domain**: Core entities, enums, and domain models (no dependencies)
- **AgendaPlus.Application**: Business logic, commands, handlers, DTOs, and validation (depends on Domain)
- **AgendaPlus.Infrastructure**: Data access, EF Core configurations, and services (depends on Application and Domain)
- **WebApi**: API controllers and program configuration (depends on all layers)

### Key Architectural Patterns

**CQRS with MediatR**
- Commands are defined in `Application/Commands` as records
- Handlers in `Application/Handlers` implement `IRequestHandler<TCommand, TResponse>`
- MediatR pipeline includes `ValidationBehavior` for FluentValidation integration

**Multi-Tenancy**
- `BaseEntityReferenceTenant` provides tenant-scoped entities via `TenantId` property
- `BaseConfiguration<T>` applies global query filters: `HasQueryFilter(x => currentUserService.TenantsId.Contains(x.TenantId))`
- `ICurrentUserService` extracts user claims from JWT and queries user's tenant associations via Dapper
- Users can belong to multiple tenants through `UserTenants` junction table

**Data Access**
- Primary ORM: Entity Framework Core with PostgreSQL and snake_case naming convention
- Secondary: Dapper for raw queries (e.g., tenant lookups in `CurrentUserService`)
- `AppDbContext` applies configurations from `Infrastructure/Configurations` using `ApplyConfigurationsFromAssembly`

**Authentication**
- JWT Bearer tokens with access/refresh token pattern
- Tokens stored in HTTP-only cookies (`accessToken`, `refreshToken`)
- Custom claims extracted in `CurrentUserService` (e.g., `CustomClaims.UserId`)

### Entity Configuration Pattern

All entity configurations inherit from `BaseConfiguration<T>` where `T : BaseEntityReferenceTenant`:
- Auto-generates UUIDs: `builder.Property(x => x.Id).HasDefaultValueSql("gen_random_uuid()")`
- Cascading tenant deletes: `HasForeignKey(x => x.TenantId).OnDelete(DeleteBehavior.Cascade)`
- Automatic tenant filtering via query filters

## Key Conventions

### Naming
- Database: snake_case (enforced by `UseSnakeCaseNamingConvention()`)
- C#: PascalCase for classes/properties, camelCase for parameters
- Project namespaces follow folder structure (e.g., `AgendaPlus.Application.Commands`)

### Entity Base Classes
- Use `BaseEntity` for tenant-independent entities (only `Id` property)
- Use `BaseEntityReferenceTenant` for tenant-scoped entities (`Id` + `TenantId`)

### Validation
- FluentValidation validators registered from `Application` assembly
- Validators automatically invoked via `ValidationBehavior<TRequest, TResponse>` in MediatR pipeline
- Validation failures throw `ValidationException`

### Dependency Injection
- `ICurrentUserService` is scoped and caches user/tenant data per request
- `IDbConnection` is registered as scoped `NpgsqlConnection` for Dapper
- Service registrations in `Program.cs` follow: Controllers → DB Context → Dapper → MediatR → Auth

### Project Reference Direction
- Domain: No external project references
- Application: References Domain only
- Infrastructure: References Application and Domain
- WebApi: References all three layers

### Note on Infrastructure Typo
The infrastructure project folder is named `AgendaPlus.Infrasctructure` (missing 't' in "Infrastructure"), but the namespace and `.csproj` use the correct spelling `AgendaPlus.Infrastructure`.

### Docker
- Using docker to run things without need it to be installed
