using AgendaPlus.Domain.Common;
using MediatR;

namespace AgendaPlus.Application.Commands.Resources;

public record DeleteResourceCommand(Guid Id) : IRequest<Result>;