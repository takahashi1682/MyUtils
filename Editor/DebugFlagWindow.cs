#if UNITY_EDITOR
using System;
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
    /// 一番上では、再生中の FPS の上限(10 / 30 / 60 / 144 / 上限なし)を選べる。
    /// </summary>
    public class DebugFlagWindow : EditorWindow
    {
        private Vector2 _scroll;

        [MenuItem("Window/MyUtils/Debug Flag Window")]
        private static void Open() => GetWindow<DebugFlagWindow>("Debug Flag");

        // FPS の選択肢。-1 は上限なし(Unity の初期値)
        private static readonly int[] FpsValues = { 10, 30, 60, 144, -1 };
        private static readonly string[] FpsLabels = { "10", "30", "60", "144", "上限なし" };

        // フラグと同じく、PC ごとに EditorPrefs へ保存する
        private static string FpsPrefsKey => $"{Application.productName}.MyUtils.DebugFlagFps";

        private static int Fps
        {
            get => EditorPrefs.GetInt(FpsPrefsKey, -1);
            set => EditorPrefs.SetInt(FpsPrefsKey, value);
        }

        [InitializeOnLoadMethod]
        private static void WatchPlayMode()
        {
            EditorApplication.playModeStateChanged += WarnOnPlay;
            EditorApplication.playModeStateChanged += ApplyFps;
        }

        // 再生を始めたら選んだ FPS にし、止めたら上限なしに戻す。
        // 上限なしを選んでいるときは、ゲーム側で設定した値を上書きしないように何もしない
        private static void ApplyFps(PlayModeStateChange state)
        {
            if (state == PlayModeStateChange.EnteredPlayMode && Fps > 0) Application.targetFrameRate = Fps;
            if (state == PlayModeStateChange.EnteredEditMode) Application.targetFrameRate = -1;
        }

        private static void WarnOnPlay(PlayModeStateChange state)
        {
            if (state != PlayModeStateChange.EnteredPlayMode) return;

            var onFlags = Enumerable.Range(1, DebugFlag.Count).Where(DebugFlag.IsOn).Select(ToLabel).ToArray();
            if (onFlags.Length > 0) Debug.LogWarning($"Debug Flag がオンです: {string.Join(", ", onFlags)}");
            if (Fps > 0) Debug.LogWarning($"Debug Flag Window で FPS の上限を {Fps} にしています");
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

            // FPS の上限。再生中に変えてもすぐ反映する
            GUILayout.Label("FPS 設定", EditorStyles.boldLabel);
            using (new EditorGUILayout.HorizontalScope())
            {
                GUILayout.Label("上限", GUILayout.Width(70));
                var index = Array.IndexOf(FpsValues, Fps);
                var newIndex = GUILayout.Toolbar(index, FpsLabels);
                if (newIndex != index)
                {
                    Fps = FpsValues[newIndex];
                    if (EditorApplication.isPlaying) Application.targetFrameRate = Fps;
                }
            }

            EditorGUILayout.Space();

            // 項目名。スクロールしても見えるように、スクロールの外に置く
            using (new EditorGUILayout.HorizontalScope())
            {
                GUILayout.Label("フラグ番号", EditorStyles.boldLabel, GUILayout.Width(70));
                GUILayout.Label("ON", EditorStyles.boldLabel, GUILayout.Width(50));
                GUILayout.Label("メモ", EditorStyles.boldLabel);
            }

            // ウィンドウが小さくても全部のフラグを見られるようにスクロールさせる
            using var scroll = new EditorGUILayout.ScrollViewScope(_scroll);
            _scroll = scroll.scrollPosition;

            for (var no = 1; no <= DebugFlag.Count; no++)
            {
                using (new EditorGUILayout.HorizontalScope())
                {
                    var isOn = DebugFlag.IsOn(no);

                    GUILayout.Label(no.ToString(), GUILayout.Width(70));

                    // オンは緑、オフは赤のボタンで表示する
                    GUI.backgroundColor = isOn ? Color.green : Color.red;
                    if (GUILayout.Button(isOn ? "ON" : "OFF", GUILayout.Width(50))) DebugFlag.Set(no, !isOn);
                    GUI.backgroundColor = Color.white;

                    var memo = EditorGUILayout.TextField(settings.GetMemo(no));
                    if (memo != settings.GetMemo(no)) settings.SetMemo(no, memo);
                }
            }
        }
    }
}
#endif
