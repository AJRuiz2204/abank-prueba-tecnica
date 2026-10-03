using System.Text.Json.Serialization;

namespace Abank.Api.Models.Dtos;

public sealed record LoginRequest(
    [property: JsonPropertyName("telefono")] string Telefono,
    [property: JsonPropertyName("password")] string Password);

public sealed record LoginResponse(
    [property: JsonPropertyName("user")] UserSessionDto User,
    [property: JsonPropertyName("access_token")] string AccessToken,
    [property: JsonPropertyName("token_type")] string TokenType = "bearer");

public sealed record UserSessionDto(
    [property: JsonPropertyName("id")] int Id,
    [property: JsonPropertyName("nombres")] string Nombres,
    [property: JsonPropertyName("apellidos")] string Apellidos,
    [property: JsonPropertyName("session_active")] bool SessionActive,
    [property: JsonPropertyName("fechanacimiento")] DateOnly FechaNacimiento,
    [property: JsonPropertyName("email")] string Email,
    [property: JsonPropertyName("telefono")] string Telefono,
    [property: JsonPropertyName("direccion")] string Direccion);