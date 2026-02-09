using System.Data;
using AgendaPlus.Application.Interfaces.Services;
using AgendaPlus.Domain.Entities;
using Dapper;
using Microsoft.AspNetCore.Http;

namespace AgendaPlus.Infrastructure.Services;

public class CurrentUserService(IHttpContextAccessor httpContextAccessor, IDbConnection dbConnection)
    : ICurrentUserService
{
    private IEnumerable<Guid>? _cachedTenantsId;
    private Guid? _cachedUserId;

    public Guid UserId
    {
        get
        {
            if (_cachedUserId.HasValue)
                return _cachedUserId.Value;

            var httpContext = httpContextAccessor.HttpContext;
            var claims = httpContext?.User?.Claims;
            if (claims == null)
                throw new InvalidOperationException("HttpContext or user claims are not available.");

            var userIdClaim = claims.FirstOrDefault(c => c.Type == CustomClaims.UserId)?.Value;
            if (string.IsNullOrWhiteSpace(userIdClaim))
                throw new InvalidOperationException("Authenticated user is missing required claim 'userId'.");

            if (!Guid.TryParse(userIdClaim, out var userIdGuid))
                throw new InvalidOperationException("Authenticated user has invalid 'userId' claim.");

            _cachedUserId = userIdGuid;
            return userIdGuid;
        }
    }

    public IEnumerable<Guid> TenantsId
    {
        get
        {
            if (_cachedTenantsId != null)
                return _cachedTenantsId;

            const string query = "SELECT tenant_id FROM user_tenants WHERE user_id = @UserId";
            _cachedTenantsId = dbConnection.Query<Guid>(query, new { UserId });

            return _cachedTenantsId;
        }
    }

    public bool IsAuthenticated => httpContextAccessor.HttpContext?.User.Identity?.IsAuthenticated ?? false;
}