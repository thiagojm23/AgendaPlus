using AgendaPlus.Application.Commands;
using FluentValidation;

namespace AgendaPlus.Application.Validators;

public class CreateUserCommandValidator : AbstractValidator<CreateUserCommand>
{
    private CreateUserCommandValidator()
    {
        RuleFor(command => command.Email)
            .NotEmpty().WithMessage("O email é obrigatório")
            .EmailAddress().WithMessage("O email informado não é válido")
            .MaximumLength(255).WithMessage("O email deve ter no máximo 255 caracteres");

        RuleFor(command => command.Password)
            .NotEmpty().WithMessage("A nova senha é obrigatória")
            .MinimumLength(8).WithMessage("A senha deve ter no mínimo 8 caracteres")
            .Matches(@"[A-Z]").WithMessage("A senha deve conter pelo menos uma letra maiúscula")
            .Matches(@"[a-z]").WithMessage("A senha deve conter pelo menos uma letra minúscula")
            .Matches(@"[0-9]").WithMessage("A senha deve conter pelo menos um número")
            .Matches(@"[\W_]").WithMessage("A senha deve conter pelo menos um caractere especial")
            .Equal(x => x.ConfirmPassword).WithMessage("As senhas não coincidem");

        RuleFor(command => command.FirstName)
            .NotEmpty().WithMessage("O nome é obrigatório")
            .MaximumLength(50).WithMessage("O nome deve ter no máximo 50 caracteres");

        RuleFor(command => command.LastName)
            .NotEmpty().WithMessage("O sobrenome é obrigatório")
            .MaximumLength(50).WithMessage("O sobrenome deve ter no máximo 50 caracteres");
    }
}