using UnityEngine;

namespace MyUtils
{
    /// <summary>
    /// SerializeFieldをInspector上で読み取り専用(編集不可)として表示する
    /// </summary>
    public class ReadOnlyAttribute : PropertyAttribute
    {
    }
}