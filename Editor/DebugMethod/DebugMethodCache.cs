#if UNITY_EDITOR
using System.Reflection;
using UnityEngine;

namespace MyUtils.DebugMethod
{
    /// <summary>
    /// [TestInvokeMethod] がついたメソッドの情報を保持するキャッシュクラス
    /// </summary>
    public class DebugMethodCache
    {
        public MonoBehaviour Target;
        public MethodInfo Info;
        public string SaveKeyBase;
    }
}
#endif