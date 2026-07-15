using System.Security.Cryptography;
using System.Text;

namespace Dietcode.Classic.Lib.Cryptography
{
    public class AES
    {
        private AES() { }

        public static string? Encrypt(string text, string? key)
        {
            if (text == null)
                return null;

            var normalizedKey = NormalizeKey(key);

            byte[] keyBytes = Encoding.UTF8.GetBytes(normalizedKey);
            byte[] messageBytes = Encoding.UTF8.GetBytes(text);

            using var aes = Aes.Create();
            aes.Key = keyBytes;
            aes.Mode = CipherMode.ECB;
            aes.Padding = PaddingMode.PKCS7;

            using ICryptoTransform transform = aes.CreateEncryptor();
            byte[] encrypted = transform.TransformFinalBlock(messageBytes, 0, messageBytes.Length);

            return Convert.ToBase64String(encrypted);
        }

        public static string? Decrypt(string? text, string? key)
        {
            if (text == null)
                return null;

            var normalizedKey = NormalizeKey(key);

            byte[] keyBytes = Encoding.UTF8.GetBytes(normalizedKey);
            byte[] messageBytes = Convert.FromBase64String(text);

            using var aes = Aes.Create();
            aes.Key = keyBytes;
            aes.Mode = CipherMode.ECB;
            aes.Padding = PaddingMode.PKCS7;

            using ICryptoTransform transform = aes.CreateDecryptor();
            byte[] decrypted = transform.TransformFinalBlock(messageBytes, 0, messageBytes.Length);

            return Encoding.UTF8.GetString(decrypted);
        }

        private static string NormalizeKey(string? key)
        {
            if (string.IsNullOrWhiteSpace(key))
                return new string(' ', 16);

            var normalized = key!;

            if (normalized.Length < 16)
                return normalized.PadLeft(16, ' ');

            if (normalized.Length > 16)
                return normalized.Substring(0, 16);

            return normalized;
        }
    }
}
