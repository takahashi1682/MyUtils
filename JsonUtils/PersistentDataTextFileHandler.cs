using System;
using System.IO;
using System.Text;
using UnityEngine;

namespace MyUtils.JsonUtils
{
    /// <summary>
    ///  Application.persistentDataPath 配下のテキストファイル専用の保存・読み込みユーティリティクラス。
    ///  指定したファイル名は自動的に persistentDataPath と結合されます。
    /// </summary>
    public static class PersistentDataTextFileHandler
    {
        private static readonly Encoding FileEncoding = Encoding.UTF8;

        /// <summary>
        /// アプリケーションの永続データパスとファイル名を結合し、フルパスを返します。
        /// </summary>
        /// <param name="fileName">ファイル名（例: "settings.json"）</param>
        /// <returns>永続データパスを含むフルパス</returns>
        public static string GetFilePath(string fileName)
            => Path.Combine(Application.persistentDataPath, fileName);

        /// <summary>
        /// 指定されたファイル名でテキストデータを保存します。
        /// </summary>
        /// <param name="fileName">保存するファイル名</param>
        /// <param name="textData">保存するテキストデータ</param>
        public static void SaveText(string fileName, string textData)
        {
            string filePath = GetFilePath(fileName);
            try
            {
                File.WriteAllText(filePath, textData, FileEncoding);
            }
            catch (Exception e)
            {
                Debug.LogError($"[TextFileHandler] Failed to save data to: {filePath}");
                Debug.LogException(e);
            }
        }

        /// <summary>
        /// 指定されたファイル名からテキストデータをロードします。
        /// </summary>
        /// <param name="fileName">ロードするファイル名</param>
        /// <returns>ロードされたテキストデータ。ファイルが存在しない場合はnullを返す。</returns>
        public static string LoadText(string fileName)
        {
            string filePath = GetFilePath(fileName);
            if (!File.Exists(filePath)) return null;

            try
            {
                return File.ReadAllText(filePath, FileEncoding);
            }
            catch (Exception e)
            {
                Debug.LogError($"[TextFileHandler] Failed to load text from: {filePath}");
                Debug.LogException(e);
                return null;
            }
        }
    }
}