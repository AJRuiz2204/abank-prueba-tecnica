using Abank.Api.Models;
using Abank.Api.Models.Dtos;

namespace Abank.Api.Services;

public static class UsuarioMappings
{
    public static UserResponse ToResponse(this Usuario u) =>
        new(u.Id, u.Nombres, u.Apellidos, DateOnly.FromDateTime(u.FechaNacimiento), u.Telefono, u.Email, u.Direccion);

    public static UserSessionDto ToSessionDto(this Usuario u) =>
        new(u.Id, u.Nombres, u.Apellidos, true, DateOnly.FromDateTime(u.FechaNacimiento), u.Email, u.Telefono, u.Direccion);

    public static Usuario ToEntity(this CreateUserRequest r, string passwordHash) => new()
    {
        Nombres = r.Nombres.Trim(),
        Apellidos = r.Apellidos.Trim(),
        FechaNacimiento = r.FechaNacimiento.ToDateTime(TimeOnly.MinValue),
        Direccion = r.Direccion.Trim(),
        Telefono = r.Telefono.Trim(),
        Email = r.Email.Trim().ToLowerInvariant(),
        Password = passwordHash
    };

    public static Usuario ToEntity(this UpdateUserRequest r, int id) => new()
    {
        Id = id,
        Nombres = r.Nombres.Trim(),
        Apellidos = r.Apellidos.Trim(),
        FechaNacimiento = r.FechaNacimiento.ToDateTime(TimeOnly.MinValue),
        Direccion = r.Direccion.Trim(),
        Telefono = r.Telefono.Trim(),
        Email = r.Email.Trim().ToLowerInvariant()
    };
}