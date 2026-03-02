using System.ComponentModel.DataAnnotations.Schema;
using AgendaPlus.Domain.Entities.Bases;
using AgendaPlus.Domain.Enums;

namespace AgendaPlus.Domain.Entities;

public class Booking : BaseEntityReferenceTenant
{
    private decimal _totalPrice;
    public Guid ResourceId { get; set; } // Changed from private set to set
    public Guid? UserId { get; set; } // Changed from private set to set (Null for guest bookings)
    public required DateTime StartBookingDateTime { get; set; }
    public required DateTime EndBookingDateTime { get; set; }
    public BookingStatus Status { get; set; } = BookingStatus.Pending;
    public required string ReservationCode { get; set; } // Unique code for lookup
    public Resource? Resource { get; set; }
    public User? User { get; set; }

    [Column(TypeName = "jsonb")] public required CustomerData CustomerData { get; set; }

    public decimal TotalPrice
    {
        get => _totalPrice;
        set => _totalPrice = Math.Round(value, 2);
    }
}

public class CustomerData
{
    public CustomerData(string name, string? email, string? phoneNumber)
    {
        if (string.IsNullOrEmpty(email) && string.IsNullOrEmpty(phoneNumber))
            throw new ArgumentException("At least one contact method must be provided (email or phone)");

        Name = name;
        Email = email;
        PhoneNumber = phoneNumber;
    }

    private CustomerData()
    {
    }

    public required string Name { get; set; }
    public string? Email { get; private set; }
    public string? PhoneNumber { get; private set; }
}