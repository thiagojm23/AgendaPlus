using AgendaPlus.Application.DTOs.Responses;
using AgendaPlus.Domain.Common;
using MediatR;

namespace AgendaPlus.Application.Commands;

public record LoginCommand(string Email, string Password) : IRequest<Result<AuthResponseDto>>;