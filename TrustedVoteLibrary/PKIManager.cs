namespace TrustedVoteLibrary;

public class PKIManager
{
    public static Result<(RSA rsa, string publicKey, string privateKey)> GenerateKeys()
    {
        try
        {
            using RSA rsa = RSA.Create(2048);
            string publicKey = Convert.ToBase64String(rsa.ExportRSAPublicKey());
            string privateKey = Convert.ToBase64String(rsa.ExportRSAPrivateKey());
            return (rsa, publicKey, privateKey);
        }
        catch (Exception e)
        {
            return Result.Fail<(RSA rsa, string publicKey, string privateKey)>(e.Message);
        }
    }
}
