using Abank.Api.Models.Dtos;
using Abank.Api.Models.Validators;
using FluentValidation.Results;
using Xunit;

namespace Abank.Tests;

public class ValidatorsTests
{
    private static readonly DateOnly FechaValida = new(1990, 5, 10);

    private static CreateUserRequest CreateValido() => new(
        Nombres: "Edwin",
        Apellidos: "García",
        FechaNacimiento: FechaValida,
        Telefono: "70001111",
        Email: "edwin@abank.com.sv",
        Password: "Abank123*",
        Direccion: "San Salvador");

    [Fact]
    public void Login_valido_pasa()
    {
        ValidationResult result = new LoginRequestValidator().Validate(new LoginRequest("70001111", "Abank123*"));
        Assert.True(result.IsValid);
    }

    [Theory]
    [InlineData("", "clave")]
    [InlineData("70001111", "")] 
    public void Login_invalido_falla(string telefono, string password)
    {
        ValidationResult result = new LoginRequestValidator().Validate(new LoginRequest(telefono, password));
        Assert.False(result.IsValid);
    }

    [Fact]
    public void CreateUser_valido_pasa()
    {
        ValidationResult result = new CreateUserRequestValidator().Validate(CreateValido());
        Assert.True(result.IsValid);
    }

    [Fact]
    public void CreateUser_nombres_vacio_falla()
    {
        ValidationResult result = new CreateUserRequestValidator().Validate(CreateValido() with { Nombres = "" });
        Assert.False(result.IsValid);
    }

    [Fact]
    public void CreateUser_fecha_futura_falla()
    {
        DateOnly futura = DateOnly.FromDateTime(DateTime.Today).AddDays(1);
        ValidationResult result = new CreateUserRequestValidator().Validate(CreateValido() with { FechaNacimiento = futura });
        Assert.False(result.IsValid);
    }

    [Theory]
    [InlineData("abc")]
    [InlineData("12")]
    [InlineData("1234567890123456")]
    public void CreateUser_telefono_invalido_falla(string telefono)
    {
        ValidationResult result = new CreateUserRequestValidator().Validate(CreateValido() with { Telefono = telefono });
        Assert.False(result.IsValid);
    }

    [Fact]
    public void CreateUser_email_invalido_falla()
    {
        ValidationResult result = new CreateUserRequestValidator().Validate(CreateValido() with { Email = "no-es-email" });
        Assert.False(result.IsValid);
    }

    [Fact]
    public void UpdateUser_password_null_pasa()
    {
        UpdateUserRequest req = new(
            Nombres: "Edwin", Apellidos: "García", FechaNacimiento: FechaValida,
            Telefono: "70001111", Email: "edwin@abank.com.sv", Password: null, Direccion: "San Salvador");
        ValidationResult result = new UpdateUserRequestValidator().Validate(req);
        Assert.True(result.IsValid);
    }

    [Fact]
    public void UpdateUser_password_vacio_falla()
    {
        UpdateUserRequest req = new(
            Nombres: "Edwin", Apellidos: "García", FechaNacimiento: FechaValida,
            Telefono: "70001111", Email: "edwin@abank.com.sv", Password: "", Direccion: "San Salvador");
        ValidationResult result = new UpdateUserRequestValidator().Validate(req);
        Assert.False(result.IsValid);
    }
}
