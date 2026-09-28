using System.Security.Cryptography;
using System.Text;

namespace Industrie.Services
{
    public interface IEncryptionService
    {
        string Encrypt(string plainText);
        string Decrypt(string cipherText);
    }
    public class EncryptionService : IEncryptionService
    {
        private readonly byte[] _key;

        public EncryptionService(IConfiguration configuration)
        {
            // Récupère une clé de 256 bits (32 octets) depuis appsettings.json
            string secretKey = configuration["Encryption:Key"]
                ?? throw new InvalidOperationException("La clé de chiffrement est manquante.");

            _key = Encoding.UTF8.GetBytes(secretKey.PadRight(32).Substring(0, 32));
        }

        public string Encrypt(string plainText)
        {
            if (string.IsNullOrEmpty(plainText)) return plainText;

            byte[] nonce = new byte[AesGcm.NonceByteSizes.MaxSize]; // 12 bytes
            RandomNumberGenerator.Fill(nonce);

            byte[] plainBytes = Encoding.UTF8.GetBytes(plainText);
            byte[] cipherBytes = new byte[plainBytes.Length];
            byte[] tag = new byte[AesGcm.TagByteSizes.MaxSize]; // 16 bytes

            using (var aesGcm = new AesGcm(_key, AesGcm.TagByteSizes.MaxSize))
            {
                aesGcm.Encrypt(nonce, plainBytes, cipherBytes, tag);
            }

            // Combine Nonce + Tag + CipherText en un seul tableau de bytes
            byte[] result = new byte[nonce.Length + tag.Length + cipherBytes.Length];
            Buffer.BlockCopy(nonce, 0, result, 0, nonce.Length);
            Buffer.BlockCopy(tag, 0, result, nonce.Length, tag.Length);
            Buffer.BlockCopy(cipherBytes, 0, result, nonce.Length + tag.Length, cipherBytes.Length);

            return Convert.ToBase64String(result);
        }

        public string Decrypt(string cipherText)
        {
            if (string.IsNullOrWhiteSpace(cipherText)) return cipherText ?? string.Empty;

            try
            {
                byte[] fullCipher = Convert.FromBase64String(cipherText);

                int nonceSize = AesGcm.NonceByteSizes.MaxSize;
                int tagSize = AesGcm.TagByteSizes.MaxSize;

                // Validation de la longueur minimale du conteneur sécurisé
                if (fullCipher.Length < nonceSize + tagSize)
                {
                    return "[Erreur format]";
                }

                int cipherSize = fullCipher.Length - nonceSize - tagSize;

                byte[] nonce = fullCipher[..nonceSize];
                byte[] tag = fullCipher[nonceSize..(nonceSize + tagSize)];
                byte[] cipherBytes = fullCipher[(nonceSize + tagSize)..];

                byte[] plainBytes = new byte[cipherSize];

                using (var aesGcm = new AesGcm(_key, AesGcm.TagByteSizes.MaxSize))
                {
                    aesGcm.Decrypt(nonce, cipherBytes, tag, plainBytes);
                }

                return Encoding.UTF8.GetString(plainBytes);
            }
            catch (Exception)
            {
                // Dégradation gracieuse par ligne pour préserver l'intégrité du circuit Blazor Server
                return "[Erreur déchiffrement]";
            }
        }
    }
}