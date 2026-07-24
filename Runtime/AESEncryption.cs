using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace MyUtils
{
    public static class AESEncryption
    {
        /// <summary>
        /// 平文をAESで暗号化
        /// </summary>
        /// <param name="plainText"></param>
        /// <param name="iv"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        public static string Encrypt(object plainText, byte[] iv, string key)
        {
            using var aes = CreateAesCipher(iv, key);
            using var encryptor = aes.CreateEncryptor(aes.Key, aes.IV);
            using var ms = new MemoryStream();
            using var cryptoStream = new CryptoStream(ms, encryptor, CryptoStreamMode.Write);
            using var writer = new StreamWriter(cryptoStream);
            writer.Write(plainText);
            writer.Flush();
            cryptoStream.FlushFinalBlock();

            return Convert.ToBase64String(ms.ToArray());
        }

        /// <summary>
        /// AESで暗号化されたBase64文字列を復号
        /// </summary>
        /// <param name="cipherText"></param>
        /// <param name="iv"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        public static string Decrypt(string cipherText, byte[] iv, string key)
        {
            using var aes = CreateAesCipher(iv, key);
            using var decryptor = aes.CreateDecryptor(aes.Key, aes.IV);
            using var ms = new MemoryStream(Convert.FromBase64String(cipherText));
            using var cryptoStream = new CryptoStream(ms, decryptor, CryptoStreamMode.Read);
            using var reader = new StreamReader(cryptoStream);

            return reader.ReadToEnd();
        }

        /// <summary>
        /// ランダムなIV（初期化ベクトル）を生成
        /// </summary>
        /// <param name="size"></param>
        /// <returns></returns>
        public static byte[] GenerateRandomIV(int size = 16)
        {
            byte[] iv = new byte[size];
            using var rng = RandomNumberGenerator.Create();
            rng.GetBytes(iv);
            return iv;
        }

        /// <summary>
        /// byte配列を16進数文字列に変換
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static string BytesToHex(byte[] data)
            => BitConverter.ToString(data).Replace("-", string.Empty);

        /// <summary>
        /// 16進数文字列をbyte配列に変換
        /// </summary>
        /// <param name="hex"></param>
        /// <returns></returns>
        public static byte[] HexToBytes(string hex)
        {
            int length = hex.Length;
            byte[] bytes = new byte[length / 2];
            for (int i = 0; i < length; i += 2)
            {
                bytes[i / 2] = Convert.ToByte(hex.Substring(i, 2), 16);
            }

            return bytes;
        }

        /// <summary>
        /// AES暗号器を構築
        /// </summary>
        /// <param name="iv"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        private static RijndaelManaged CreateAesCipher(byte[] iv, string key)
        {
            return new RijndaelManaged
            {
                BlockSize = 128,
                KeySize = 128,
                Mode = CipherMode.CBC,
                Padding = PaddingMode.PKCS7,
                IV = iv,
                Key = Encoding.UTF8.GetBytes(key)
            };
        }
    }
}