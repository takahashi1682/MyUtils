#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

namespace MyUtils
{
    [InitializeOnLoad]
    public class FolderPainter
    {
        // フォルダ名と色の対応ルールは FolderPainterSettings(ProjectSettings)に保存される。
        // Projectウィンドウでフォルダを右クリック > MyUtils > フォルダの色を設定... で変更できる。

        static FolderPainter()
        {
            // プロジェクトウィンドウの描画イベントに登録
            EditorApplication.projectWindowItemOnGUI += OnGUI;
        }

        private static void OnGUI(string guid, Rect rect)
        {
            string path = AssetDatabase.GUIDToAssetPath(guid);

            // フォルダ以外、またはメタデータがない場合はスルー
            if (string.IsNullOrEmpty(path) || !AssetDatabase.IsValidFolder(path)) return;

            // フォルダ名を取得
            string folderName = System.IO.Path.GetFileName(path);

            // 設定した名前に一致するかチェック
            if (FolderPainterSettings.instance.TryGetColor(folderName, out Color color))
            {
                // 背景全体を塗るためのRect
                // リスト表示の時は少し右にずらすとアイコンが見やすくなります
                var backgroundRect = new Rect(rect);

                // 色の透明度を調整（0.1〜0.2くらいが文字も見やすくておすすめ）
                color.a = 0.15f;

                // 背景を描画
                EditorGUI.DrawRect(backgroundRect, color);

                // 左端にアクセントの線を引く（よりプロっぽくなります）
                var lineRect = new Rect(rect.x, rect.y, 3, rect.height);
                color.a = 1.0f; // 線はくっきり
                EditorGUI.DrawRect(lineRect, color);
            }
        }
    }
}
#endif