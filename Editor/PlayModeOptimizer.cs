#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

namespace MyUtils
{
    /// <summary>
    /// Project Settings > Editor > Enter Play Mode Settings の「When entering Play Mode」をワンクリックで切り替える。
    /// Unity 6.6からはチェックボックス(Enter Play Mode Options)が廃止され、4択のドロップダウンに統一されたため、それに合わせた表記にしている。
    /// </summary>
    public class PlayModeOptimizer : EditorWindow
    {
        private static readonly (string Label, string Description, EnterPlayModeOptions Options)[] Modes =
        {
            ("Reload Domain and Scene", "再生ごとにstaticとシーンをリセットする(最も安全だが遅い)", EnterPlayModeOptions.None),
            ("Reload Scene only", "staticを保持し、シーンだけ再読み込みする", EnterPlayModeOptions.DisableDomainReload),
            ("Reload Domain only", "staticをリセットし、シーンの再読み込みをスキップする", EnterPlayModeOptions.DisableSceneReload),
            ("Do not reload Domain or Scene", "staticもシーンもリセットしない(最速。新規プロジェクトの既定)",
                EnterPlayModeOptions.DisableDomainReload | EnterPlayModeOptions.DisableSceneReload),
        };

        private Vector2 _scrollPos;

        [MenuItem("Window/MyUtils/Play Mode Optimizer")]
        public static void ShowWindow() => GetWindow<PlayModeOptimizer>("PlayMode Optimizer");

        // 旧仕様でEnter Play Mode Optionsが無効のままのプロジェクトは、毎回両方リロードされる扱いになる
        private static EnterPlayModeOptions CurrentOptions =>
            EditorSettings.enterPlayModeOptionsEnabled
                ? EditorSettings.enterPlayModeOptions & (EnterPlayModeOptions.DisableDomainReload | EnterPlayModeOptions.DisableSceneReload)
                : EnterPlayModeOptions.None;

        private void OnGUI()
        {
            _scrollPos = GUILayout.BeginScrollView(_scrollPos);

            GUILayout.Label("When entering Play Mode", EditorStyles.boldLabel);
            EditorGUILayout.Space();

            var current = CurrentOptions;
            foreach (var mode in Modes)
            {
                DrawModeRow(mode.Label, mode.Description, mode.Options == current, mode.Options);
            }

            EditorGUILayout.Space(20);
            DrawHelpBox(current);

            GUILayout.EndScrollView();
        }

        private static void DrawModeRow(string label, string description, bool isSelected, EnterPlayModeOptions options)
        {
            EditorGUILayout.BeginHorizontal(EditorStyles.helpBox);

            EditorGUILayout.BeginVertical(GUILayout.ExpandWidth(true));
            GUILayout.Label(label);
            GUILayout.Label(description, EditorStyles.miniLabel);
            EditorGUILayout.EndVertical();

            var defaultColor = GUI.backgroundColor;
            GUI.backgroundColor = isSelected ? Color.green : defaultColor;

            if (GUILayout.Button(isSelected ? "使用中" : "選択", GUILayout.Width(80), GUILayout.Height(20)) && !isSelected)
            {
                EditorSettings.enterPlayModeOptionsEnabled = true;
                EditorSettings.enterPlayModeOptions = options;
            }

            GUI.backgroundColor = defaultColor;
            EditorGUILayout.EndHorizontal();
        }

        private static void DrawHelpBox(EnterPlayModeOptions current)
        {
            if (current == EnterPlayModeOptions.None)
            {
                EditorGUILayout.HelpBox("【フルリロード】再生ごとにstaticとシーンがリセットされます。再生開始に時間がかかります。", MessageType.Info);
                return;
            }

            var status = "【高速モード】";
            if ((current & EnterPlayModeOptions.DisableDomainReload) != 0) status += " [static保持]";
            if ((current & EnterPlayModeOptions.DisableSceneReload) != 0) status += " [Scene読込スキップ]";
            EditorGUILayout.HelpBox(status + " が適用されています。前回の再生の状態が残る場合がある点に注意してください。", MessageType.Warning);
        }
    }
}
#endif
