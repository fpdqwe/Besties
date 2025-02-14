using BCrypt.Net;
using BLL.Abstractions;

namespace BLL.Infrastructure
{
    public class PasswordHasher : IPasswordHasher
    {
        public string Hash(string password)
        {
            return BCrypt.Net.BCrypt.EnhancedHashPassword(password);
        }

        public bool Verify(string value, string password)
        {
            return BCrypt.Net.BCrypt.EnhancedVerify(value, password);
        }
    }
}
