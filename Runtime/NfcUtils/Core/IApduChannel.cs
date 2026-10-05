namespace MyUtils.NfcUtils
{
    /// <summary>
    /// カードにAPDUコマンドを送って応答を受け取る経路。
    /// 実機は <see cref="PcscCard"/>、動作確認用に差し替えることもできる
    /// </summary>
    public interface IApduChannel
    {
        /// <summary>
        /// コマンドを送り、応答データとステータスワード(正常は0x9000)を返す
        /// </summary>
        byte[] Transmit(byte[] command, out ushort statusWord);
    }
}
