using System;
using R3;

namespace MyUtils.NfcUtils
{
    /// <summary>
    /// NFCリーダーを見張って、できごとを通知する。
    /// <see cref="CheckCard"/> を呼ぶたびにタグが置かれているかを調べ、置かれた・外された・エラーが起きた、を Subscribe で受け取れる。
    /// リーダーが後から接続されても、自動でつなぎ直す。
    ///
    /// 例:
    ///     var watcher = new NfcWatcher();
    ///     watcher.OnCardRead.Subscribe(text => Debug.Log(text));
    ///     Observable.Interval(TimeSpan.FromSeconds(0.5)).Subscribe(_ => watcher.CheckCard());
    ///     watcher.Write("こんにちは");
    /// </summary>
    public class NfcWatcher : IDisposable
    {
        private readonly Subject<string> _onCardRead = new();
        private readonly Subject<Unit> _onNoCard = new();
        private readonly Subject<string> _onWritten = new();
        private readonly Subject<string> _onError = new();

        private NfcReader _reader;

        // タグが置かれているか(null は、まだ調べていない)
        private bool? _hasCard;

        /// <summary>タグが置かれたときに、タグに書かれている文字を通知する</summary>
        public Observable<string> OnCardRead => _onCardRead;

        /// <summary>タグが置かれていない状態になったときに通知する(外された、または最初から置かれていない)</summary>
        public Observable<Unit> OnNoCard => _onNoCard;

        /// <summary>タグに書き込めたときに、書き込んだ文字を通知する</summary>
        public Observable<string> OnWritten => _onWritten;

        /// <summary>失敗したときに、エラーメッセージを通知する</summary>
        public Observable<string> OnError => _onError;

        /// <summary>
        /// リーダーに置かれているタグに、文字を書き込む(結果は OnWritten / OnError で通知)
        /// </summary>
        public void Write(string text)
        {
            try
            {
                if (_reader == null) throw new NfcException("リーダーに接続されていません");

                _reader.WriteText(text);
                _onWritten.OnNext(text);
            }
            catch (Exception e)
            {
                _onError.OnNext(e.Message);
            }
        }

        public void Dispose()
        {
            _reader?.Dispose();
            _onCardRead.Dispose();
            _onNoCard.Dispose();
            _onWritten.Dispose();
            _onError.Dispose();
        }

        /// <summary>
        /// タグが置かれているかを調べ、変わったときだけ通知する(一定間隔で呼び出す)
        /// </summary>
        public void CheckCard()
        {
            try
            {
                // リーダーにまだ接続していなければ接続する
                _reader ??= new NfcReader();

                bool hasCard = _reader.HasCard();
                if (hasCard == _hasCard) return;

                _hasCard = hasCard;
                if (hasCard)
                {
                    _onCardRead.OnNext(_reader.ReadText());
                }
                else
                {
                    _onNoCard.OnNext(Unit.Default);
                }
            }
            catch (Exception e)
            {
                // 失敗したら、次の確認でリーダーに接続し直す
                _reader?.Dispose();
                _reader = null;
                _hasCard = null;
                _onError.OnNext(e.Message);
            }
        }
    }
}
