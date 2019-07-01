using System;
using System.Text;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;

namespace Tweezers.Identity.HashUtils
{
    public static class Hash
    {
        public static string Create(string value)
        {
            string salt = Salt.Value;
            byte[] valueBytes = KeyDerivation.Pbkdf2(
                password: value,
                salt: Encoding.UTF8.GetBytes(salt),
                prf: KeyDerivationPrf.HMACSHA512,
                iterationCount: 10000,
                numBytesRequested: 256);

            return Convert.ToBase64String(valueBytes);
        }

        public static bool Validate(string value, string hash)
            => Create(value) == hash;
    }
}
