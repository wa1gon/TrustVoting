namespace TrustedVotingLibraryTest;

[TestClass]
public class KeyGeneratorTest
{
    [TestMethod]
    public void TestGenerateKeys()
    {
        // Act
        var (rsa, publicKey, privateKey) = PKIManager.GenerateKeys();

        // Assert
        Assert.IsNotNull(rsa, "RSA object should not be null");
        Assert.IsNotNull(publicKey, "Public key should not be null");
        Assert.IsNotNull(privateKey, "Private key should not be null");

        // Optionally, add more assertions to verify key lengths
        // Assert.IsTrue(publicKey.Length > 0, "Public key should not be empty");
        // Assert.IsTrue(privateKey.Length > 0, "Private key should not be empty");
    }
}
