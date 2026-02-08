using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace MyUtils
{
    [CustomEditor(typeof(MonoBehaviour), true)]
    [CanEditMultipleObjects]
    public class InspectorButtonEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            // 通常のインスペクタを表示
            DrawDefaultInspector();

            // 対象の型から [InspectorButton] 属性がついたメソッドを探す
            var methods = target.GetType()
                .GetMethods(BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic)
                .Where(m => m.GetCustomAttribute<InspectorButtonAttribute>() != null);

            var methodInfos = methods as MethodInfo[] ?? methods.ToArray();
            if (!methodInfos.Any()) return;

            EditorGUILayout.Space(10);
            EditorGUILayout.LabelField("Debug Commands", EditorStyles.boldLabel);

            foreach (var method in methodInfos)
            {
                // メソッド名をボタン名にする（必要なら属性でカスタマイズ可能）
                if (GUILayout.Button(method.Name, GUILayout.Height(25)))
                {
                    foreach (var t in targets) // 複数選択時にも対応
                    {
                        method.Invoke(t, null);
                    }
                }
            }
        }
    }
}