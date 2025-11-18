using System;
using System.IO;
using MyUtils.DataStore.Core;
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

        public static void SaveData(T outputData, DataStoreSetting setting)
            => SaveData(outputData, setting.FileName, setting.IsEncrypt, setting.IvFileName, setting.AesKey);

        /// <summary>
        /// データオブジェクトを暗号化・保存します。
        /// </summary>
        public static void SaveData(T outputData, string fileName, bool isEncrypt, string ivKeyFileName, string aesKey)
        {
            // JsonFileHandler にファイルパス生成を任せ、ローカル変数での再結合を避ける
            string filePath = PersistentDataTextFileHandler.GetFilePath(fileName);

            // 1. JSONにシリアライズ
            string json = JsonUtility.ToJson(outputData, false);
            string dataToSave = json; // デフォルトは生JSON

            if (isEncrypt)
            {
                byte[] iv = AESEncryption.GenerateRandomIV();
                dataToSave = AESEncryption.Encrypt(json, iv, aesKey);

                // 2. IVを専用ファイルに保存 (エラー処理はJsonFileHandler側で担当)
                string ivPath = GetIVFilePath(ivKeyFileName);
                PersistentDataTextFileHandler.SaveText(ivPath, AESEncryption.BytesToHex(iv));
            }

            // 3. データ本体を保存 (エラー処理はJsonFileHandler側で担当)
            PersistentDataTextFileHandler.SaveText(filePath, dataToSave);
        }

        public static bool LoadData(out T loadData, T defaultValue, DataStoreSetting setting)
            => LoadData(out loadData, defaultValue, setting.FileName, setting.IsEncrypt, setting.IvFileName,
                setting.AesKey);

        /// <summary>
        /// ファイルからデータをロードし、復号化・復元します。
        /// </summary>
        public static bool LoadData(out T loadData, T defaultValue, string fileName, bool isEncrypt, string ivFileName,
            string aesKey)
        {
            string filePath = PersistentDataTextFileHandler.GetFilePath(fileName);
            string dataToLoad = PersistentDataTextFileHandler.LoadText(filePath);

            if (dataToLoad == null)
            {
                Debug.Log($"[EncryptedJsonFileHandler] File not found. Returning new instance: {filePath}");
                loadData = defaultValue;
                return false;
            }

            try
            {
                string finalJson;

                if (isEncrypt)
                {
                    string ivPath = GetIVFilePath(ivFileName);
                    string ivHex = PersistentDataTextFileHandler.LoadText(ivPath);

                    if (ivHex == null)
                    {
                        Debug.LogError(
                            $"[EncryptedJsonFileHandler] IV key file not found for encrypted data: {ivPath}");
                        loadData = defaultValue;
                        return false;
                    }

                    byte[] iv = AESEncryption.HexToBytes(ivHex);
                    finalJson = AESEncryption.Decrypt(dataToLoad, iv, aesKey);
                }
                else
                {
                    finalJson = dataToLoad;
                }

                loadData = JsonUtility.FromJson<T>(finalJson);
                return true;
            }
            catch (Exception e)
            {
                Debug.LogError($"[EncryptedJsonFileHandler] Failed to load/decrypt/parse data from: {fileName}");
                Debug.LogException(e);
                loadData = defaultValue;
                return false;
            }
        }
    }
}