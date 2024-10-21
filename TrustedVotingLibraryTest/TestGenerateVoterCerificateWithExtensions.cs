

namespace TrustedVotingLibraryTest;

    [TestClass]
    public class CertificateManagerTest
    {
        [TestMethod]
        public void TestGenerateVoterCertificate()
        {
            // Arrange

            string guid = Guid.NewGuid().ToString();
            
            using (RSA rsa = RSA.Create())
            {
                var voteArea = new BallotCertInfo()
                {
                    Country = "US",
                    State = "AR",
                    County = "Benton",
                    City = "Rogers",
                    Ward = "1",
                    Precinct = "2",
                    District = "42",
                    Id = guid,
                    Email = "clerk@anytown.St.US"
                };
                // Act
                var cert = CertificateManager.GenerateVoterCertificateWithExtensions(
                    voteArea, rsa);

                // Assert
                Assert.IsNotNull(cert, "Certificate should not be null");
                Assert.AreEqual(cert.Value.GetSubjectValueByName("CN"), guid, "Common Name above guid");
                Assert.AreEqual(cert.Value.GetSubjectValueByName("E"), voteArea.Email, "Email should match"); }
        }
    }
