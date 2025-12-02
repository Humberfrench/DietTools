using System;
using System.Security.Cryptography;
using System.Text;

namespace Dietcode.Core.Lib.Cryptography
{
    public class AES
    {
        private AES() { }

        private static string NormalizeKey(string? key)
        {
            if (string.IsNullOrWhiteSpace(key))
                return new string(' ', 16); // mantém compatibilidade

            if (key.Length < 16)
                return key.PadLeft(16, ' ');

            if (key.Length > 16)
                return key.Substring(0, 16);

            return key;
        }

        // ==========================================================
        // ENCRYPT
        // ==========================================================
        public static string? Encrypt(string text, string? key)
        {
            if (text == null) return null;

            key = NormalizeKey(key);
            byte[] keyBytes = Encoding.UTF8.GetBytes(key);
            byte[] messageBytes = Encoding.UTF8.GetBytes(text);

            using var aes = Aes.Create();
            aes.Key = keyBytes;
            aes.Mode = CipherMode.ECB;
            aes.Padding = PaddingMode.PKCS7;

            using ICryptoTransform transform = aes.CreateEncryptor();
            byte[] encrypted = transform.TransformFinalBlock(messageBytes, 0, messageBytes.Length);

            return Convert.ToBase64String(encrypted);
        }

        // ==========================================================
        // DECRYPT
        // ==========================================================
        public static string? Decrypt(string? text, string? key)
        {
            if (text == null) return null;

            key = NormalizeKey(key);
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
