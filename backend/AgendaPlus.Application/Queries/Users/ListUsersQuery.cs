using AgendaPlus.Domain.Common;
using AgendaPlus.Domain.Entities;
using AgendaPlus.Domain.Enums;
using MediatR;

namespace AgendaPlus.Application.Queries.Users;

public record ListUsersQuery(
    int PageNumber = 1,
    int PageSize = 10,
    UserRole? Role = null,
    bool? IsActive = null) : IRequest<Result<List<User>>>;