using System;
using System.Linq;

namespace MyUtils.NfcUtils
{
    /// <summary>
    /// NFCリーダーを使って、タグにテキストを書き込む・タグのテキストを読み込む。
    /// 使い終わったら Dispose する(using を使うと自動で行われる)。
    ///
    /// 例:
    ///     using var reader = new NfcReader();
    ///     reader.WriteText("こんにちは", "pass");   // パスワードを付けると、暗号化して書き込む
    ///     string text = reader.ReadText("pass");
    ///
    /// うまくいかないときは、<see cref="NfcException"/> が発生する(メッセージを画面に表示できる)。
    /// 中身の仕組み(PC/SCやNDEF)は Core フォルダにあるが、使うだけなら読まなくてよい
    /// </summary>
    public class NfcReader : IDisposable
    {
        private readonly PcscContext _context;

        /// <summary>使っているリーダーの名前</summary>
        public string ReaderName { get; }

        /// <summary>
        /// リーダーに接続する。リーダーが見つからないときは NfcException
        /// </summary>
        public NfcReader()
        {
            _context = new PcscContext();

            try
            {
                ReaderName = _context.ListReaders().FirstOrDefault();
                if (ReaderName == null) throw new NfcException("リーダーが見つかりません。PCに接続してください");
            }
            catch
            {
                _context.Dispose();
                throw;
            }
        }

        /// <summary>
        /// リーダーにタグ(カード)が置かれているかどうか
        /// </summary>
        public bool HasCard()
        {
            using var card = _context.TryConnect(ReaderName);
            return card != null;
        }

        /// <summary>
        /// リーダーに置かれたタグにテキストを書き込む(前の内容は消える)
        /// </summary>
        /// <param name="text">書き込む文字</param>
        /// <param name="password">指定すると、この文字をパスワードで暗号化して書く(読むときに同じパスワードが必要)。空なら暗号化しない</param>
        public void WriteText(string text, string password = null)
        {
            if (!string.IsNullOrEmpty(password)) text = NfcCrypto.Encrypt(text, password);

            using var card = Connect();
            new Type2Tag(card).WriteNdefMessage(NdefText.Encode(text, "ja"));
        }

        /// <summary>
        /// リーダーに置かれたタグのテキストを読み込む。何も書かれていなければ空の文字列
        /// </summary>
        /// <param name="password">暗号化されて書かれている場合に必要なパスワード(違えば NfcException)</param>
        public string ReadText(string password = null)
        {
            using var card = Connect();
            var message = new Type2Tag(card).ReadNdefMessage();
            if (message.Length == 0) return string.Empty;

            if (!NdefText.TryDecode(message, out var text, out _))
            {
                throw new NfcException("テキスト以外のデータが書かれています");
            }

            if (!NfcCrypto.IsEncrypted(text)) return text;

            if (string.IsNullOrEmpty(password)) throw new NfcException("パスワードが必要です");
            return NfcCrypto.Decrypt(text, password);
        }

        public void Dispose()
        {
            _context.Dispose();
        }

        // リーダー上のタグに接続する。置かれていなければ NfcException
        private PcscCard Connect()
        {
            return _context.TryConnect(ReaderName)
                   ?? throw new NfcException("タグが置かれていません。リーダーにタグを置いてください");
        }
    }
}
