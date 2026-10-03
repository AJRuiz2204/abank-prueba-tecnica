using Abank.Api.Common;
using Abank.Api.Data;
using Abank.Api.Models;
using Abank.Api.Models.Dtos;
using Abank.Api.Models.Validators;
using Abank.Api.Services;
using Moq;
using Xunit;

namespace Abank.Tests;

public class AuthServiceTests
{
    private readonly Mock<IUsuarioRepository> _repo = new();
    private readonly Mock<IPasswordHasher> _hasher = new();
    private readonly Mock<IJwtTokenGenerator> _jwt = new();

    private AuthService BuildSut() =>
        new(_repo.Object, _hasher.Object, _jwt.Object, new LoginRequestValidator());

    private static Usuario Usuario() => new()
    {
        Id = 1, Nombres = "Edwin", Apellidos = "García", Telefono = "70001111",
        Email = "edwin@abank.com.sv", Password = "$2a$11$hash", Estado = "A",
        FechaNacimiento = new DateTime(1990, 5, 10)
    };

    [Fact]
    public async Task Login_credenciales_correctas_devuelve_token()
    {
        _repo.Setup(r => r.GetByTelefonoAsync("70001111", It.IsAny<CancellationToken>())).ReturnsAsync(Usuario());
        _hasher.Setup(h => h.Verify("Abank123*", "$2a$11$hash")).Returns(true);
        _jwt.Setup(j => j.Generate(It.IsAny<Usuario>())).Returns("token-firmado");

        LoginResponse resp = await BuildSut().LoginAsync(new LoginRequest("70001111", "Abank123*"), CancellationToken.None);

        Assert.Equal("token-firmado", resp.AccessToken);
        Assert.Equal("bearer", resp.TokenType);
        Assert.True(resp.User.SessionActive);
        Assert.Equal("70001111", resp.User.Telefono);
    }

    [Fact]
    public async Task Login_password_incorrecta_lanza_Unauthorized()
    {
        _repo.Setup(r => r.GetByTelefonoAsync(It.IsAny<string>(), It.IsAny<CancellationToken>())).ReturnsAsync(Usuario());
        _hasher.Setup(h => h.Verify(It.IsAny<string>(), It.IsAny<string>())).Returns(false);

        await Assert.ThrowsAsync<UnauthorizedException>(() =>
            BuildSut().LoginAsync(new LoginRequest("70001111", "mala"), CancellationToken.None));
    }

    [Fact]
    public async Task Login_usuario_inexistente_lanza_Unauthorized()
    {
        _repo.Setup(r => r.GetByTelefonoAsync(It.IsAny<string>(), It.IsAny<CancellationToken>())).ReturnsAsync((Usuario?)null);

        await Assert.ThrowsAsync<UnauthorizedException>(() =>
            BuildSut().LoginAsync(new LoginRequest("99999999", "Abank123*"), CancellationToken.None));

        _jwt.Verify(j => j.Generate(It.IsAny<Usuario>()), Times.Never);
    }
}
