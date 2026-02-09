# Functional Includes Pattern - Guia de Uso

## Descrição
Os repositories agora suportam o pattern Functional Includes (Lambda Expressions), permitindo que você carregue propriedades de navegação dinamicamente quando necessário.

## Como Usar

### Exemplo 1: Buscar Token com User incluído
```csharp
// Sem includes (retorna Token sem User carregado)
var token = await _tokenRepository.GetByRefreshTokenAsync(refreshToken);

// Com includes (retorna Token com User carregado)
var token = await _tokenRepository.GetByRefreshTokenAsync(
    refreshToken, 
    t => t.User
);
```

### Exemplo 2: Buscar User com múltiplos includes
```csharp
// Com múltiplos includes
var user = await _userRepository.GetByIdAsync(
    userId,
    u => u.Tokens,
    u => u.UserTenants
);
```

### Exemplo 3: Buscar todos com includes
```csharp
// Buscar todos os tokens com User incluído
var tokens = await _tokenRepository.GetAllAsync(t => t.User);

// Buscar todos os users com Tenants incluídos
var users = await _userRepository.GetAllAsync(u => u.UserTenants);
```

## Comportamento

### Performance
- **Sem includes**: Usa Dapper para consultas mais rápidas (SQL direto)
- **Com includes**: Usa Entity Framework Core para suportar eager loading

### Interfaces Atualizadas
Todos os métodos de busca agora aceitam um parâmetro opcional `params Expression<Func<T, object>>[] includes`:

#### IBaseRepository<T>
- `Task<T?> GetByIdAsync(Guid id, params Expression<Func<T, object>>[] includes)`
- `Task<IEnumerable<T>> GetAllAsync(params Expression<Func<T, object>>[] includes)`

#### IUserRepository
- `Task<User?> GetByEmailAsync(string email, params Expression<Func<User, object>>[] includes)`

#### ITokenRepository
- `Task<Token?> GetByRefreshTokenAsync(string refreshToken, params Expression<Func<Token, object>>[] includes)`
- `Task<Token?> GetByUserIdAsync(Guid userId, params Expression<Func<Token, object>>[] includes)`

## Vantagens
✅ Carregamento sob demanda de propriedades de navegação  
✅ Performance otimizada: usa Dapper quando includes não são necessários  
✅ Flexibilidade: pode incluir múltiplas propriedades de navegação  
✅ Evita N+1 queries quando necessário  
