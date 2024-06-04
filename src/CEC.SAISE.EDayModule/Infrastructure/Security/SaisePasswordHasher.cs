using System;
using System.Security.Cryptography;
using System.Text;
using Microsoft.AspNet.Identity;

namespace CEC.SAISE.EDayModule.Infrastructure.Security
{
    public class SaisePasswordHasher : IPasswordHasher
    {
        public string HashPassword(string password)
        {
            SHA1 sha1 = new SHA1CryptoServiceProvider();
            return Convert.ToBase64String(sha1.ComputeHash(Encoding.Unicode.GetBytes(password)));
        }

        public PasswordVerificationResult VerifyHashedPassword(string hashedPassword, string providedPassword)
        {
            var hash = HashPassword(providedPassword);
            if (hashedPassword == hash)
            {
                return PasswordVerificationResult.Success;
            }
            else
            {
                return PasswordVerificationResult.Failed;
            }
        }
    }
}