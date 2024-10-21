namespace TrustedVoteLibrary;

public class VoterRegistration
{
    private Dictionary<string, X509Certificate2> registeredVoters = new Dictionary<string, X509Certificate2>();

    public void RegisterVoter(string voterId, X509Certificate2 certificate)
    {
        registeredVoters[voterId] = certificate;
    }

    public X509Certificate2 GetVoterCertificate(string voterId)
    {
        return registeredVoters[voterId];
    }
}
