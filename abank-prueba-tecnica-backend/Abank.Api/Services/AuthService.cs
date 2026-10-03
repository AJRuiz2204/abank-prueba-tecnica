using Abank.Api.Common;
using Abank.Api.Data;
using Abank.Api.Models;
using Abank.Api.Models.Dtos;
using FluentValidation;

namespace Abank.Api.Services;

public sealed class AuthService(
    IUsuarioRepository repository,
    IPasswordHasher hasher,
    IJwtTokenGenerator tokenGenerator,
    IValidator<LoginRequest> validator) : IAuthService
{
    public async Task<LoginResponse> LoginAsync(LoginRequest request, CancellationToken ct)
    {
        await validator.ValidateAndThrowAsync(request, ct);

        Usuario? usuario = await repository.GetByTelefonoAsync(request.Telefono.Trim(), ct);

        if (usuario is null || !hasher.Verify(request.Password, usuario.Password))
            throw new UnauthorizedException("Teléfono o contraseña incorrectos.");

        return new LoginResponse(usuario.ToSessionDto(), tokenGenerator.Generate(usuario));
    }
}
