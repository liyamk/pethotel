using PetHotel.IServices;
using System.Security.Cryptography;
using System.Text;

namespace PetHotel.Services
{
    // https://code-maze.com/csharp-hashing-salting-passwords-best-practices/
    internal class HashService : IHashService
    {
        private const int keySize = 64;
        private const int iterations = 350000;
        private readonly HashAlgorithmName hashAlgorithm = HashAlgorithmName.SHA512;
        public string GetHashedValue(string password, out string saltEncoded)
        {
            byte[] salt = RandomNumberGenerator.GetBytes(keySize);
            saltEncoded = Convert.ToHexString(salt);

            var hash = Rfc2898DeriveBytes.Pbkdf2(
                Encoding.UTF8.GetBytes(password),
                salt,
                iterations,
                hashAlgorithm,
                keySize);

            return Convert.ToHexString(hash);
        }

        public bool VerifyPassword(string password, string hashedPassword, string saltEncoded)
        {
            byte[] salt = Convert.FromHexString(saltEncoded);
            var hashToCompare = Rfc2898DeriveBytes.Pbkdf2(password, salt, iterations, hashAlgorithm, keySize);

            // the below will compare for fixed amount of time - decrease password size guessing
            return CryptographicOperations.FixedTimeEquals(hashToCompare, Convert.FromHexString(hashedPassword));
        }
    }
}
