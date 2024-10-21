namespace TrustedVoteLibrary;

public class VoteEncryption
{
    public static Result<byte[]> EncryptVote(string vote, RSA authorityRsa)
    {
        try
        {
            byte[] voteBytes = Encoding.UTF8.GetBytes(vote);
            return authorityRsa.Encrypt(voteBytes, RSAEncryptionPadding.OaepSHA256);
        }
        catch (Exception e)
        {
            return Result.Fail<byte[]>(e.Message);
        }

    }
}
