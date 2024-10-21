namespace TrustedVoteLibrary;
public class CertificateGenerator
{
    public static Result<X509Certificate2> CreateCertificate(BallotCertInfo ballot, X509Certificate2 caCert, RSA? caPrivateKey, 
        int yearsValid = 1)
    {
        try
        {
            // Generate RSA key pair for the new certificate
            using (RSA rsa = RSA.Create(2048))
            {
                // Create the certificate request
                var req = new CertificateRequest(
                    new X500DistinguishedName(ballot.GenerateSubject()), 
                    rsa, 
                    HashAlgorithmName.SHA256, 
                    RSASignaturePadding.Pkcs1);

                // Add extensions (optional)
                req.CertificateExtensions.Add(new X509KeyUsageExtension(X509KeyUsageFlags.DigitalSignature, true));
                req.CertificateExtensions.Add(new X509BasicConstraintsExtension(false, false, 0, false));
                req.CertificateExtensions.Add(new X509SubjectKeyIdentifierExtension(req.PublicKey, false));

                // Sign the request with the CA's private key
                DateTimeOffset notBefore = DateTimeOffset.Now;
                DateTimeOffset notAfter = notBefore.AddYears(yearsValid);

                // Create a certificate signed by the CA
                X509Certificate2 signedCert = req.Create(
                    caCert, 
                    notBefore, 
                    notAfter, 
                    new ReadOnlySpan<byte>(Guid.NewGuid().ToByteArray()));

                // Combine the new certificate with the private key and export it
                return signedCert.CopyWithPrivateKey(rsa);
            }            
        }
        catch (Exception e)
        {
            return Result.Fail(e.Message);
        }
    }
}
/*
To create a certificate using a Certificate Authority (CA) certificate in C#, you will follow these general steps:

Generate a Key Pair: Generate a new RSA key pair for the certificate that will be signed by the CA.
    Create a Certificate Request (CSR): Use the generated key pair to create a certificate signing request (CSR).
    Sign the Certificate: Use the CA's private key to sign the CSR, thereby generating the new certificate.
    Save the Signed Certificate: Save the signed certificate to a file or use it within your application.
    Here's how you can implement this in C#:

Step-by-Step Implementation
1. Set Up the Environment
    Ensure that you have the necessary namespaces:

csharp
    Copy code
using System;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
2. Generate the Key Pair and Create a CSR
This involves creating the key pair for the new certificate and then generating a CSR:
*/
