using AgendaPlus.Domain.Common;
using MediatR;

namespace AgendaPlus.Application.Commands;

public record CreateUserCommand(
    string FirstName,
    string LastName,
    string Email,
    string Password,
    string ConfirmPassword) : IRequest<Result>;