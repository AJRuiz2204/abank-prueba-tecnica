using Abank.Api.Common;
using Abank.Api.Data;
using Abank.Api.Models;
using Abank.Api.Models.Dtos;
using Abank.Api.Models.Validators;
using Abank.Api.Services;
using Moq;
using Xunit;

namespace Abank.Tests;

public class UsuarioServiceTests
{
    private readonly Mock<IUsuarioRepository> _repo = new();
    private readonly Mock<IPasswordHasher> _hasher = new();

    private UsuarioService BuildSut() =>
        new(_repo.Object, _hasher.Object, new CreateUserRequestValidator(), new UpdateUserRequestValidator());

    private static CreateUserRequest CreateValido() => new(
        Nombres: "Edwin", Apellidos: "García", FechaNacimiento: new DateOnly(1990, 5, 10),
        Telefono: "70001111", Email: "edwin@abank.com.sv", Password: "Abank123*", Direccion: "San Salvador");

    private static UpdateUserRequest UpdateValido(string? password) => new(
        Nombres: "Edwin", Apellidos: "García", FechaNacimiento: new DateOnly(1990, 5, 10),
        Telefono: "70001111", Email: "edwin@abank.com.sv", Password: password, Direccion: "San Salvador");

    [Fact]
    public async Task GetById_inexistente_lanza_NotFound()
    {
        _repo.Setup(r => r.GetByIdAsync(It.IsAny<int>(), It.IsAny<CancellationToken>())).ReturnsAsync((Usuario?)null);

        await Assert.ThrowsAsync<NotFoundException>(() => BuildSut().GetByIdAsync(99, CancellationToken.None));
    }

    [Fact]
    public async Task Create_guarda_password_hasheada()
    {
        Usuario? capturado = null;
        _hasher.Setup(h => h.Hash("Abank123*")).Returns("HASH");
        _repo.Setup(r => r.CreateAsync(It.IsAny<Usuario>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Usuario u, CancellationToken _) => { capturado = u; return u; });

        UserResponse resp = await BuildSut().CreateAsync(CreateValido(), CancellationToken.None);

        Assert.NotNull(capturado);
        Assert.Equal("HASH", capturado!.Password);
        Assert.Equal("edwin@abank.com.sv", resp.Email);
    }

    [Fact]
    public async Task Update_sin_password_no_rehashea_y_pasa_null()
    {
        string? hashRecibido = "centinela";
        _repo.Setup(r => r.UpdateAsync(It.IsAny<Usuario>(), It.IsAny<string?>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Usuario u, string? h, CancellationToken _) => { hashRecibido = h; return u; });

        await BuildSut().UpdateAsync(1, UpdateValido(password: null), CancellationToken.None);

        Assert.Null(hashRecibido);
        _hasher.Verify(h => h.Hash(It.IsAny<string>()), Times.Never);
    }

    [Fact]
    public async Task Update_con_password_rehashea()
    {
        string? hashRecibido = null;
        _hasher.Setup(h => h.Hash("NuevaClave1")).Returns("NUEVO-HASH");
        _repo.Setup(r => r.UpdateAsync(It.IsAny<Usuario>(), It.IsAny<string?>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Usuario u, string? h, CancellationToken _) => { hashRecibido = h; return u; });

        await BuildSut().UpdateAsync(1, UpdateValido(password: "NuevaClave1"), CancellationToken.None);

        Assert.Equal("NUEVO-HASH", hashRecibido);
    }

    [Fact]
    public async Task Delete_inexistente_lanza_NotFound()
    {
        _repo.Setup(r => r.DeactivateAsync(It.IsAny<int>(), It.IsAny<CancellationToken>())).ReturnsAsync((Usuario?)null);

        await Assert.ThrowsAsync<NotFoundException>(() => BuildSut().DeleteAsync(99, CancellationToken.None));
    }
}
