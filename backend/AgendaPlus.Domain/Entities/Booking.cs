using System.ComponentModel.DataAnnotations.Schema;
using AgendaPlus.Domain.Entities.Bases;
using AgendaPlus.Domain.Enums;

namespace AgendaPlus.Domain.Entities;

public class Booking : BaseEntityReferenceTenant
{
    private decimal _totalPrice;
    public Guid ResourceId { get; private set; }
    public required DateTimeOffset StartBookingDateTime { get; set; }
    public required DateTimeOffset EndBookingDateTime { get; set; }
    public BookingStatus Status { get; set; } = BookingStatus.Pending;
    public Resource? Resource { get; set; }

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
            throw new ArgumentException("Pelo menos uma forma de contato deve ser fornecida (email ou telefone)");

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