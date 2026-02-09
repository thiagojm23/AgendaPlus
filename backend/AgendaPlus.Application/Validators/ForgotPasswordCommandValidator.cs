using AgendaPlus.Application.Commands;
using FluentValidation;

namespace AgendaPlus.Application.Validators;

public class ForgotPasswordCommandValidator : AbstractValidator<ForgotPasswordCommand>
{
    public ForgotPasswordCommandValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("O email é obrigatório")
            .EmailAddress().WithMessage("O email informado não é válido")
            .MaximumLength(255).WithMessage("O email deve ter no máximo 255 caracteres");
    }
}
