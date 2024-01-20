using Forum.Models;
using System.Security.Cryptography;
using System.Text;

namespace Forum.Tools
{
    public class PassWordSecurityService : IPasswordSecurity
    {
        private readonly IConfiguration _configuration;

        public PassWordSecurityService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public User hashPasswordWithSalt(ref User user)
        {
            if(user is null)
            {
                return null;
            }
            var provider = SHA512.Create();
            var salt = _configuration["SaltPassWord:Secret"];
            
            byte[] bytes = provider.ComputeHash(Encoding.ASCII.GetBytes(salt + user.MotDePasse));
            user.MotDePasse = BitConverter.ToString(bytes);
            return user;
        }
    }
}
