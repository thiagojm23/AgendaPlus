using AgendaPlus.Application.Commands;
using FluentValidation;

namespace AgendaPlus.Application.Validators;

public class ResetPasswordCommandValidator : AbstractValidator<ResetPasswordCommand>
{
    public ResetPasswordCommandValidator()
    {
        RuleFor(x => x.Token)
            .NotEmpty().WithMessage("O token é obrigatório");

        RuleFor(x => x.NewPassword)
            .NotEmpty().WithMessage("A nova senha é obrigatória")
            .MinimumLength(8).WithMessage("A senha deve ter no mínimo 8 caracteres")
            .Matches(@"[A-Z]").WithMessage("A senha deve conter pelo menos uma letra maiúscula")
            .Matches(@"[a-z]").WithMessage("A senha deve conter pelo menos uma letra minúscula")
            .Matches(@"[0-9]").WithMessage("A senha deve conter pelo menos um número")
            .Matches(@"[\W_]").WithMessage("A senha deve conter pelo menos um caractere especial")
            .Equal(x => x.ConfirmPassword).WithMessage("As senhas não coincidem");
    }
}