namespace Abank.Api.Models;

/// <summary>
/// Entidad que representa la tabla usuarios. Uso interno, nunca se expone por la API
/// </summary>
public sealed class Usuario
{
    public int Id { get; init; }
    public string Nombres { get; init; } = string.Empty;
    public string Apellidos { get; init; } = string.Empty;
    public DateTime FechaNacimiento { get; init; }
    public string Direccion { get; init; } = string.Empty;
    public string Password { get; init; } = string.Empty;
    public string Telefono { get; init; } = string.Empty;
    public string Email { get; init; } = string.Empty;
    public string Estado { get; init; } = "A";
    public DateTime FechaCreacion { get; init; }
    public DateTime? FechaModificacion { get; init; }
}
