#if UNITY_EDITOR
using System.Linq;
using MyUtils.Misc;
using UnityEditor;
using UnityEngine;

namespace MyUtils
{
    /// <summary>
    /// DebugFlag の50個のフラグを切り替えるウィンドウ。再生中でも切り替えられる。
    /// フラグの横には、何に使っているかのメモを残せる。
    /// フラグがオンのまま再生を始めると、消し忘れに気づけるようにコンソールへ警告を出す。
    /// </summary>
    public class DebugFlagWindow : EditorWindow
    {
        private Vector2 _scroll;

        [MenuItem("Window/MyUtils/Debug Flag Window")]
        private static void Open() => GetWindow<DebugFlagWindow>("Debug Flag");

        [InitializeOnLoadMethod]
        private static void WatchPlayMode() => EditorApplication.playModeStateChanged += WarnOnPlay;

        private static void WarnOnPlay(PlayModeStateChange state)
        {
            if (state != PlayModeStateChange.EnteredPlayMode) return;

            var onFlags = Enumerable.Range(1, DebugFlag.Count).Where(DebugFlag.IsOn).Select(ToLabel).ToArray();
            if (onFlags.Length > 0) Debug.LogWarning($"Debug Flag がオンです: {string.Join(", ", onFlags)}");
        }

        // メモがあれば「Flag 1 (無敵)」、なければ「Flag 1」
        private static string ToLabel(int no)
        {
            var memo = DebugFlagSettings.instance.GetMemo(no);
            return string.IsNullOrEmpty(memo) ? $"Flag {no}" : $"Flag {no} ({memo})";
        }

        private void OnGUI()
        {
            var settings = DebugFlagSettings.instance;

            // ウィンドウが小さくても全部のフラグを見られるようにスクロールさせる
            using var scroll = new EditorGUILayout.ScrollViewScope(_scroll);
            _scroll = scroll.scrollPosition;

            for (var no = 1; no <= DebugFlag.Count; no++)
            {
                using (new EditorGUILayout.HorizontalScope())
                {
                    var isOn = DebugFlag.IsOn(no);

                    // オンは緑、オフは赤のボタンで表示する
                    GUI.backgroundColor = isOn ? Color.green : Color.red;
                    if (GUILayout.Button($"Flag {no}", GUILayout.Width(70))) DebugFlag.Set(no, !isOn);
                    GUI.backgroundColor = Color.white;

                    var memo = EditorGUILayout.TextField(settings.GetMemo(no));
                    if (memo != settings.GetMemo(no)) settings.SetMemo(no, memo);
                }
            }
        }
    }
}
#endif
