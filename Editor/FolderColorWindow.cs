#if UNITY_EDITOR
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace MyUtils
{
    /// <summary>
    /// Projectウィンドウでフォルダを右クリックして、FolderPainterの色を設定する小さなウィンドウ。
    /// 色はフォルダ名に対して設定され、同名のフォルダにも反映される。
    /// </summary>
    public class FolderColorWindow : EditorWindow
    {
        private const string MenuPath = "Assets/MyUtils/フォルダの色を設定...";

        private static readonly Color[] Presets =
        {
            new(1.0f, 0.35f, 0.35f), new(1.0f, 0.65f, 0.2f), new(0.9f, 0.9f, 0f), new(0.4f, 1.0f, 0.4f),
            new(0.2f, 0.85f, 0.85f), new(0.2f, 0.5f, 1.0f), new(0.8f, 0.4f, 1.0f), new(1.0f, 0.5f, 0.8f),
        };

        private string[] _folderNames;
        private Color _color;

        [MenuItem(MenuPath, true)]
        private static bool ValidateOpen() => GetSelectedFolderNames().Length > 0;

        [MenuItem(MenuPath, false, 2000)]
        private static void Open()
        {
            var window = CreateInstance<FolderColorWindow>();
            window.titleContent = new GUIContent("フォルダの色");
            window._folderNames = GetSelectedFolderNames();

            // 設定済みならその色から、未設定なら既定の色から始める
            window._color = FolderPainterSettings.instance.TryGetColor(window._folderNames[0], out var current)
                ? current
                : Presets[5];

            window.minSize = window.maxSize = new Vector2(300, 150);
            window.ShowUtility();
        }

        // 選択中のフォルダ名(重複なし)
        private static string[] GetSelectedFolderNames()
        {
            return Selection.assetGUIDs
                .Select(AssetDatabase.GUIDToAssetPath)
                .Where(path => !string.IsNullOrEmpty(path) && AssetDatabase.IsValidFolder(path))
                .Select(Path.GetFileName)
                .Distinct()
                .ToArray();
        }

        private void OnGUI()
        {
            if (_folderNames == null || _folderNames.Length == 0)
            {
                Close();
                return;
            }

            EditorGUILayout.LabelField(string.Join(", ", _folderNames), EditorStyles.boldLabel);
            EditorGUILayout.Space(2);

            EditorGUI.BeginChangeCheck();
            var color = EditorGUILayout.ColorField(new GUIContent("色"), _color, true, false, false);
            if (EditorGUI.EndChangeCheck()) SetColor(color);

            EditorGUILayout.Space(4);
            using (new EditorGUILayout.HorizontalScope())
            {
                foreach (var preset in Presets)
                {
                    var rect = GUILayoutUtility.GetRect(28, 22, GUILayout.ExpandWidth(false));
                    EditorGUI.DrawRect(rect, preset);
                    if (GUI.Button(rect, GUIContent.none, GUIStyle.none)) SetColor(preset);
                }
            }

            EditorGUILayout.Space(8);
            using (new EditorGUILayout.HorizontalScope())
            {
                if (GUILayout.Button("色を解除"))
                {
                    foreach (var folderName in _folderNames) FolderPainterSettings.instance.Remove(folderName);
                    Close();
                }

                if (GUILayout.Button("閉じる")) Close();
            }
        }

        private void SetColor(Color color)
        {
            _color = color;
            foreach (var folderName in _folderNames) FolderPainterSettings.instance.SetColor(folderName, color);
        }
    }
}
#endif
