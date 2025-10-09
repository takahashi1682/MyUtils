using System;
using System.IO;
using UnityEngine;

namespace MyUtils.JsonUtils
{
    /// <summary>
    /// ジェネリックなデータ型を JSON 形式でシリアライズし、暗号化・復号化およびファイルI/Oを処理します。
    /// </summary>
    public static class EncryptedJsonFileHandler<T> where T : new()
    {
        /// <summary>
        /// IV (Initialization Vector) ファイルのフルパスを返します。
        /// </summary>
        public static string GetIVFilePath(string fileName)
            => Path.Combine(Application.persistentDataPath, fileName);

        /// <summary>
        /// データオブジェクトを暗号化・保存します。
        /// </summary>
        public static void SaveData(string fileName, T outputData, bool isEncrypt, string ivKeyFileName, string aesKey)
        {
            // JsonFileHandler にファイルパス生成を任せ、ローカル変数での再結合を避ける
            string filePath = TextFileHandler.GetFilePath(fileName);

            // 1. JSONにシリアライズ
            string json = JsonUtility.ToJson(outputData, false);
            string dataToSave = json; // デフォルトは生JSON

            if (isEncrypt)
            {
                byte[] iv = AESEncryption.GenerateRandomIV();
                dataToSave = AESEncryption.Encrypt(json, iv, aesKey);

                // 2. IVを専用ファイルに保存 (エラー処理はJsonFileHandler側で担当)
                string ivPath = GetIVFilePath(ivKeyFileName);
                TextFileHandler.SaveText(ivPath, AESEncryption.BytesToHex(iv));
            }

            // 3. データ本体を保存 (エラー処理はJsonFileHandler側で担当)
            TextFileHandler.SaveText(filePath, dataToSave);
        }

        /// <summary>
        /// ファイルからデータをロードし、復号化・復元します。
        /// </summary>
        public static T LoadData(string fileName, bool isEncrypt, string ivFileName, string aesKey)
        {
            string filePath = TextFileHandler.GetFilePath(fileName);
            string dataToLoad = TextFileHandler.LoadText(filePath);

            if (dataToLoad == null)
            {
                Debug.Log($"[EncryptedJsonFileHandler] File not found. Returning new instance: {filePath}");
                return new T();
            }

            try
            {
                string finalJson;

                if (isEncrypt)
                {
                    string ivPath = GetIVFilePath(ivFileName);
                    string ivHex = TextFileHandler.LoadText(ivPath);

                    if (ivHex == null)
                    {
                        Debug.LogError(
                            $"[EncryptedJsonFileHandler] IV key file not found for encrypted data: {ivPath}");
                        return new T();
                    }

                    byte[] iv = AESEncryption.HexToBytes(ivHex);
                    finalJson = AESEncryption.Decrypt(dataToLoad, iv, aesKey);
                }
                else
                {
                    finalJson = dataToLoad;
                }

                return JsonUtility.FromJson<T>(finalJson);
            }
            catch (Exception e)
            {
                Debug.LogError($"[EncryptedJsonFileHandler] Failed to load/decrypt/parse data from: {fileName}");
                Debug.LogException(e);
                return new T();
            }
        }
    }
}