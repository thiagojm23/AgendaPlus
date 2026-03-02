using AgendaPlus.Domain.Common;
using MediatR;

namespace AgendaPlus.Application.Commands.Users;

public record DeleteUserCommand(Guid Id) : IRequest<Result>;