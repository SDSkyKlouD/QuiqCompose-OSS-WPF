using System;
using System.IO;
using System.Linq;
using System.Security;
using System.Security.Cryptography;
using System.Text;

namespace SDSK.QuiqCompose.WinDesktop.Classes.Helpers.Security {
    internal static class StringCryptoExtensions {
        private const int _keySize = 256;

        /// <summary>
        /// Encrypt plainText
        /// </summary>
        /// <param name="plainText">Text to be encrypted</param>
        /// <param name="p">PW</param>
        /// <param name="s">SLT; should ASCII characters</param>
        /// <param name="iv">INITVEC; should exactly 32B(256bit) ASCII characters</param>
        /// <returns>Base64 encoded, encrypted text string</returns>
        internal static SecureString Encrypt(this string plainText, string p, string s, string iv) {
            byte[] ivBytes = Encoding.ASCII.GetBytes(iv).Select(e => (byte) (e >> 1)).ToArray();
            byte[] sBytes = Encoding.ASCII.GetBytes(s + ApplicationData.Instance.CryptoSalt).Select(e => (byte) (e << 1)).ToArray();
            byte[] plainTextBytes = Encoding.Unicode.GetBytes(plainText);
            using(PasswordDeriveBytes pw = new PasswordDeriveBytes(p, sBytes, HashAlgorithmName.SHA256.Name, 3)) {
                byte[] keyBytes = pw.GetBytes(_keySize / 8);
                using(RijndaelManaged symKey = new RijndaelManaged() {
                    Mode = CipherMode.CBC,
                    BlockSize = _keySize,
                    KeySize = _keySize,
                    Padding = PaddingMode.PKCS7
                }) {
                    ICryptoTransform enc = symKey.CreateEncryptor(keyBytes, ivBytes);
                    byte[] cipherTextBytes;
                    using(MemoryStream memoryStream = new MemoryStream()) {
                        using(CryptoStream cryptoStream = new CryptoStream(memoryStream, enc, CryptoStreamMode.Write)) {
                            cryptoStream.Write(plainTextBytes, 0, plainTextBytes.Length);
                        }

                        cipherTextBytes = memoryStream.ToArray();
                    }

                    return Convert.ToBase64String(cipherTextBytes).ToSecureString();
                }
            }
        }

        /// <summary>
        /// Decrypt cipherText
        /// </summary>
        /// <param name="cipherText">Encrypted text to be decrypted</param>
        /// <param name="p">PW</param>
        /// <param name="s">SLT; should ASCII characters</param>
        /// <param name="iv">INITVEC; should exactly 32B(256bit) ASCII characters</param>
        /// <returns>Decrypted text string</returns>
        internal static SecureString Decrypt(this string cipherText, string p, string s, string iv) {
            byte[] ivBytes = Encoding.ASCII.GetBytes(iv).Select(e => (byte) (e >> 1)).ToArray();
            byte[] sBytes = Encoding.ASCII.GetBytes(s + ApplicationData.Instance.CryptoSalt).Select(e => (byte) (e << 1)).ToArray();
            byte[] cipherTextBytes = Convert.FromBase64String(cipherText);
            using(PasswordDeriveBytes pw = new PasswordDeriveBytes(p, sBytes, HashAlgorithmName.SHA256.Name, 3)) {
                byte[] keyBytes = pw.GetBytes(_keySize / 8);
                using(RijndaelManaged symKey = new RijndaelManaged() {
                    Mode = CipherMode.CBC,
                    BlockSize = _keySize,
                    KeySize = _keySize,
                    Padding = PaddingMode.PKCS7
                }) {
                    ICryptoTransform dec = symKey.CreateDecryptor(keyBytes, ivBytes);
                    byte[] plainTextBytes = new byte[cipherTextBytes.Length];
                    int decByteCount;
                    using(MemoryStream memoryStream = new MemoryStream(cipherTextBytes)) {
                        using(CryptoStream cryptoStream = new CryptoStream(memoryStream, dec, CryptoStreamMode.Read)) {
                            decByteCount = cryptoStream.Read(plainTextBytes, 0, plainTextBytes.Length);
                        }
                    }

                    return Encoding.Unicode.GetString(plainTextBytes, 0, decByteCount).ToSecureString();
                }
            }
        }
    }
}
