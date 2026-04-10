using BackendService.Services.Interface;
using System.Security.Cryptography;

namespace BackendService.Services
{
    public class PasswordHasherService : IPasswordHasherService
    {
        private const int SaltSize = 16;
        private const int HashSize = 32;
        private const int Iterations = 10000;

        private static readonly HashAlgorithmName AlgorithmName = HashAlgorithmName.SHA512;
        public string Hash(string password)
        {
            byte[] salt = RandomNumberGenerator.GetBytes(SaltSize);
            byte[] hash = Rfc2898DeriveBytes.Pbkdf2(password, salt, Iterations, AlgorithmName, HashSize);

            return $"{Convert.ToHexString(hash)}-{Convert.ToHexString(salt)}";
        }

        public bool VerifyPassword(string password, string passwordHash)
        {
            string[] parts = passwordHash.Split('-');
            byte[] hash = Convert.FromHexString(parts[0]);
            byte[] salt = Convert.FromHexString(parts[1]);

            byte[] inputHash = Rfc2898DeriveBytes.Pbkdf2(password, salt, Iterations, AlgorithmName, HashSize);

            return CryptographicOperations.FixedTimeEquals(hash, inputHash);
        }
    }
}
