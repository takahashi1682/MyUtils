#if UNITY_EDITOR
using System;

namespace MyUtils.DebugMethod
{
    /// <summary>
    /// インスペクターやデバッグウィンドウからメソッドを直接実行できるようにする属性。
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, Inherited = true, AllowMultiple = false)]
    public class DebugMethodAttribute : Attribute
    {
        public string ButtonName { get; }

        public DebugMethodAttribute(string buttonName = null)
        {
            ButtonName = buttonName;
        }
    }
}
#endif