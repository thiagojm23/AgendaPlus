using AgendaPlus.Application.Commands.Tenants;
using FluentValidation;

namespace AgendaPlus.Application.Validators.Tenants;

public class DeleteTenantCommandValidator : AbstractValidator<DeleteTenantCommand>
{
    public DeleteTenantCommandValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty().WithMessage("Tenant ID is required");
    }
}