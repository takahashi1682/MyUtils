using System;
using System.Security.Cryptography;
using MyUtils.JsonUtils;

namespace MyUtils.NfcUtils
{
    /// <summary>
    /// タグに書く文字をパスワードで暗号化・復号する。
    /// AES暗号化は <see cref="AESEncryption"/> を使い、ここではパスワードから鍵を作る処理と、
    /// パスワードの間違いを見つけるための認証タグだけを足している。
    ///
    /// 暗号化した文字は "enc1:" + Base64 の文字列になり、タグには普通のテキストとして書かれる。
    /// 中身は「ソルト 8バイト + AES暗号文 + 認証タグ 8バイト」。
    /// パスワードからPBKDF2で鍵(AES 16バイト + IV 16バイト + 認証用 16バイト)を作る。
    /// 復号のとき認証タグを確かめるので、パスワードが違うと必ず失敗する。
    ///
    /// ※ 学習用のサンプルです。タグ自体の書き込み保護ではなく、中身を読めなくするだけです
    /// </summary>
    public static class NfcCrypto
    {
        /// <summary>暗号化された文字であることを示す先頭の印</summary>
        public const string Prefix = "enc1:";

        private const int SaltLength = 8;
        private const int TagLength = 8;
        private const int Iterations = 10000;

        private const int KeyLength = 16;
        private const int IvLength = 16;
        private const int MacKeyLength = 16;

        /// <summary>
        /// 暗号化された文字か(先頭が "enc1:" か)
        /// </summary>
        public static bool IsEncrypted(string text)
        {
            return text != null && text.StartsWith(Prefix, StringComparison.Ordinal);
        }

        /// <summary>
        /// 文字をパスワードで暗号化する。同じ文字・パスワードでも、毎回違う結果になる
        /// </summary>
        public static string Encrypt(string text, string password)
        {
            var salt = AESEncryption.GenerateRandomIV(SaltLength);
            DeriveKeys(password, salt, out var key, out var iv, out var macKey);

            var cipher = Convert.FromBase64String(AESEncryption.Encrypt(text ?? string.Empty, iv, key));

            var data = new byte[SaltLength + cipher.Length + TagLength];
            Buffer.BlockCopy(salt, 0, data, 0, SaltLength);
            Buffer.BlockCopy(cipher, 0, data, SaltLength, cipher.Length);

            var tag = ComputeTag(macKey, data, SaltLength + cipher.Length);
            Buffer.BlockCopy(tag, 0, data, SaltLength + cipher.Length, TagLength);

            return Prefix + Convert.ToBase64String(data);
        }

        /// <summary>
        /// 暗号化された文字を、パスワードで元に戻す。パスワードが違えば NfcException
        /// </summary>
        public static string Decrypt(string encrypted, string password)
        {
            if (!IsEncrypted(encrypted)) throw new NfcException("暗号化されたデータではありません");

            byte[] data;
            try
            {
                data = Convert.FromBase64String(encrypted.Substring(Prefix.Length));
            }
            catch (FormatException)
            {
                throw new NfcException("暗号化されたデータが壊れています");
            }

            // 最低でも、ソルト + 暗号文1ブロック(16バイト) + 認証タグ
            if (data.Length < SaltLength + 16 + TagLength) throw new NfcException("暗号化されたデータが壊れています");

            var salt = new byte[SaltLength];
            Buffer.BlockCopy(data, 0, salt, 0, SaltLength);
            DeriveKeys(password, salt, out var key, out var iv, out var macKey);

            // 先に認証タグを確かめる(パスワードが違う、またはデータが壊れていれば、ここで分かる)
            int bodyLength = data.Length - TagLength;
            var expected = ComputeTag(macKey, data, bodyLength);
            int difference = 0;
            for (int i = 0; i < TagLength; i++)
            {
                difference |= expected[i] ^ data[bodyLength + i];
            }

            if (difference != 0) throw new NfcException("パスワードが違います");

            var cipher = new byte[bodyLength - SaltLength];
            Buffer.BlockCopy(data, SaltLength, cipher, 0, cipher.Length);
            return AESEncryption.Decrypt(Convert.ToBase64String(cipher), iv, key);
        }

        // パスワードとソルトから、暗号化の鍵・IV・認証用の鍵を作る
        private static void DeriveKeys(string password, byte[] salt, out byte[] key, out byte[] iv, out byte[] macKey)
        {
            using (var derive = new Rfc2898DeriveBytes(password ?? string.Empty, salt, Iterations,
                       HashAlgorithmName.SHA256))
            {
                var bytes = derive.GetBytes(KeyLength + IvLength + MacKeyLength);
                key = new byte[KeyLength];
                iv = new byte[IvLength];
                macKey = new byte[MacKeyLength];
                Buffer.BlockCopy(bytes, 0, key, 0, KeyLength);
                Buffer.BlockCopy(bytes, KeyLength, iv, 0, IvLength);
                Buffer.BlockCopy(bytes, KeyLength + IvLength, macKey, 0, MacKeyLength);
            }
        }

        // data の先頭 length バイトの認証タグ(HMAC-SHA256の先頭8バイト)
        private static byte[] ComputeTag(byte[] macKey, byte[] data, int length)
        {
            using (var hmac = new HMACSHA256(macKey))
            {
                var hash = hmac.ComputeHash(data, 0, length);
                var tag = new byte[TagLength];
                Buffer.BlockCopy(hash, 0, tag, 0, TagLength);
                return tag;
            }
        }
    }
}
