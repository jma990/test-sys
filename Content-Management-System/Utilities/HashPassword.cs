
using System.Text;
using System.Security.Cryptography;
using Konscious.Security.Cryptography;

namespace Content_Management_System.Utilities
{
    public class HashPassword
    {
        private static readonly int SaltSize = 16; // Size of salt in bytes
        private static readonly int HashSize = 32; // Size of hash in bytes

        public static string This(string password, string salt)
        {
            // Hash the password using Argon2
            byte[] passwordBytes = Encoding.UTF8.GetBytes(password);
            byte[] saltBytes = Convert.FromBase64String(salt);
            byte[] passwordWithSaltBytes = new byte[passwordBytes.Length + saltBytes.Length];

            Buffer.BlockCopy(passwordBytes, 0, passwordWithSaltBytes, 0, passwordBytes.Length);
            Buffer.BlockCopy(saltBytes, 0, passwordWithSaltBytes, passwordBytes.Length, saltBytes.Length);

            using (var hasher = new Argon2id(passwordWithSaltBytes))
            {
                hasher.DegreeOfParallelism = 8; // Adjust based on server's capabilities
                hasher.MemorySize = 65536; // 64MB
                hasher.Iterations = 4;

                byte[] hash = hasher.GetBytes(HashSize); // Generate a hash
                return Convert.ToBase64String(hash); // Return the hash as a Base64 string
            }
        }

        // Generate a new salt
        public static string GenerateSalt()
        {
            byte[] saltBytes = new byte[SaltSize];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(saltBytes);
            }
            return Convert.ToBase64String(saltBytes);
        }

    }
}