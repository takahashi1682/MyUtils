#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

namespace MyUtils.Editor
{
    public class PlayModeOptimizer : EditorWindow
    {
        private Vector2 _scrollPos;

        [MenuItem("Window/Build/Play Mode Optimizer")]
        public static void ShowWindow() => GetWindow<PlayModeOptimizer>("PlayMode Optimizer");

        private void OnGUI()
        {
            _scrollPos = GUILayout.BeginScrollView(_scrollPos);

            GUILayout.Label("再生モード設定", EditorStyles.boldLabel);
            EditorGUILayout.Space();

            bool isNormalModeActive = !EditorSettings.enterPlayModeOptionsEnabled;
            DrawToggleRow("通常リロード設定を使用 (ON=通常 / OFF=高速化)", isNormalModeActive, () =>
            {
                EditorSettings.enterPlayModeOptionsEnabled = !EditorSettings.enterPlayModeOptionsEnabled;
            });

            EditorGUILayout.Space(10);
            GUILayout.Label("詳細設定 (ON = 毎回リセットする / OFF = リセットをスキップ)", EditorStyles.miniLabel);

            // Domain Reload
            bool isDomainReloadEnabled =
                (EditorSettings.enterPlayModeOptions & EnterPlayModeOptions.DisableDomainReload) == 0;
            DrawToggleRow("Domain Reload (staticのリセット)", isDomainReloadEnabled, () =>
            {
                ToggleOptionInverse(EnterPlayModeOptions.DisableDomainReload);
            });

            // Scene Reload
            bool isSceneReloadEnabled =
                (EditorSettings.enterPlayModeOptions & EnterPlayModeOptions.DisableSceneReload) == 0;
            DrawToggleRow("Scene Reload (シーンの再読み込み)", isSceneReloadEnabled, () =>
            {
                ToggleOptionInverse(EnterPlayModeOptions.DisableSceneReload);
            });

            EditorGUILayout.Space(20);
            HelpBoxInverse(isNormalModeActive, isDomainReloadEnabled, isSceneReloadEnabled);

            GUILayout.EndScrollView();
        }

        private static void DrawToggleRow(string label, bool isOn, System.Action toggleAction)
        {
            EditorGUILayout.BeginHorizontal(EditorStyles.helpBox);
            GUILayout.Label(label, GUILayout.ExpandWidth(true));

            // 色を逆転：通常(isOn)が緑、高速化(OFF)がグレー
            var defaultColor = GUI.backgroundColor;
            GUI.backgroundColor = isOn ? Color.green : Color.red;

            if (GUILayout.Button(isOn ? "ON" : "OFF", GUILayout.Width(80), GUILayout.Height(20)))
            {
                toggleAction.Invoke();
            }

            GUI.backgroundColor = defaultColor;
            EditorGUILayout.EndHorizontal();
        }

        /// <summary>
        /// 内部の「Disable（無効化）」フラグを逆方向に操作する
        /// </summary>
        private static void ToggleOptionInverse(EnterPlayModeOptions option)
        {
            if ((EditorSettings.enterPlayModeOptions & option) != 0)
                EditorSettings.enterPlayModeOptions &= ~option;
            else
                EditorSettings.enterPlayModeOptions |= option;
        }

        private static void HelpBoxInverse(bool isNormal, bool domain, bool scene)
        {
            if (isNormal)
            {
                EditorGUILayout.HelpBox("【通常モード】Unity標準の挙動です。再生ごとにリロードされます。", MessageType.Info);
            }
            else
            {
                string status = "【高速モード】";
                if (!domain) status += " [static保持]";
                if (!scene) status += " [Scene読込スキップ]";
                EditorGUILayout.HelpBox(status + " が適用されています。挙動の変化に注意してください。", MessageType.Warning);
            }
        }
    }
}
#endif