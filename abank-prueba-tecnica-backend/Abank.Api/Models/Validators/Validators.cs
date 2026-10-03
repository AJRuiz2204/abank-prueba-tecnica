using Abank.Api.Models.Dtos;
using FluentValidation;

namespace Abank.Api.Models.Validators;

public abstract class UserPayloadValidator<T> : AbstractValidator<T> where T : IUserPayload
{
    protected UserPayloadValidator()
    {
        RuleFor(x => x.Nombres).NotEmpty().MaximumLength(100);
        RuleFor(x => x.Apellidos).NotEmpty().MaximumLength(100);
        RuleFor(x => x.FechaNacimiento)
            .LessThan(DateOnly.FromDateTime(DateTime.Today))
            .WithMessage("La fecha de nacimiento debe ser anterior a hoy.");
        RuleFor(x => x.Direccion).NotEmpty().MaximumLength(250);
        RuleFor(x => x.Telefono).NotEmpty()
            .Matches(@"^\+?[0-9]{6,15}$").WithMessage("El teléfono debe tener entre 6 y 15 dígitos.");
        RuleFor(x => x.Email).NotEmpty().EmailAddress().MaximumLength(150);
    }
}

public sealed class CreateUserRequestValidator : UserPayloadValidator<CreateUserRequest>
{
    public CreateUserRequestValidator()
    {
        RuleFor(x => x.Password).NotEmpty().MaximumLength(72);
    }
}

public sealed class UpdateUserRequestValidator : UserPayloadValidator<UpdateUserRequest>
{
    public UpdateUserRequestValidator()
    {
        When(x => x.Password != null, () =>
            RuleFor(x => x.Password!).NotEmpty().MaximumLength(72));
    }
}

public sealed class LoginRequestValidator : AbstractValidator<LoginRequest>
{
    public LoginRequestValidator()
    {
        RuleFor(x => x.Telefono).NotEmpty();
        RuleFor(x => x.Password).NotEmpty();
    }
}