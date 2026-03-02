using AgendaPlus.Application.Commands.Tenants;
using FluentValidation;

namespace AgendaPlus.Application.Validators.Tenants;

public class UpdateTenantAddressCommandValidator : AbstractValidator<UpdateTenantAddressCommand>
{
    public UpdateTenantAddressCommandValidator()
    {
        RuleFor(x => x.TenantId)
            .NotEmpty().WithMessage("Tenant ID is required");

        RuleFor(x => x.Street)
            .NotEmpty().WithMessage("Street is required")
            .MaximumLength(200).WithMessage("Street must be at most 200 characters");

        RuleFor(x => x.City)
            .NotEmpty().WithMessage("City is required")
            .MaximumLength(100).WithMessage("City must be at most 100 characters");

        RuleFor(x => x.State)
            .NotEmpty().WithMessage("State is required")
            .MaximumLength(50).WithMessage("State must be at most 50 characters");

        RuleFor(x => x.ZipCode)
            .NotEmpty().WithMessage("ZIP code is required")
            .MaximumLength(20).WithMessage("ZIP code must be at most 20 characters");

        RuleFor(x => x.Country)
            .NotEmpty().WithMessage("Country is required")
            .MaximumLength(50).WithMessage("Country must be at most 50 characters");
    }
}