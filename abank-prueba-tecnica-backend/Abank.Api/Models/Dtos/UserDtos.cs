using System.Text.Json.Serialization;

namespace Abank.Api.Models.Dtos;

/// <summary>
/// Datos comunes de Create y Update; permite reutilizar validaciones (DRY)
/// </summary>
public interface IUserPayload
{
    string Nombres { get; }
    string Apellidos { get; }
    DateOnly FechaNacimiento { get; }
    string Telefono { get; }
    string Email { get; }
    string Direccion { get; }
}

public sealed record UserResponse(
    [property: JsonPropertyName("id")] int Id,
    [property: JsonPropertyName("nombres")] string Nombres,
    [property: JsonPropertyName("apellidos")] string Apellidos,
    [property: JsonPropertyName("fechanacimiento")] DateOnly FechaNacimiento,
    [property: JsonPropertyName("telefono")] string Telefono,
    [property: JsonPropertyName("email")] string Email,
    [property: JsonPropertyName("direccion")] string? Direccion);

public sealed record CreateUserRequest(
    [property: JsonPropertyName("nombres")] string Nombres,
    [property: JsonPropertyName("apellidos")] string Apellidos,
    [property: JsonPropertyName("fechanacimiento")] DateOnly FechaNacimiento,
    [property: JsonPropertyName("telefono")] string Telefono,
    [property: JsonPropertyName("email")] string Email,
    [property: JsonPropertyName("password")] string Password,
    [property: JsonPropertyName("direccion")] string Direccion) : IUserPayload;

public sealed record UpdateUserRequest(
    [property: JsonPropertyName("nombres")] string Nombres,
    [property: JsonPropertyName("apellidos")] string Apellidos,
    [property: JsonPropertyName("fechanacimiento")] DateOnly FechaNacimiento,
    [property: JsonPropertyName("telefono")] string Telefono,
    [property: JsonPropertyName("email")] string Email,
    [property: JsonPropertyName("password")] string? Password,
    [property: JsonPropertyName("direccion")] string Direccion) : IUserPayload;