#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace MyUtils.DebugMethod
{
    /// <summary>
    /// デバッグボタンと引数フィールドの描画・値の保存を担当するGUIクラス
    /// </summary>
    public static class DebugMethodGUI
    {
        private static readonly Dictionary<string, object[]> _parametersCache = new();

        public static void DrawMethodButton(DebugMethodCache cache)
        {
            var parameters = cache.Info.GetParameters();
            string baseKey = cache.SaveKeyBase;

            if (!_parametersCache.ContainsKey(baseKey))
            {
                _parametersCache[baseKey] = new object[parameters.Length];
            }

            // --- 修正箇所：属性（Attribute）からカスタムのボタン名を取得する ---
            var attr = (DebugMethodAttribute)Attribute.GetCustomAttribute(cache.Info, typeof(DebugMethodAttribute));
            string displayButtonName = (!string.IsNullOrEmpty(attr?.ButtonName)) ? attr.ButtonName : cache.Info.Name;

            using (new EditorGUILayout.VerticalScope(GUI.skin.box))
            {
                using (new EditorGUILayout.HorizontalScope())
                {
                    GUI.backgroundColor = new Color(0.2f, 0.6f, 0.9f);

                    // --- 修正箇所：cache.Info.Name だった部分を displayButtonName に変更 ---
                    if (GUILayout.Button($"▶ {displayButtonName}", GUILayout.Width(160), GUILayout.Height(22)))
                    {
                        cache.Info.Invoke(cache.Target, _parametersCache[baseKey]);
                    }

                    GUI.backgroundColor = Color.white;

                    if (parameters.Length == 0)
                    {
                        EditorGUILayout.LabelField("(No Parameters)", EditorStyles.miniLabel,
                            GUILayout.Height(22));
                    }
                }

                if (parameters.Length > 0)
                {
                    EditorGUI.indentLevel++;
                    for (int i = 0; i < parameters.Length; i++)
                    {
                        var p = parameters[i];
                        string itemKey = $"{baseKey}.param_{i}";
                        _parametersCache[baseKey][i] = DrawParameterField(p.Name, p.ParameterType,
                            _parametersCache[baseKey][i], itemKey);
                    }

                    EditorGUI.indentLevel--;
                }
            }
        }

        private static object DrawParameterField(string label, Type type, object value, string key)
        {
            if (value == null)
            {
                if (type == typeof(int)) value = EditorPrefs.GetInt(key, 0);
                else if (type == typeof(float)) value = EditorPrefs.GetFloat(key, 0f);
                else if (type == typeof(string)) value = EditorPrefs.GetString(key, "");
                else if (type == typeof(bool)) value = EditorPrefs.GetBool(key, false);
                else if (type.IsEnum) value = Enum.ToObject(type, EditorPrefs.GetInt(key, 0));
                else if (typeof(UnityEngine.Object).IsAssignableFrom(type))
                {
                    value = AssetDatabase.LoadAssetAtPath(EditorPrefs.GetString(key, ""), type);
                }
                else if (type.IsClass && type != typeof(string))
                {
                    value = Activator.CreateInstance(type);
                }
            }

            object newValue = value;

            if (type.IsClass && type != typeof(string) && !typeof(UnityEngine.Object).IsAssignableFrom(type))
            {
                EditorGUILayout.BeginVertical(GUI.skin.box);
                EditorGUILayout.LabelField($"[Class] {label}", EditorStyles.boldLabel);
                EditorGUI.indentLevel++;

                var fields = type.GetFields(BindingFlags.Public | BindingFlags.Instance);
                foreach (var field in fields)
                {
                    object nextValue = DrawParameterField(field.Name, field.FieldType, field.GetValue(value),
                        $"{key}.{field.Name}");
                    field.SetValue(value, nextValue);
                }

                EditorGUI.indentLevel--;
                EditorGUILayout.EndVertical();
                return value;
            }

            if (type == typeof(int))
            {
                newValue = EditorGUILayout.IntField(label, value is int iv ? iv : 0);
                if ((int)newValue != (value is int cv ? cv : 0)) EditorPrefs.SetInt(key, (int)newValue);
            }
            else if (type == typeof(float))
            {
                newValue = EditorGUILayout.FloatField(label, value is float fv ? fv : 0f);
                if (!Mathf.Approximately((float)newValue, value is float cf ? cf : 0f))
                    EditorPrefs.SetFloat(key, (float)newValue);
            }
            else if (type == typeof(string))
            {
                newValue = EditorGUILayout.TextField(label, (string)value);
                if ((string)newValue != (string)value) EditorPrefs.SetString(key, (string)newValue);
            }
            else if (type == typeof(bool))
            {
                newValue = EditorGUILayout.Toggle(label, value is bool bv && bv);
                if ((bool)newValue != (value is bool cb && cb)) EditorPrefs.SetBool(key, (bool)newValue);
            }
            else if (type.IsEnum)
            {
                newValue = EditorGUILayout.EnumPopup(label, (Enum)value);
                if (!newValue.Equals(value)) EditorPrefs.SetInt(key, Convert.ToInt32(newValue));
            }
            else if (typeof(UnityEngine.Object).IsAssignableFrom(type))
            {
                newValue = EditorGUILayout.ObjectField(label, (UnityEngine.Object)value, type, true);
                if ((UnityEngine.Object)newValue != (UnityEngine.Object)value)
                    EditorPrefs.SetString(key, AssetDatabase.GetAssetPath((UnityEngine.Object)newValue));
            }

            return newValue;
        }
    }
}
#endif