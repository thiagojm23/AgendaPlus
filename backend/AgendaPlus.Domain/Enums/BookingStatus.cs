namespace AgendaPlus.Domain.Enums
{
    public enum BookingStatus
    {
        Pending = 1,
        WaitingResourceConfirmation, //Apenas para o futuro
        WaitingCustomerConfirmation, //Apenas para o futuro
        WaitingPayment, //Apenas para o futuro
        Paid, //Apenas para o futuro
        Confirmed,
        Cancelled,
        Completed
    }
}
