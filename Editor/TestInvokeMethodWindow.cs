#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace MyUtils
{
    public class TestInvokeMethodWindow : EditorWindow
    {
        // 高速化のための情報をまとめるクラス
        private class MethodCache
        {
            public MonoBehaviour Target;
            public MethodInfo Info;
            public string SaveKeyBase;
        }

        private Vector2 _scrollPos;
        private readonly Dictionary<string, object[]> _parametersCache = new();
        private readonly List<MethodCache> _foundMethods = new(); // シーン内の対象を記憶するリスト

        [MenuItem("Window/MyUtils/Test Invoke Method Window")]
        public static void ShowWindow() => GetWindow<TestInvokeMethodWindow>("Test Invoke Method Window");

        private void OnEnable()
        {
            RefreshMethodList();
            // シーンが切り替わったときに自動でリストを更新する設定
            UnityEditor.SceneManagement.EditorSceneManager.sceneOpened += (_, _) => RefreshMethodList();
        }

        private void OnFocus() => RefreshMethodList(); // ウィンドウを触った時に最新の状態にする

        private void OnGUI()
        {
            // 1. ヘッダー部分
            using (new EditorGUILayout.HorizontalScope(EditorStyles.toolbar))
            {
                if (GUILayout.Button("🔄 Refresh List", EditorStyles.toolbarButton))
                {
                    RefreshMethodList();
                }

                GUILayout.FlexibleSpace();
            }

            _scrollPos = EditorGUILayout.BeginScrollView(_scrollPos);

            if (_foundMethods.Count == 0)
            {
                EditorGUILayout.HelpBox("[TestInvokeMethod] 属性がついたメソッドをシーン内から見つけられませんでした。", MessageType.Info);
            }

            // 2. キャッシュされたリストをループして描画
            MonoBehaviour lastMono = null;
            foreach (var cache in _foundMethods)
            {
                if (cache.Target == null) continue;

                // 同じコンポーネントなら一つの枠にまとめる
                if (lastMono != cache.Target)
                {
                    if (lastMono != null) EditorGUILayout.EndVertical(); // 前の枠を閉じる

                    EditorGUILayout.Space(10);
                    EditorGUILayout.BeginVertical(GUI.skin.box);

                    using (new EditorGUILayout.HorizontalScope())
                    {
                        // オブジェクト選択ボタン
                        if (GUILayout.Button($"{cache.Target.gameObject.name} ({cache.Target.GetType().Name})",
                                EditorStyles.boldLabel))
                        {
                            Selection.activeGameObject = cache.Target.gameObject;
                        }

                        // 🔍 フォーカスボタン（SceneViewのカメラを飛ばす）
                        if (GUILayout.Button(new GUIContent("🔍", "Scene内のこのオブジェクトへ移動"), GUILayout.Width(30)))
                        {
                            var go = cache.Target.gameObject;
                            Selection.activeGameObject = go;
                            EditorGUIUtility.PingObject(go);
                            SceneView.FrameLastActiveSceneView();
                        }
                    }

                    lastMono = cache.Target;
                }

                DrawMethodButton(cache.Target, cache.Info, cache.SaveKeyBase);
            }

            if (lastMono != null) EditorGUILayout.EndVertical();

            EditorGUILayout.EndScrollView();
        }

        // --- 重い処理（検索）を1回だけやるメソッド ---
        private void RefreshMethodList()
        {
            _foundMethods.Clear();
            _parametersCache.Clear();

            // 非表示のオブジェクトも含めて全て取得
            var allComponents = FindObjectsByType<MonoBehaviour>(
                FindObjectsInactive.Include);

            foreach (var mono in allComponents)
            {
                if (mono == null) continue;

                var methods = mono.GetType()
                    .GetMethods(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
                    .Where(m => m.GetCustomAttributes(typeof(TestInvokeMethod), true).Length > 0);

                foreach (var method in methods)
                {
                    _foundMethods.Add(new MethodCache
                    {
                        Target = mono,
                        Info = method,
                        // 保存用のキー：シーン名も含めるとより安全
                        SaveKeyBase = $"TestInvoke.{mono.gameObject.scene.name}.{mono.gameObject.name}.{method.Name}"
                    });
                }
            }

            Repaint();
        }

        private void DrawMethodButton(MonoBehaviour target, MethodInfo method, string baseKey)
        {
            var parameters = method.GetParameters();

            if (!_parametersCache.ContainsKey(baseKey))
            {
                _parametersCache[baseKey] = new object[parameters.Length];
            }

            using (new EditorGUILayout.HorizontalScope())
            {
                // 実行ボタン
                if (GUILayout.Button($"▶ {method.Name}", GUILayout.Width(150)))
                {
                    method.Invoke(target, _parametersCache[baseKey]);
                }

                // 引数入力エリア
                using (new EditorGUILayout.VerticalScope())
                {
                    for (int i = 0; i < parameters.Length; i++)
                    {
                        var p = parameters[i];
                        string itemKey = $"{baseKey}.param_{i}";
                        _parametersCache[baseKey][i] = DrawParameterField(p.Name, p.ParameterType,
                            _parametersCache[baseKey][i], itemKey);
                    }
                }
            }
        }

        private static object DrawParameterField(string label, Type type, object value, string key)
        {
            // 1. 初回表示時の復元処理（value が null の場合のみ実行）
            if (value == null)
            {
                if (type == typeof(int)) value = EditorPrefs.GetInt(key, 0);
                else if (type == typeof(float)) value = EditorPrefs.GetFloat(key, 0f);
                else if (type == typeof(string)) value = EditorPrefs.GetString(key, "");
                else if (type == typeof(bool)) value = EditorPrefs.GetBool(key, false);
                else if (type.IsEnum) value = Enum.ToObject(type, EditorPrefs.GetInt(key, 0));
                else if (typeof(UnityEngine.Object).IsAssignableFrom(type))
                {
                    string path = EditorPrefs.GetString(key, "");
                    value = AssetDatabase.LoadAssetAtPath(path, type);
                }
                else if (type.IsClass && type != typeof(string))
                {
                    value = Activator.CreateInstance(type);
                    // クラスの場合は中身を再帰的に復元するために、ここではインスタンス化だけ行う
                }
            }

            object newValue = value;

            // 2. クラス（QueInfoDataなど）の再帰描画
            if (type.IsClass && type != typeof(string) && !typeof(UnityEngine.Object).IsAssignableFrom(type))
            {
                EditorGUILayout.BeginVertical(GUI.skin.box);
                EditorGUILayout.LabelField($"[Class] {label}", EditorStyles.boldLabel);
                EditorGUI.indentLevel++;

                var fields = type.GetFields(BindingFlags.Public | BindingFlags.Instance);
                foreach (var field in fields)
                {
                    object fieldValue = field.GetValue(value);
                    // キーを "親キー.フィールド名" にして階層化
                    object nextValue =
                        DrawParameterField(field.Name, field.FieldType, fieldValue, $"{key}.{field.Name}");
                    field.SetValue(value, nextValue);
                }

                EditorGUI.indentLevel--;
                EditorGUILayout.EndVertical();
                return value;
            }

            // 3. 基本型の描画と保存
            if (type == typeof(int))
            {
                if (value != null)
                {
                    newValue = EditorGUILayout.IntField(label, (int)value);
                    if ((int)newValue != (int)value) EditorPrefs.SetInt(key, (int)newValue);
                }
            }
            else if (type == typeof(float))
            {
                if (value != null)
                {
                    newValue = EditorGUILayout.FloatField(label, (float)value);
                    if (!Mathf.Approximately((float)newValue, (float)value)) EditorPrefs.SetFloat(key, (float)newValue);
                }
            }
            else if (type == typeof(string))
            {
                newValue = EditorGUILayout.TextField(label, (string)value);
                if ((string)newValue != (string)value) EditorPrefs.SetString(key, (string)newValue);
            }
            else if (type == typeof(bool))
            {
                newValue = value != null && EditorGUILayout.Toggle(label, (bool)value);
                if (value != null && (bool)newValue != (bool)value) EditorPrefs.SetBool(key, (bool)newValue);
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