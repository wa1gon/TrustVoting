using FluentResults;

namespace TrustedVotingLibraryTest;

[TestClass]
public class CATest
{
    [TestMethod]
    public void TestGenerateVoterCertificateWithExtensions()
    {
        // Arrange

        string guid = Guid.NewGuid().ToString();
        
        using (RSA rsa = RSA.Create())
        {
            var caInfo = new CACertInfo()
            {
                Country = "US",
                State = "AR",
                County = "Benton",
                City = "Rogers",
                CommonName = "OpenVoting",
                Id = guid,
                Email = "clerk@anytown.St.US"
            };
            // Act

            Result<X509Certificate2> caCertResult = CertificateAuthority.GenerateCACertificate(caInfo);

            // Assert
            Assert.IsTrue(caCertResult.IsSuccess);
            var cn = caCertResult.Value.GetSubjectValueByName("CN");
            Assert.AreEqual(cn, caInfo.CommonName, "Common Name ");

            CertificateAuthority.SaveCertificateWithPrivateKey(caCertResult.Value,"/tmp/testCA.pfx","kb1etc");
        }
    }
}
