using AgendaPlus.Domain.Common;
using MediatR;

namespace AgendaPlus.Application.Commands.Users;

public record UpdateUserCommand(
    Guid Id,
    string FirstName,
    string LastName,
    string? PhoneNumber,
    string? Document,
    string? BusinessName) : IRequest<Result>;