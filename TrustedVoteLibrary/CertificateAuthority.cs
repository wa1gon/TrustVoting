using FluentResults;

namespace TrustedVoteLibrary;

public class CertificateAuthority
{
    public static Result<X509Certificate2> GenerateCACertificate(CACertInfo info)
    {
        // Generate RSA Key Pair
        using (RSA rsa = RSA.Create(2048))
        {
            try
            {


            // Define the certificate subject
            var subject = info.GenerateSubject();
            var subjectName = new X500DistinguishedName(subject);

            // Create a certificate request for the CA
            var req = new CertificateRequest(subjectName, rsa, HashAlgorithmName.SHA256, RSASignaturePadding.Pkcs1);

            // Add basic constraints: Mark this as a Certificate Authority
            req.CertificateExtensions.Add(new X509BasicConstraintsExtension(true, false, 0, true));

            // Add key usage extension: Specify that this certificate is for key signing
            req.CertificateExtensions.Add(new X509KeyUsageExtension(X509KeyUsageFlags.KeyCertSign | X509KeyUsageFlags.CrlSign, true));

            // Add subject key identifier (optional but recommended)
            req.CertificateExtensions.Add(new X509SubjectKeyIdentifierExtension(req.PublicKey, false));

            // Self-sign the certificate
            var caCert = req.CreateSelfSigned(DateTimeOffset.Now, DateTimeOffset.Now.AddYears(10));

            return caCert;
            }
            catch (Exception e)
            {
                return Result.Fail(e.Message);
            }
        }
    }

    public static Result SaveCertificate(X509Certificate2 certificate, string filePath)
    {
        try
        {
            // Export the certificate as a PFX (with private key) or CER (without private key)
            byte[] certData = certificate.Export(X509ContentType.Pfx);
            System.IO.File.WriteAllBytes(filePath, certData);
            return Result.Ok();
        }
        catch (Exception e)
        {
            return Result.Fail(e.Message);
        }

    }
    public static Result SaveCertificateWithPrivateKey(X509Certificate2 certificate, string filePath, string password)
    {
        // Export the certificate along with the private key as a PFX (PKCS#12)
        byte[] certData = certificate.Export(X509ContentType.Pfx, password);
        System.IO.File.WriteAllBytes(filePath, certData);
    }
}
