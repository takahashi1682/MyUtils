using System;
using System.Runtime.InteropServices;

namespace MyUtils.NfcUtils
{
    /// <summary>
    /// 接続中のカード。<see cref="IDisposable.Dispose"/> で切断する
    /// </summary>
    public sealed class PcscCard : IApduChannel, IDisposable
    {
        // 応答データ(最大256バイト) + ステータスワード(2バイト)
        private const int ResponseBufferSize = 258;

        // SCARD_IO_REQUEST(プロトコル番号 + 構造体サイズ)
        private const int PciSize = 8;

        private IntPtr _handle;
        private IntPtr _sendPci;

        internal PcscCard(IntPtr handle, uint protocol)
        {
            _handle = handle;
            _sendPci = Marshal.AllocHGlobal(PciSize);
            Marshal.WriteInt32(_sendPci, 0, (int)protocol);
            Marshal.WriteInt32(_sendPci, 4, PciSize);
        }

        public byte[] Transmit(byte[] command, out ushort statusWord)
        {
            var response = new byte[ResponseBufferSize];
            uint length = (uint)response.Length;

            PcscContext.Check(PcscNative.SCardTransmit(_handle, _sendPci, command, (uint)command.Length,
                IntPtr.Zero, response, ref length));

            if (length < 2) throw new NfcException("カードからの応答が短すぎます");

            statusWord = (ushort)((response[length - 2] << 8) | response[length - 1]);

            var data = new byte[length - 2];
            Array.Copy(response, data, data.Length);
            return data;
        }

        public void Dispose()
        {
            if (_handle != IntPtr.Zero)
            {
                PcscNative.SCardDisconnect(_handle, PcscNative.LeaveCard);
                _handle = IntPtr.Zero;
            }

            if (_sendPci != IntPtr.Zero)
            {
                Marshal.FreeHGlobal(_sendPci);
                _sendPci = IntPtr.Zero;
            }
        }
    }
}
