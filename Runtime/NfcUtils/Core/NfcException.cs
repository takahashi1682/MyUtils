using System;

namespace MyUtils.NfcUtils
{
    /// <summary>
    /// NFCの読み書きで起きたエラー(画面にそのまま表示できる日本語メッセージを持つ)
    /// </summary>
    public class NfcException : Exception
    {
        public NfcException(string message) : base(message) { }
    }
}
