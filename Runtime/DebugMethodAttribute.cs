#if UNITY_EDITOR
using System;

namespace MyUtils
{
    /// <summary>
    /// インスペクターやデバッグウィンドウからメソッドを直接実行できるようにする属性。
    /// </summary>
    [AttributeUsage(AttributeTargets.Method)]
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