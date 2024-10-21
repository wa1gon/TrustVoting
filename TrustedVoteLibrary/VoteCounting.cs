namespace TrustedVoteLibrary;

public class VoteCounting
{
    public static Result<string> DecryptVote(byte[] encryptedVote, RSA authorityRsa)
    {
        try
        {
            byte[] decryptedBytes = authorityRsa.Decrypt(encryptedVote, RSAEncryptionPadding.OaepSHA256);
            return Encoding.UTF8.GetString(decryptedBytes);
        }
        catch (Exception e)
        {
            return Result.Fail<string>(e.Message);
        }
    }
}
