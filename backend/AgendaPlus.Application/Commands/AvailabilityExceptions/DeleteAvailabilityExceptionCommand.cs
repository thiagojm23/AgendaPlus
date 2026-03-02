using AgendaPlus.Domain.Common;
using MediatR;

namespace AgendaPlus.Application.Commands.AvailabilityExceptions;

public record DeleteAvailabilityExceptionCommand(Guid Id) : IRequest<Result>;