using System.Collections.Generic;

namespace MyUtils.Parameter
{
    /// <summary>
    /// ゲーム内パラメーターの管理機能
    /// </summary>
    public static class ParameterManager
    {
        private static readonly Dictionary<string, object> Parameters = new();

        /// <summary>
        /// 指定したキーに対応する値を追加または更新します
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        public static void SetParameter(string key, object value) => Parameters[key] = value;

        /// <summary>
        /// 指定したキーに対応する値を取得します
        /// </summary>
        /// <param name="key"></param>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        public static T GetParameterOrDefault<T>(string key, T defaultValue)
            => (T)Parameters.GetValueOrDefault(key, defaultValue);

        /// <summary>
        /// 指定したキーに対応する値を取得します
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public static bool RemoveParameter(string key) => Parameters.Remove(key);

        /// <summary>
        /// 指定したキーに対応する値を取得します
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static bool RemoveParameter(string key, out object value) => Parameters.Remove(key, out value);

        /// <summary>
        /// パラメーターを全て削除します
        /// </summary>
        public static void ClearParameter() => Parameters.Clear();
    }
}