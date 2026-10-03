using Abank.Api.Models;
using Abank.Api.Models.Dtos;

namespace Abank.Api.Services;

public interface IPasswordHasher
{
    string Hash(string password);
    bool Verify(string password, string hash);
}

public interface IJwtTokenGenerator
{
    string Generate(Usuario usuario);
}

public interface IAuthService
{
    Task<LoginResponse> LoginAsync(LoginRequest request, CancellationToken ct);
}

public interface IUsuarioService
{
    Task<IReadOnlyList<UserResponse>> GetAllAsync(CancellationToken ct);
    Task<UserResponse> GetByIdAsync(int id, CancellationToken ct);
    Task<UserResponse> CreateAsync(CreateUserRequest request, CancellationToken ct);
    Task<UserResponse> UpdateAsync(int id, UpdateUserRequest request, CancellationToken ct);
    Task<UserResponse> DeleteAsync(int id, CancellationToken ct);
}