using FluentAssertions;
using Services.Security;

namespace Tests;

public class PasswordHasherTests
{
    [Fact]
    public void Hash_ShouldReturnDifferentHashes_ForSamePassword()
    {
        var password = "Str0ng!Passw0rd";

        var hash1 = PasswordHasher.Hash(password);
        var hash2 = PasswordHasher.Hash(password);

        hash1.Should().NotBeNullOrWhiteSpace();
        hash2.Should().NotBeNullOrWhiteSpace();
        hash1.Should().NotBe(hash2);
    }

    [Fact]
    public void Verify_ShouldReturnTrue_ForMatchingPassword()
    {
        var password = "Str0ng!Passw0rd";
        var hash = PasswordHasher.Hash(password);

        var ok = PasswordHasher.Verify(password, hash);

        ok.Should().BeTrue();
    }

    [Fact]
    public void Verify_ShouldReturnFalse_ForWrongPassword()
    {
        var password = "Str0ng!Passw0rd";
        var hash = PasswordHasher.Hash(password);

        var ok = PasswordHasher.Verify("WrongPassword", hash);

        ok.Should().BeFalse();
    }

    [Fact]
    public void Verify_ShouldReturnFalse_ForInvalidHash()
    {
        var ok = PasswordHasher.Verify("pw", "invalid-format");

        ok.Should().BeFalse();
    }
}
