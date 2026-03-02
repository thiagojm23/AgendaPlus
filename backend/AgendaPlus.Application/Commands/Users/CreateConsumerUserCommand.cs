using AgendaPlus.Domain.Common;
using MediatR;

namespace AgendaPlus.Application.Commands.Users;

public record CreateConsumerUserCommand(
    string FirstName,
    string LastName,
    string Email,
    string Password,
    string ConfirmPassword) : IRequest<Result<Guid>>;