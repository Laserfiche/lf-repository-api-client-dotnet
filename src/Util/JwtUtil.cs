using Microsoft.IdentityModel.JsonWebTokens;
using System.Security.Claims;

namespace Laserfiche.Repository.Api.Client.Util
{
    internal static class JwtUtil
    {
        internal static JsonWebToken ReadJWT(string jwt)
        {
            return new JsonWebTokenHandler().ReadJsonWebToken(jwt);
        }

        internal static string GetAccountIdFromJwt(JsonWebToken jwt)
        {
            return jwt.TryGetClaim("csid", out Claim claim) ? claim.Value : "";
        }
    }
}
