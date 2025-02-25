using System.Security.Cryptography;
using System.Text;

namespace VaultX.Extensions
{
    public static class SecurityExtensions
    {
        internal static readonly int randomStringBytes = 5000;
        
        public static string GetRandomString()
        {
            var randomBytes = new byte[randomStringBytes];
            using (var rng = RandomNumberGenerator.Create())
                rng.GetBytes(randomBytes);
            return Convert.ToBase64String(randomBytes);
        }

        public static string GetStringHash(this string plaintext)
        {
            var inputBytes = Encoding.UTF8.GetBytes(plaintext);
            return Convert.ToHexString(SHA256.HashData(inputBytes));
        }
    }
}