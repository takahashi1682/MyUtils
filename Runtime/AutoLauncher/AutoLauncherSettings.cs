using UnityEngine;
using System.Collections.Generic;

namespace MyUtils.AutoLauncher
{
    [CreateAssetMenu(fileName = "AutoLauncherSettings", menuName = "MyUtils/AutoLauncherSettings")]
    public class AutoLauncherSettings : ScriptableObject
    {
        // 生成したいPrefabのリスト（Addressablesの参照）
        public List<GameObject> LaunchObjects = new();
    }
}