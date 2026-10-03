using Abank.Api.Services;
using Xunit;

namespace Abank.Tests;

public class PasswordHasherTests
{
    private readonly BCryptPasswordHasher _hasher = new();

    [Fact]
    public void Hash_no_devuelve_texto_plano()
    {
        string hash = _hasher.Hash("Abank123*");

        Assert.NotEqual("Abank123*", hash);
        Assert.StartsWith("$2", hash);
    }

    [Fact]
    public void Verify_true_para_password_correcta()
    {
        string hash = _hasher.Hash("Abank123*");
        Assert.True(_hasher.Verify("Abank123*", hash));
    }

    [Fact]
    public void Verify_false_para_password_incorrecta()
    {
        string hash = _hasher.Hash("Abank123*");
        Assert.False(_hasher.Verify("otra-clave", hash));
    }
}
