using System.Security.Cryptography;
using System.Text;

namespace Dietcode.Core.Lib.Cryptography
{
    public class AES
    {
        private AES() { }

        // Deriva uma chave forte de 32 bytes (AES-256) a partir de string (senha/chave)
        // Para uso interno simples: PBKDF2 com salt fixo (melhor: salt por app no config + rotação)
        private static byte[] DeriveKey(string? key)
        {
            key ??= "";

            // Ideal: vir do appsettings/secret e ser único por app
            byte[] salt = Encoding.UTF8.GetBytes("Dietcode.FixedSalt.v1");

            // .NET moderno: usar o método estático Pbkdf2
            return Rfc2898DeriveBytes.Pbkdf2(
                password: Encoding.UTF8.GetBytes(key),
                salt: salt,
                iterations: 100_000,
                hashAlgorithm: HashAlgorithmName.SHA256,
                outputLength: 32 // AES-256
            );
        }
        // ==========================================================
        // ENCRYPT (AES-GCM) => retorna "v2:{base64(nonce|tag|cipher)}"
        // ==========================================================
        public static string? Encrypt(string text, string? key)
        {
            if (text == null) return null;

            byte[] keyBytes = DeriveKey(key);
            byte[] nonce = RandomNumberGenerator.GetBytes(12);
            byte[] plainBytes = Encoding.UTF8.GetBytes(text);

            byte[] cipherBytes = new byte[plainBytes.Length];
            byte[] tag = new byte[16];

            using var aes = new AesGcm(keyBytes, tagSizeInBytes: 16);
            {
                aes.Encrypt(nonce, plainBytes, cipherBytes, tag);
            }

            // pacote: nonce(12) + tag(16) + cipher(n)
            byte[] combined = new byte[12 + 16 + cipherBytes.Length];
            Buffer.BlockCopy(nonce, 0, combined, 0, 12);
            Buffer.BlockCopy(tag, 0, combined, 12, 16);
            Buffer.BlockCopy(cipherBytes, 0, combined, 28, cipherBytes.Length);

            return "v2:" + Convert.ToBase64String(combined);
        }

        // ==========================================================
        // DECRYPT
        // - suporta legado (ECB atual) sem prefixo
        // - suporta novo "v2:"
        // ==========================================================
        public static string? Decrypt(string? text, string? key)
        {
            if (text == null) return null;

            if (text.StartsWith("v2:", StringComparison.Ordinal))
            {
                byte[] keyBytes = DeriveKey(key);
                byte[] combined = Convert.FromBase64String(text.Substring(3));

                if (combined.Length < 28)
                    throw new CryptographicException("Ciphertext inválido.");

                byte[] nonce = new byte[12];
                byte[] tag = new byte[16];
                byte[] cipherBytes = new byte[combined.Length - 28];

                Buffer.BlockCopy(combined, 0, nonce, 0, 12);
                Buffer.BlockCopy(combined, 12, tag, 0, 16);
                Buffer.BlockCopy(combined, 28, cipherBytes, 0, cipherBytes.Length);

                byte[] plainBytes = new byte[cipherBytes.Length];

                using var aes = new AesGcm(keyBytes, tagSizeInBytes: 16);
                {
                    aes.Decrypt(nonce, cipherBytes, tag, plainBytes);
                }

                return Encoding.UTF8.GetString(plainBytes);
            }

            // ===== Legado: mantém compatibilidade com seu ECB antigo =====
            return DecryptLegacyEcb(text, key);
        }

        // ====== Seu código atual (legado) isolado ======
        private static string NormalizeKeyLegacy(string? key)
        {
            if (string.IsNullOrWhiteSpace(key))
                return new string(' ', 16);

            if (key.Length < 16)
                return key.PadLeft(16, ' ');

            if (key.Length > 16)
                return key.Substring(0, 16);

            return key;
        }

        private static string? DecryptLegacyEcb(string text, string? key)
        {
            key = NormalizeKeyLegacy(key);
            byte[] keyBytes = Encoding.UTF8.GetBytes(key);
            byte[] messageBytes = Convert.FromBase64String(text);

            using var aes = Aes.Create();
            aes.Key = keyBytes;
            aes.Mode = CipherMode.ECB;
            aes.Padding = PaddingMode.PKCS7;

            using ICryptoTransform transform = aes.CreateDecryptor();
            byte[] decrypted = transform.TransformFinalBlock(messageBytes, 0, messageBytes.Length);

            return Encoding.UTF8.GetString(decrypted);
        }
    }
}