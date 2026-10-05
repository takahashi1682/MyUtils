using System;
using System.ComponentModel;

namespace MyUtils.NfcUtils
{
    /// <summary>
    /// PC/SCの接続コンテキスト。リーダーの一覧取得と、カードへの接続を行う。
    /// 使い終わったら <see cref="Dispose"/> で解放する
    /// </summary>
    public sealed class PcscContext : IDisposable
    {
        private IntPtr _context;

        public PcscContext()
        {
            Check(PcscNative.SCardEstablishContext(PcscNative.ScopeUser, IntPtr.Zero, IntPtr.Zero, out _context));
        }

        /// <summary>
        /// 接続されているリーダー名の一覧。リーダーがなければ空配列
        /// </summary>
        public string[] ListReaders()
        {
            uint length = 0;
            int result = PcscNative.SCardListReaders(_context, null, null, ref length);
            if (result == PcscNative.ErrorNoReadersAvailable) return Array.Empty<string>();
            Check(result);

            var buffer = new char[length];
            result = PcscNative.SCardListReaders(_context, null, buffer, ref length);
            if (result == PcscNative.ErrorNoReadersAvailable) return Array.Empty<string>();
            Check(result);

            return new string(buffer, 0, (int)length).Split(new[] { '\0' }, StringSplitOptions.RemoveEmptyEntries);
        }

        /// <summary>
        /// リーダー上のカードに接続する。カードが置かれていない(または応答しない)ときはnull
        /// </summary>
        public PcscCard TryConnect(string readerName)
        {
            int result = PcscNative.SCardConnect(_context, readerName, PcscNative.ShareShared,
                PcscNative.ProtocolT0 | PcscNative.ProtocolT1, out var card, out var protocol);

            if (result == PcscNative.ErrorNoSmartcard ||
                result == PcscNative.ErrorRemovedCard ||
                result == PcscNative.ErrorUnresponsiveCard)
            {
                return null;
            }

            Check(result);
            return new PcscCard(card, protocol);
        }

        public void Dispose()
        {
            if (_context == IntPtr.Zero) return;

            PcscNative.SCardReleaseContext(_context);
            _context = IntPtr.Zero;
        }

        internal static void Check(int result)
        {
            if (result == PcscNative.Success) return;

            // PC/SCのエラーコードはWindowsのエラーメッセージとして引ける
            string message = $"PC/SCエラー: {new Win32Exception(result).Message} (0x{result:X8})";

            // スマートカードサービスは、リーダーを接続するまで起動していないことが多い
            if (result == PcscNative.ErrorServiceStopped)
            {
                message += " リーダーを接続するか、Windowsの「Smart Card」サービスを開始してください";
            }

            throw new NfcException(message);
        }
    }
}
