using AgendaPlus.Domain.Common;
using AgendaPlus.Domain.Entities;
using MediatR;

namespace AgendaPlus.Application.Queries.Users;

public record GetCurrentUserQuery : IRequest<Result<User>>;