namespace MyUtils.TalkUtils
{
    /// <summary>
    /// 会話中に次の会話の呼び出しが来たときの動作(R3の AwaitOperation のうち、会話に使えるもの)
    /// </summary>
    public enum ETalkOverlapMode
    {
        /// <summary>会話中の呼び出しは無視する</summary>
        Drop,

        /// <summary>呼び出し順に待機し、今の会話が終わってから次の会話を始める</summary>
        Sequential,

        /// <summary>今の会話を中断して、新しい会話を始める</summary>
        Switch
    }
}
