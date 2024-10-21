using System.Security.Cryptography;
using System.Text;

namespace TrustedVoteLibrary;

public class Voting
{
    public static Result<byte[]> SignVote(string vote, RSA rsa)
    {
        try
        {
            byte[] voteBytes = Encoding.UTF8.GetBytes(vote);
            return rsa.SignData(voteBytes, HashAlgorithmName.SHA256, RSASignaturePadding.Pkcs1);    
        }
        catch (Exception e)
        {
            return Result.Fail<byte[]>(e.Message);
        }
    }
}
