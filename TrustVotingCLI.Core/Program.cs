using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using Microsoft.Extensions.Configuration;
using TrustedVoteLibrary;

class Program
{
    
    static void Main(string[] args)
    {
        if (args.Length == 0)
        {
            Console.WriteLine("Please provide a command: createCACert or createCert");
            return;
        }

        var configuration = new ConfigurationBuilder()
            .SetBasePath(AppContext.BaseDirectory)
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
            .Build();

        CACertInfo caCertInfo = new CACertInfo
        {
            CACertFileName = configuration["CA:CACertFileName"],
            CACertPath = configuration["CA:CACertPath"],
            City = configuration["CA:City"],
            Country = configuration["CA:Country"],
            County = configuration["CA:County"],
            Email = configuration["CA:Email"],
            FriendlyName = configuration["CA:FriendlyName"],
            Id = Guid.NewGuid().ToString(),
            Issuer = configuration["CA:Issuer"],
            Organization = configuration["CA:Organization"],
            OrganizationalUnit = configuration["CA:OrganizationUnit"],
            Password = configuration["CA:Password"],
            State = configuration["CA:State"],
            ValidFrom = configuration["CA:ValidFrom"],
            ValidTo = configuration["CA:ValidTo"]
        };
        var caCertPath =  configuration["CA:CaCertPath"];
        var caCertFileName = configuration["CA:CaCertFileName"];
        
        
        switch (args[0].ToLower())
        {
            case "createcacert":
                CreateCACert(caCertInfo);
                break;
            case "createcert":
                string? optionalFileName = args.Length > 1 ? args[1] : null;
                // CreateCert(caCertInfo);
                break;
            default:
                Console.WriteLine("Invalid command. Use createCACert or createCert.");
                break;
        }
    }

    static void CreateCACert(CACertInfo caCertInfo)
    {
        var caCert = CertificateAuthority.GenerateCACertificate(caCertInfo);
        string fullName = System.IO.Path.Combine(caCertInfo.CACertPath, caCertInfo.CACertFileName);
        CertificateAuthority.SaveCertificateWithPrivateKey(caCert, fullName, caCertInfo.Password);

    }

    static void CreateCert(string certPath, string certFileName, string caCertPath, string caCertFileName, string caCertPassword)
    {
        // Load the CA certificate and private key
        X509Certificate2 caCert = new X509Certificate2(System.IO.Path.Combine(caCertPath, caCertFileName), caCertPassword);
        RSA? caPrivateKey = caCert.GetRSAPrivateKey();

        // Subject name for the new certificate
        string subjectName = "CN=MySignedCert, O=MyOrganization, C=US";

        // Create the certificate signed by the CA
        if (caPrivateKey != null)
        {
            // X509Certificate2 signedCert = CertificateGenerator.CreateCertificate(subjectName, caCert, caPrivateKey);

            // Save the signed certificate with the private key to a PFX file
            // byte[] pfxData = signedCert.Export(X509ContentType.Pfx, "CertPassword");
            // System.IO.File.WriteAllBytes(System.IO.Path.Combine(certPath, certFileName), pfxData);

            Console.WriteLine("Certificate created and saved successfully.");
        }
        else
        {
            Console.WriteLine("Failed to get CA private key.");
        }
    }
}
