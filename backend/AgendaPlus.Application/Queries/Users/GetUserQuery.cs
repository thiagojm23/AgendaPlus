using AgendaPlus.Domain.Common;
using AgendaPlus.Domain.Entities;
using MediatR;

namespace AgendaPlus.Application.Queries.Users;

public record GetUserQuery(Guid Id) : IRequest<Result<User>>;