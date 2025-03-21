﻿using System.Security.Cryptography;
using System.Text;

namespace Dietcode.Core.Lib
{
    public static class Cryptography
    {
        /// <summary>
        /// Encrypts a text value by a given 16 character key using AES.
        /// </summary>
        /// <param name="text">The text to encrypt.</param>
        /// <param name="key">A string up to 16 characters.</param>
        /// <returns></returns>
        public static string? Encrypt(string text, string? key)
        {
            key = key?.Length < 16 ? key?.PadLeft(16, ' ') : key?.Length > 16 ? key?.Substring(0, 16) : key;

            string? result = null;

            byte[]? keyBytes = key != null ? Encoding.UTF8.GetBytes(key) : null;
            byte[] messageBytes = Encoding.UTF8.GetBytes(text);

            if (keyBytes == null) { return null; }

            Aes aes = Aes.Create();
            aes.Key = keyBytes;
            aes.Mode = CipherMode.ECB;
            aes.Padding = PaddingMode.PKCS7;

            //using (ICryptoTransform transform = aes.CreateEncryptor(aes.Key, Encoding.UTF8.GetBytes(iv)))
            using (ICryptoTransform transform = aes.CreateEncryptor())
            {
                byte[] encryptedBytes = transform.TransformFinalBlock(messageBytes, 0, messageBytes.Length);
                result = Convert.ToBase64String(encryptedBytes);
            }

            return result;
        }

        public static string? Decrypt(string? text, string? key)
        {
            if (text == null || key == null) { return null; }

            key = key?.Length < 16 ? key?.PadLeft(16, ' ') : key?.Length > 16 ? key?.Substring(0, 16) : key;

            string? result = null;

            byte[]? keyBytes = key != null ? System.Text.Encoding.UTF8.GetBytes(key) : null;
            byte[] messageBytes = Convert.FromBase64String(text);

            if (keyBytes == null) { return null; }

            Aes aes = Aes.Create();
            aes.Key = keyBytes;
            aes.Mode = CipherMode.ECB;
            aes.Padding = PaddingMode.PKCS7;

            using (ICryptoTransform transform = aes.CreateDecryptor())
            {
                byte[] decryptedBytes = transform.TransformFinalBlock(messageBytes, 0, messageBytes.Length);
                result = System.Text.Encoding.UTF8.GetString(decryptedBytes);
            }

            return result;
        }

    }
}
