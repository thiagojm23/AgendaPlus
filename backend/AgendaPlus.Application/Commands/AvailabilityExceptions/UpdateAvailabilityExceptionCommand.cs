using AgendaPlus.Domain.Common;
using AgendaPlus.Domain.Enums;
using MediatR;

namespace AgendaPlus.Application.Commands.AvailabilityExceptions;

public record UpdateAvailabilityExceptionCommand(
    Guid Id,
    string? Reason,
    StrategyAvailabilityException Strategy,
    DateTime StartBlockTime,
    DateTime EndBlockTime,
    TimeOnly? OverrideStartTime,
    TimeOnly? OverrideEndTime,
    decimal? Price) : IRequest<Result>;