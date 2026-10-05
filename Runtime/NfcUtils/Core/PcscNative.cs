using System;
using System.Runtime.InteropServices;

namespace MyUtils.NfcUtils
{
    /// <summary>
    /// Windows標準のスマートカードAPI(winscard.dll / PC/SC)の呼び出し口。
    /// Unityのネイティブ連携なので、Windows(64bit)のエディタ・ビルドでのみ動く
    /// </summary>
    internal static class PcscNative
    {
        public const uint ScopeUser = 0;
        public const uint ShareShared = 2;
        public const uint ProtocolT0 = 1;
        public const uint ProtocolT1 = 2;
        public const uint LeaveCard = 0;

        public const int Success = 0;
        public const int ErrorNoReadersAvailable = unchecked((int)0x8010002E);
        public const int ErrorNoSmartcard = unchecked((int)0x8010000C);
        public const int ErrorUnresponsiveCard = unchecked((int)0x80100066);
        public const int ErrorRemovedCard = unchecked((int)0x80100069);
        public const int ErrorServiceStopped = unchecked((int)0x8010001D);

        [DllImport("winscard.dll")]
        public static extern int SCardEstablishContext(uint scope, IntPtr reserved1, IntPtr reserved2,
            out IntPtr context);

        [DllImport("winscard.dll")]
        public static extern int SCardReleaseContext(IntPtr context);

        // mszReadersには、リーダー名が「\0」区切りで並んで返る(最後は「\0\0」)
        [DllImport("winscard.dll", EntryPoint = "SCardListReadersW", CharSet = CharSet.Unicode)]
        public static extern int SCardListReaders(IntPtr context, string groups, char[] readers,
            ref uint readersLength);

        [DllImport("winscard.dll", EntryPoint = "SCardConnectW", CharSet = CharSet.Unicode)]
        public static extern int SCardConnect(IntPtr context, string reader, uint shareMode,
            uint preferredProtocols, out IntPtr card, out uint activeProtocol);

        [DllImport("winscard.dll")]
        public static extern int SCardDisconnect(IntPtr card, uint disposition);

        [DllImport("winscard.dll")]
        public static extern int SCardTransmit(IntPtr card, IntPtr sendPci, byte[] sendBuffer, uint sendLength,
            IntPtr recvPci, byte[] recvBuffer, ref uint recvLength);
    }
}
