namespace TrustedVoteLibrary;

using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography.X509Certificates;
using Microsoft.IdentityModel.Tokens;

public class JwsExample
{
    public static void Main()
    {
        try
        {
            // Load your X509 Certificate
            var cert = new X509Certificate2("MyCert.pfx", "YourPassword");
            var key = new X509SecurityKey(cert);

            // Create the token handler
            var tokenHandler = new JwtSecurityTokenHandler();

            // Create the JWT Header with the X.509 Certificate
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
                    new Claim("sub", "1234567890"),
                    new Claim("name", "John Doe"),
                    new Claim("admin", "true")
                }),
                Expires = DateTime.UtcNow.AddHours(1),
                SigningCredentials = new X509SigningCredentials(cert)
            };

            // Create and sign the JWT
            var token = tokenHandler.CreateToken(tokenDescriptor);
            var tokenString = tokenHandler.WriteToken(token);

            // Output the token
            Console.WriteLine("JWT: " + tokenString);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }
}
