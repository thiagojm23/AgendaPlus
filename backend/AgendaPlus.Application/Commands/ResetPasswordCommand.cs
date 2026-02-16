using AgendaPlus.Domain.Common;
using MediatR;

namespace AgendaPlus.Application.Commands;

public record ResetPasswordCommand(Guid UserId, string Token, string NewPassword, string ConfirmPassword)
    : IRequest<Result<bool>>;