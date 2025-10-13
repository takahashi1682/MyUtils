using System;
using UnityEngine;

namespace MyUtils.DataStore.Core
{
    [Serializable]
    public class DataStoreSetting
    {
        public bool IsEncrypt = true;
        [Range(16, 16)] public string AesKey = "e5Cp29Pda8n5Qv13";
        public string FileName = "player_setting.json";
        public string IvFileName = "player_setting.iv";
    }
}