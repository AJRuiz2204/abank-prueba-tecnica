using Abank.Api.Models;
using Abank.Api.Models.Dtos;
using Abank.Api.Services;
using Xunit;

namespace Abank.Tests;

public class UsuarioMappingsTests
{
    private static Usuario NuevoUsuario() => new()
    {
        Id = 7,
        Nombres = "Edwin",
        Apellidos = "García",
        FechaNacimiento = new DateTime(1990, 5, 10),
        Direccion = "San Salvador",
        Password = "$2a$11$hash",
        Telefono = "70001111",
        Email = "edwin@abank.com.sv",
        Estado = "A"
    };

    [Fact]
    public void ToResponse_mapea_campos_y_no_expone_password()
    {
        UserResponse dto = NuevoUsuario().ToResponse();

        Assert.Equal(7, dto.Id);
        Assert.Equal("Edwin", dto.Nombres);
        Assert.Equal(new DateOnly(1990, 5, 10), dto.FechaNacimiento);
        Assert.Equal("edwin@abank.com.sv", dto.Email);
        Assert.DoesNotContain("Password", typeof(UserResponse).GetProperties().Select(p => p.Name));
    }

    [Fact]
    public void ToSessionDto_marca_sesion_activa()
    {
        UserSessionDto dto = NuevoUsuario().ToSessionDto();

        Assert.True(dto.SessionActive);
        Assert.Equal("70001111", dto.Telefono);
        Assert.Equal(new DateOnly(1990, 5, 10), dto.FechaNacimiento);
    }

    [Fact]
    public void ToEntity_create_asigna_hash_y_normaliza_email()
    {
        CreateUserRequest req = new(
            Nombres: "  Edwin  ", Apellidos: "García", FechaNacimiento: new DateOnly(1990, 5, 10),
            Telefono: "70001111", Email: "  Edwin@ABANK.com.SV ", Password: "Abank123*", Direccion: "San Salvador");

        Usuario entity = req.ToEntity("HASH");

        Assert.Equal("HASH", entity.Password);
        Assert.Equal("Edwin", entity.Nombres);
        Assert.Equal("edwin@abank.com.sv", entity.Email);
        Assert.Equal(new DateTime(1990, 5, 10), entity.FechaNacimiento);
    }

    [Fact]
    public void ToEntity_update_asigna_id()
    {
        UpdateUserRequest req = new(
            Nombres: "Edwin", Apellidos: "García", FechaNacimiento: new DateOnly(1990, 5, 10),
            Telefono: "70001111", Email: "edwin@abank.com.sv", Password: null, Direccion: "San Salvador");

        Usuario entity = req.ToEntity(42);

        Assert.Equal(42, entity.Id);
    }
}
