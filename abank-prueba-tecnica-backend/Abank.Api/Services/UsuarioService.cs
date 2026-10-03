using Abank.Api.Common;
using Abank.Api.Data;
using Abank.Api.Models;
using Abank.Api.Models.Dtos;
using FluentValidation;

namespace Abank.Api.Services;

public sealed class UsuarioService(
    IUsuarioRepository repository,
    IPasswordHasher hasher,
    IValidator<CreateUserRequest> createValidator,
    IValidator<UpdateUserRequest> updateValidator) : IUsuarioService
{
    public async Task<IReadOnlyList<UserResponse>> GetAllAsync(CancellationToken ct)
    {
        IReadOnlyList<Usuario> usuarios = await repository.GetAllAsync(ct);
        return usuarios.Select(u => u.ToResponse()).ToList();
    }

    public async Task<UserResponse> GetByIdAsync(int id, CancellationToken ct) =>
        (await repository.GetByIdAsync(id, ct))?.ToResponse() ?? throw NotFound(id);

    public async Task<UserResponse> CreateAsync(CreateUserRequest request, CancellationToken ct)
    {
        await createValidator.ValidateAndThrowAsync(request, ct);
        Usuario creado = await repository.CreateAsync(request.ToEntity(hasher.Hash(request.Password)), ct);
        return creado.ToResponse();
    }

    public async Task<UserResponse> UpdateAsync(int id, UpdateUserRequest request, CancellationToken ct)
    {
        await updateValidator.ValidateAndThrowAsync(request, ct);
        string? hash = request.Password is null ? null : hasher.Hash(request.Password);
        Usuario? actualizado = await repository.UpdateAsync(request.ToEntity(id), hash, ct);
        return actualizado?.ToResponse() ?? throw NotFound(id);
    }

    public async Task<UserResponse> DeleteAsync(int id, CancellationToken ct) =>
        (await repository.DeactivateAsync(id, ct))?.ToResponse() ?? throw NotFound(id);

    private static NotFoundException NotFound(int id) => new($"No existe un usuario activo con id {id}.");
}
