using System;
using System.Collections.Generic;

namespace MyUtils.NfcUtils
{
    /// <summary>
    /// NFC Forum Type 2 タグ(NTAG213/215/216、MIFARE Ultralight など)の読み書き。
    /// PC/SCリーダーの擬似APDU(FF CA / FF B0 / FF D6)を使い、NDEFメッセージを読み書きする。
    /// ※ FeliCa(Suicaなど)やMIFARE Classicは、メモリ構造が違うため対象外
    /// </summary>
    public class Type2Tag
    {
        // 1ページ = 4バイト。ページ3がCapability Container、ページ4からがユーザーデータ領域
        private const int PageSize = 4;
        private const int CapabilityPage = 3;
        private const int FirstDataPage = 4;
        private const int ReadLength = 16;

        // Capability Containerの先頭バイト(NDEF対応タグの印)
        private const byte NdefMagicNumber = 0xE1;

        // TLV(Tag-Length-Value)
        private const byte NullTlv = 0x00;
        private const byte NdefTlv = 0x03;
        private const byte TerminatorTlv = 0xFE;
        private const byte LongLengthMarker = 0xFF;

        private readonly IApduChannel _channel;

        public Type2Tag(IApduChannel channel)
        {
            _channel = channel;
        }

        /// <summary>
        /// タグのUIDを読む
        /// </summary>
        public byte[] ReadUid()
        {
            return Send("UIDの読み取り", 0xFF, 0xCA, 0x00, 0x00, 0x00);
        }

        /// <summary>
        /// NDEFメッセージを読む。メッセージが書かれていなければ空配列
        /// </summary>
        public byte[] ReadNdefMessage()
        {
            int capacity = ReadCapacity();
            var data = new List<byte>();

            // 必要なバイト数が読み込み済みになるまで、続きを読み足す
            void Fill(int length)
            {
                while (data.Count < length && data.Count < capacity)
                {
                    data.AddRange(ReadPages(FirstDataPage + data.Count / PageSize));
                }

                if (data.Count < length) throw new NfcException("タグのデータが壊れています");
            }

            int index = 0;
            while (true)
            {
                Fill(index + 1);
                byte tag = data[index];

                if (tag == NullTlv)
                {
                    index++;

                    // 領域の最後まで空白なら、メッセージは書かれていない(初期状態のタグ)
                    if (index >= capacity) return Array.Empty<byte>();
                    continue;
                }

                if (tag == TerminatorTlv) return Array.Empty<byte>();

                Fill(index + 2);
                int length = data[index + 1];
                int start = index + 2;
                if (length == LongLengthMarker)
                {
                    Fill(index + 4);
                    length = (data[index + 2] << 8) | data[index + 3];
                    start = index + 4;
                }

                if (tag == NdefTlv)
                {
                    Fill(start + length);
                    return data.GetRange(start, length).ToArray();
                }

                // NDEF以外のTLV(ロック情報など)は読み飛ばす
                index = start + length;
            }
        }

        /// <summary>
        /// NDEFメッセージを書き込む(以前のメッセージは置き換わる)
        /// </summary>
        public void WriteNdefMessage(byte[] message)
        {
            int capacity = ReadCapacity();

            var tlv = new List<byte> { NdefTlv };
            if (message.Length < LongLengthMarker)
            {
                tlv.Add((byte)message.Length);
            }
            else
            {
                tlv.Add(LongLengthMarker);
                tlv.Add((byte)(message.Length >> 8));
                tlv.Add((byte)message.Length);
            }

            tlv.AddRange(message);
            tlv.Add(TerminatorTlv);

            if (tlv.Count > capacity)
            {
                throw new NfcException($"データが大きすぎます(必要 {tlv.Count} バイト / 空き {capacity} バイト)");
            }

            // ページ(4バイト)単位で書くため、端数は0で埋める
            while (tlv.Count % PageSize != 0) tlv.Add(0x00);

            for (int offset = 0; offset < tlv.Count; offset += PageSize)
            {
                WritePage(FirstDataPage + offset / PageSize, tlv.GetRange(offset, PageSize).ToArray());
            }
        }

        // ユーザーデータ領域のバイト数(Capability Containerの3バイト目 × 8)
        private int ReadCapacity()
        {
            var capability = ReadPages(CapabilityPage);
            if (capability.Length < PageSize || capability[0] != NdefMagicNumber)
            {
                throw new NfcException("NDEF形式のタグではありません(未フォーマット、または対応していないタグです)");
            }

            return capability[2] * 8;
        }

        // 指定ページから読む(リーダーによって返るバイト数が違うので、そのまま返す)
        private byte[] ReadPages(int page)
        {
            var data = Send("読み取り", 0xFF, 0xB0, 0x00, (byte)page, ReadLength);
            if (data.Length == 0) throw new NfcException("タグから読み取れませんでした");
            return data;
        }

        private void WritePage(int page, byte[] data)
        {
            var command = new byte[5 + PageSize];
            command[0] = 0xFF;
            command[1] = 0xD6;
            command[2] = 0x00;
            command[3] = (byte)page;
            command[4] = PageSize;
            Array.Copy(data, 0, command, 5, PageSize);
            Send("書き込み", command);
        }

        private byte[] Send(string operation, params byte[] command)
        {
            var response = _channel.Transmit(command, out var statusWord);
            if (statusWord != 0x9000)
            {
                throw new NfcException($"{operation}に失敗しました(ステータス 0x{statusWord:X4})");
            }

            return response;
        }
    }
}
