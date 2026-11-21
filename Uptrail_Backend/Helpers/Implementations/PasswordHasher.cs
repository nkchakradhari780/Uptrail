using Microsoft.AspNetCore.Identity;
using Uptrail_Backend.Helpers.Interfaces;

namespace Uptrail_Backend.Helpers.Implementations
{
    public class PasswordHasher : IPasswordHasher
    {
        public string HashPassword(string password)
        {
            var PasswordHash = BCrypt.Net.BCrypt.HashPassword(password);
            return PasswordHash;
        }

        public bool VerifyPassword(string password, string hashedPassword)
        {
            var isPasswordValid = BCrypt.Net.BCrypt.Verify(password, hashedPassword);
            return isPasswordValid;
        }
    }
}
