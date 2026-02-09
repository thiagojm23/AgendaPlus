using MediatR;

namespace AgendaPlus.Application.Commands;

public record ForgotPasswordCommand(string Email) : IRequest<bool>;
