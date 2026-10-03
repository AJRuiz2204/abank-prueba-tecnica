using Abank.Api.Models;

namespace Abank.Api.Data;

public interface IUsuarioRepository
{
    Task<IReadOnlyList<Usuario>> GetAllAsync(CancellationToken ct);
    Task<Usuario?> GetByIdAsync(int id, CancellationToken ct);
    Task<Usuario?> GetByTelefonoAsync(string telefono, CancellationToken ct);
    Task<Usuario> CreateAsync(Usuario usuario, CancellationToken ct);
    Task<Usuario?> UpdateAsync(Usuario usuario, string? passwordHash, CancellationToken ct);
    Task<Usuario?> DeactivateAsync(int id, CancellationToken ct);
}