using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Editor
{
    [InitializeOnLoad]
    public class FolderPainter
    {
        // フォルダ名と色の対応ルール
        private static readonly Dictionary<string, Color> ColorSettings = new()
        {
            { "Prefabs", new Color(0.2f, 0.5f, 1.0f) }, // 青
            { "Materials", new Color(1.0f, 0.4f, 0.4f) }, // 赤
            { "Scenes", new Color(0.4f, 1.0f, 0.4f) }, // 緑
            { "Scripts", new Color(0.3f, 0.8f, 1.0f) }, // 水色
            { "Sprites", new Color(0.8f, 0.4f, 1.0f) }, // 紫
            { "Textures", new Color(1.0f, 0.8f, 0.2f) }, // オレンジ
        };

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
            if (ColorSettings.TryGetValue(folderName, out Color color))
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