using Abank.Api.Common;
using Abank.Api.Models;
using Dapper;
using Npgsql;

namespace Abank.Api.Data;

public sealed class UsuarioRepository(NpgsqlDataSource dataSource) : IUsuarioRepository
{
    private const string Columns =
        "id, nombres, apellidos, fecha_nacimiento, direccion, telefono, email, estado, fecha_creacion, fecha_modificacion";

    public async Task<IReadOnlyList<Usuario>> GetAllAsync(CancellationToken ct)
    {
        await using NpgsqlConnection cn = await dataSource.OpenConnectionAsync(ct);
        IEnumerable<Usuario> rows = await cn.QueryAsync<Usuario>(new CommandDefinition(
            $"SELECT {Columns} FROM usuarios WHERE estado = 'A' ORDER BY id", cancellationToken: ct));
        return rows.AsList();
    }

    public async Task<Usuario?> GetByIdAsync(int id, CancellationToken ct)
    {
        await using NpgsqlConnection cn = await dataSource.OpenConnectionAsync(ct);
        return await cn.QuerySingleOrDefaultAsync<Usuario>(new CommandDefinition(
            $"SELECT {Columns} FROM usuarios WHERE id = @id AND estado = 'A'", new { id }, cancellationToken: ct));
    }

    public async Task<Usuario?> GetByTelefonoAsync(string telefono, CancellationToken ct)
    {
        await using NpgsqlConnection cn = await dataSource.OpenConnectionAsync(ct);
        return await cn.QuerySingleOrDefaultAsync<Usuario>(new CommandDefinition(
            $"SELECT {Columns}, password FROM usuarios WHERE telefono = @telefono AND estado = 'A'",
            new { telefono }, cancellationToken: ct));
    }

    public async Task<Usuario> CreateAsync(Usuario u, CancellationToken ct)
    {
        const string sql = $"""
            INSERT INTO usuarios (nombres, apellidos, fecha_nacimiento, direccion, password, telefono, email)
            VALUES (@Nombres, @Apellidos, @FechaNacimiento, @Direccion, @Password, @Telefono, @Email)
            RETURNING {Columns};
            """;

        return await ExecuteWithConflictHandling(cn =>
            cn.QuerySingleAsync<Usuario>(new CommandDefinition(sql, new
            {
                u.Nombres,
                u.Apellidos,
                u.FechaNacimiento,
                u.Direccion,
                u.Password,
                u.Telefono,
                u.Email
            }, cancellationToken: ct)), ct);
    }

    public async Task<Usuario?> UpdateAsync(Usuario u, string? passwordHash, CancellationToken ct)
    {
        const string sql = $"""
            UPDATE usuarios SET
                nombres = @Nombres,
                apellidos = @Apellidos,
                fecha_nacimiento = @FechaNacimiento,
                direccion = @Direccion,
                telefono = @Telefono,
                email = @Email,
                password = COALESCE(@PasswordHash, password)
            WHERE id = @Id AND estado = 'A'
            RETURNING {Columns};
            """;

        return await ExecuteWithConflictHandling(cn =>
            cn.QuerySingleOrDefaultAsync<Usuario>(new CommandDefinition(sql, new
            {
                u.Id,
                u.Nombres,
                u.Apellidos,
                u.FechaNacimiento,
                u.Direccion,
                u.Telefono,
                u.Email,
                PasswordHash = passwordHash
            }, cancellationToken: ct)), ct);
    }

    public async Task<Usuario?> DeactivateAsync(int id, CancellationToken ct)
    {
        await using NpgsqlConnection cn = await dataSource.OpenConnectionAsync(ct);
        return await cn.QuerySingleOrDefaultAsync<Usuario>(new CommandDefinition(
            $"UPDATE usuarios SET estado = 'I' WHERE id = @id AND estado = 'A' RETURNING {Columns}",
            new { id }, cancellationToken: ct));
    }

    private async Task<T> ExecuteWithConflictHandling<T>(Func<NpgsqlConnection, Task<T>> action, CancellationToken ct)
    {
        await using NpgsqlConnection cn = await dataSource.OpenConnectionAsync(ct);
        try
        {
            return await action(cn);
        }
        catch (PostgresException ex) when (ex.SqlState == PostgresErrorCodes.UniqueViolation)
        {
            throw new ConflictException(ex.ConstraintName switch
            {
                "uq_usuarios_telefono" => "El teléfono ya está registrado.",
                "uq_usuarios_email" => "El email ya está registrado.",
                _ => "El registro ya existe."
            });
        }
    }
}