using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;

namespace backend_api.Services
{
    public static class JwtAuth
    {
        private const string SecretKey = "SuperSecretKey123456789012345678"; //Secret key to generate token 
        public static string GenerateToken(string username, int id, string role)
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(SecretKey)); //Set the Secret Key
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256); //Set the Secret Key was a credential

            var claims = new[] //Set the body of jwt, calls claim
            {
                new Claim(ClaimTypes.Name, username),
                new Claim("EntityId", id.ToString()),
                new Claim(ClaimTypes.Role, role),
                // You can add more claims based on your requirements
            };

            var finalToken = new JwtSecurityToken(
                issuer: "your-issuer",  // Change this to your issuer
                audience: "your-audience",  // Change this to your audience
                claims: claims,
                expires: DateTime.Now.AddHours(3), //Time to expire in 3h
                signingCredentials: credentials
            );

            string token = new JwtSecurityTokenHandler().WriteToken(finalToken);
            return token; //Return a generate token
        }

        public static bool ValidateToken(string token) //Function to validate the status of token
        {
            if (string.IsNullOrEmpty(token)) //Reject the function when the token is empty or null
            {
                return false;
            }

            var tokenHandler = new JwtSecurityTokenHandler();
            var securityKey = Encoding.UTF8.GetBytes(SecretKey); //Check the token using the secret key 

            try
            {
                tokenHandler.ValidateToken(token, new TokenValidationParameters //Check the validation of token jwt
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(securityKey),
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    ClockSkew = TimeSpan.Zero
                }, out SecurityToken validatedToken);
                return true; // Token with permissions 
            }
            catch
            {
                return false; // Token unauthorized
            }
        }

        public static (int? userId, string? username) DecoderUserIdUsername(string token) //Function to decode the information inside token, will decode the token for a username and a id
        {
            if (token is null)
            {
                return (null, null);
            }

            var handler = new JwtSecurityTokenHandler(); //Set the handler jwtSecurity
            var jwtToken = handler.ReadToken(token) as JwtSecurityToken; //Read the token 

            var userIdClaim = jwtToken?.Claims.FirstOrDefault(c => c.Type == "EntityId"); //Claim the UserId from token

            string? usernameClaim = jwtToken?.Claims
            .FirstOrDefault(c => c.Type == ClaimTypes.Name || c.Type == "name")?.Value; //Claim the Username from token

            return userIdClaim != null && int.TryParse(userIdClaim.Value, out int userId) && !string.IsNullOrEmpty(usernameClaim) ? (userId, usernameClaim) : (null, null); //Condition where the username and id need to be true to return own value, if haven't data return null
        }

        public static (int? userId, string? username, string? roleType) Decoder(string token) //Function to decode the information inside token, will decode the token for a role type
        {
            if (string.IsNullOrEmpty(token))
            {
                return (null, null, null);
            }

            var handler = new JwtSecurityTokenHandler(); //Set the handler jwtSecurity
            var jwtToken = handler.ReadToken(token) as JwtSecurityToken; //Read the token 

            var userIdClaim = jwtToken?.Claims.FirstOrDefault(c => c.Type == "EntityId"); //Claim the UserId from token

            string? roleTypeClaim = jwtToken?.Claims
            .FirstOrDefault(c => c.Type == ClaimTypes.Role || c.Type == "role")?.Value; //Claim the role type from token

            string? usernameClaim = jwtToken?.Claims
            .FirstOrDefault(c => c.Type == ClaimTypes.Name || c.Type == "name")?.Value; //Claim the Username from token

            return !string.IsNullOrEmpty(roleTypeClaim) && !string.IsNullOrEmpty(usernameClaim) && userIdClaim != null && int.TryParse(userIdClaim.Value, out int userId)  ? (userId, usernameClaim, roleTypeClaim) : (null, null, null); //Condition where the role type need to be true to return own value, if haven't data return null
        }
    }
}