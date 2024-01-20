using Forum.Models;

namespace Forum.Tools
{
    public interface IPasswordSecurity
    {
        public User hashPasswordWithSalt(ref User user);
    }
}
