using System;
using System.Collections.Generic;
using System.Text;

namespace MyUtils.NfcUtils
{
    /// <summary>
    /// NDEFメッセージのうち、テキストレコード(Well Known Type "T")だけを扱う変換処理。
    /// タグに書く中身(NDEFメッセージ)との相互変換のみで、カードの読み書きは <see cref="Type2Tag"/> が担当する
    /// </summary>
    public static class NdefText
    {
        // レコードヘッダのビット
        private const byte MessageBegin = 0x80;
        private const byte MessageEnd = 0x40;
        private const byte ShortRecord = 0x10;
        private const byte IdLengthPresent = 0x08;
        private const byte TypeNameFormatMask = 0x07;
        private const byte TypeNameFormatWellKnown = 0x01;

        private const byte TextType = (byte)'T';
        private const byte Utf16Flag = 0x80;
        private const byte LanguageLengthMask = 0x3F;

        /// <summary>
        /// テキスト1件だけを持つNDEFメッセージを作る
        /// </summary>
        /// <param name="text">書き込む文字列(UTF-8)</param>
        /// <param name="languageCode">言語コード(例: ja, en)</param>
        public static byte[] Encode(string text, string languageCode)
        {
            var language = Encoding.ASCII.GetBytes(languageCode ?? string.Empty);
            if (language.Length == 0 || language.Length > LanguageLengthMask)
            {
                throw new NfcException("言語コードは1〜63文字の英数字で指定してください");
            }

            var body = Encoding.UTF8.GetBytes(text ?? string.Empty);
            int payloadLength = 1 + language.Length + body.Length;
            bool isShort = payloadLength <= byte.MaxValue;

            var message = new List<byte>
            {
                (byte)(MessageBegin | MessageEnd | (isShort ? ShortRecord : 0) | TypeNameFormatWellKnown),
                1 // タイプ長("T"の1バイト)
            };

            if (isShort)
            {
                message.Add((byte)payloadLength);
            }
            else
            {
                message.Add((byte)(payloadLength >> 24));
                message.Add((byte)(payloadLength >> 16));
                message.Add((byte)(payloadLength >> 8));
                message.Add((byte)payloadLength);
            }

            message.Add(TextType);
            message.Add((byte)language.Length); // ステータス(UTF-8 + 言語コード長)
            message.AddRange(language);
            message.AddRange(body);
            return message.ToArray();
        }

        /// <summary>
        /// NDEFメッセージから、最初のテキストレコードを取り出す。テキストがなければfalse
        /// </summary>
        public static bool TryDecode(byte[] message, out string text, out string languageCode)
        {
            text = null;
            languageCode = null;

            int index = 0;
            while (index < message.Length)
            {
                if (!TryReadRecord(message, ref index, out var type, out var payload, out bool isLast))
                {
                    return false;
                }

                if (type.Length == 1 && type[0] == TextType && payload.Length >= 1)
                {
                    byte status = payload[0];
                    int languageLength = status & LanguageLengthMask;
                    if (payload.Length < 1 + languageLength) return false;

                    var encoding = (status & Utf16Flag) != 0 ? Encoding.BigEndianUnicode : Encoding.UTF8;
                    languageCode = Encoding.ASCII.GetString(payload, 1, languageLength);
                    text = encoding.GetString(payload, 1 + languageLength, payload.Length - 1 - languageLength);
                    return true;
                }

                if (isLast) break;
            }

            return false;
        }

        // レコードを1件読み、indexを次のレコードの先頭へ進める。壊れていればfalse
        private static bool TryReadRecord(byte[] message, ref int index, out byte[] type, out byte[] payload,
            out bool isLast)
        {
            type = null;
            payload = null;
            isLast = false;

            if (message.Length - index < 3) return false;

            byte header = message[index++];
            isLast = (header & MessageEnd) != 0;
            int typeLength = message[index++];

            int payloadLength;
            if ((header & ShortRecord) != 0)
            {
                payloadLength = message[index++];
            }
            else
            {
                if (message.Length - index < 4) return false;
                payloadLength = (message[index] << 24) | (message[index + 1] << 16) |
                                (message[index + 2] << 8) | message[index + 3];
                index += 4;
            }

            int idLength = 0;
            if ((header & IdLengthPresent) != 0)
            {
                if (message.Length - index < 1) return false;
                idLength = message[index++];
            }

            if (payloadLength < 0 || message.Length - index < typeLength + idLength + payloadLength) return false;

            type = new byte[typeLength];
            Array.Copy(message, index, type, 0, typeLength);
            index += typeLength + idLength;

            // Well Known以外のレコードは、typeを空にして(テキストとして扱わずに)読み飛ばす
            if ((header & TypeNameFormatMask) != TypeNameFormatWellKnown) type = Array.Empty<byte>();

            payload = new byte[payloadLength];
            Array.Copy(message, index, payload, 0, payloadLength);
            index += payloadLength;
            return true;
        }
    }
}
