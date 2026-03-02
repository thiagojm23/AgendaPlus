using AgendaPlus.Domain.Common;
using MediatR;

namespace AgendaPlus.Application.Commands.Users;

public record CreateOwnerUserCommand(
    string FirstName,
    string LastName,
    string Email,
    string Password,
    string ConfirmPassword,
    string PhoneNumber,
    string? Document,
    string? BusinessName,
    string TenantName,
    string TimeZone,
    string Street,
    string City,
    string State,
    string ZipCode,
    string Country) : IRequest<Result<Guid>>;