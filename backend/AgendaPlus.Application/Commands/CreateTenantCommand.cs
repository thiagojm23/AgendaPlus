namespace AgendaPlus.Application.Commands
{
    //public record CreateTenantCommand(
    //    string Name,
    //    string TimeZone,
    //    bool IsActive
    //);

    //public class CreateBookingCommanHandler : IRequestHandler<CreateTenantCommand, Guid>
    //{
    //    private readonly IApplicationDbContext _context;
    //    public CreateBookingCommanHandler(IApplicationDbContext context)
    //    {
    //        _context = context;
    //    }
    //    public async Task<Guid> Handle(CreateTenantCommand request, CancellationToken cancellationToken)
    //    {
    //        var tenant = new Tenant
    //        {
    //            Name = request.Name,
    //            TimeZone = request.TimeZone,
    //            IsActive = request.IsActive
    //        };

    //        _context.Tenants.Add(tenant);
    //        await _context.SaveChangesAsync(cancellationToken);
    //        return tenant.Id;
    //    }
    //}

}
