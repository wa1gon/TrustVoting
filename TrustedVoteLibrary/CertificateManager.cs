namespace TrustedVoteLibrary;
public class CertificateManager
{
    /*
       CN stands for Common Name.
        O stands for Organization.
        OU stands for Organizational Unit.
        C stands for Country.
        L stands for Locality (e.g., city).
        S stands for State or Province.
        STREET (Street Address)
        DC (Domain Component)
        UID (User ID)
        EMAIL (Email Address)
     */
    public static X509Certificate2 GenerateVoterCertificateWithExtensions(
        string state, string votingDistrict, string balletNum, RSA rsa)
    {
        var request = new CertificateRequest($"CN={balletNum}, C={state}, L={votingDistrict}",
            rsa, HashAlgorithmName.SHA256, RSASignaturePadding.Pkcs1);

        // Add the voting district as an extension
        var votingDistrictExtension = new X509Extension(
            new Oid("1.2.3.4.5.6.7.8.1", "Voting District"), System.Text.Encoding.ASCII.GetBytes(votingDistrict),
            false);
        request.CertificateExtensions.Add(votingDistrictExtension);

        // Create the certificate
        var certificate = request.CreateSelfSigned(DateTimeOffset.Now, DateTimeOffset.Now.AddYears(1));
        return certificate;
    }
}
