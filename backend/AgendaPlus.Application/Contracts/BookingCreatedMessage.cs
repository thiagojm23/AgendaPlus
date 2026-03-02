namespace AgendaPlus.Application.Contracts;

public record BookingCreatedMessage
{
    public required Guid BookingId { get; init; }
    public required string CustomerEmail { get; init; }
    public required string CustomerName { get; init; }
    public string? CustomerPhone { get; init; }
    public required Guid ResourceId { get; init; }
    public required string ReservationCode { get; init; }
    public required DateTime StartDateTime { get; init; }
    public required DateTime EndDateTime { get; init; }
    public required decimal TotalPrice { get; init; }
    public bool IsGuest { get; init; }
}