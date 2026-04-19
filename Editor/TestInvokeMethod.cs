#if UNITY_EDITOR
using System;

namespace MyUtils
{
    [AttributeUsage(AttributeTargets.Method)]
    public class TestInvokeMethod : Attribute
    {
    }
}
#endif