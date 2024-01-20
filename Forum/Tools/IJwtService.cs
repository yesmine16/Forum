using Forum.Models;
using System.Security.Claims;

namespace Forum.Tools
{
    public interface IJwtService
    {
        string GenerateToken(IEnumerable<Claim> claims);

    }
}
