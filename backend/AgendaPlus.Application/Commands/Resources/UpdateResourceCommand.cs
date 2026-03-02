using AgendaPlus.Domain.Common;
using AgendaPlus.Domain.Enums;
using MediatR;

namespace AgendaPlus.Application.Commands.Resources;

public record UpdateResourceCommand(
    Guid Id,
    string Name,
    string? Description,
    ResourceType ResourceType,
    DaysOfWeekBitwise OpenDays,
    bool IsActive) : IRequest<Result>;