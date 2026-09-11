namespace MyUtils
{
    /// <summary>
    /// オーディオの再生・停止時の挙動
    /// </summary>
    public enum EAudioPlayMode
    {
        /// <summary>最初から再生する</summary>
        Play,

        /// <summary>再生中でなければ再生する</summary>
        PlayIfNotPlaying,

        /// <summary>フェードインしながら再生する</summary>
        FadeInPlay,

        /// <summary>クロスフェード再生する</summary>
        CrossFadePlay,
        
        /// <summary>停止する</summary>
        Stop,

        /// <summary>フェードアウトしてから停止する</summary>
        FadeOutStop,
        
        /// <summary>すべて停止する</summary>
        StopAll,

        /// <summary>すべてフェードアウトしてから停止する</summary>
        StopAllFadeOut,
    }
}