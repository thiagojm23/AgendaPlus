using AgendaPlus.Application.DTOs;
using AgendaPlus.Domain.Common;
using MediatR;

namespace AgendaPlus.Application.Commands;

public record RefreshTokenCommand(Guid UserId, string RefreshToken) : IRequest<Result<AuthResponseDto>>;