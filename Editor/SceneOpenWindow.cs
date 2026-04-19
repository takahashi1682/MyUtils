#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

namespace MyUtils
{
    public class SceneOpenWindow : EditorWindow
    {
        [MenuItem("Window/MyUtils/Scene Open Window")]
        public static void OpenWindow() => GetWindow<SceneOpenWindow>("Scene Open Window");

        private const string SaveKEYFilter = "MyUtils.SceneOpenWindow.FilterText";

        private string[] _allScenePaths; 
        private List<string> _filteredScenePaths = new(); // 高速化：表示用のリストをキャッシュ
        private string _filterText = "Assets/"; 
        private GUIStyle _buttonStyle;
        private Vector2 _scrollPosition;
        private GUIContent _sceneIcon;

        private void OnEnable()
        {
            _filterText = EditorPrefs.GetString(SaveKEYFilter, "Assets/");
            _sceneIcon = EditorGUIUtility.IconContent("SceneAsset Icon");
            RefreshSceneList();
        }

        private void OnGUI()
        {
            // 軽量化：スタイル作成を1回だけに制限
            if (_buttonStyle == null)
            {
                _buttonStyle = new GUIStyle(EditorStyles.miniButton)
                {
                    fixedHeight = 20f,
                    alignment = TextAnchor.MiddleLeft,
                    imagePosition = ImagePosition.ImageLeft
                };
            }

            EditorGUILayout.LabelField("Filter by path:", EditorStyles.boldLabel);
            
            EditorGUI.BeginChangeCheck();
            _filterText = EditorGUILayout.TextField(_filterText);
            if (EditorGUI.EndChangeCheck())
            {
                EditorPrefs.SetString(SaveKEYFilter, _filterText);
                UpdateFilteredList(); // 文字が変わった時だけリストを計算
            }

            EditorGUILayout.Space();
            if (GUILayout.Button("Refresh")) RefreshSceneList();
            EditorGUILayout.Space();

            _scrollPosition = EditorGUILayout.BeginScrollView(_scrollPosition);

            // 高速化：計算済みのリストを表示するだけ
            foreach (string scenePath in _filteredScenePaths)
            {
                string label = System.IO.Path.GetFileNameWithoutExtension(scenePath);
                var content = new GUIContent(label, _sceneIcon.image);

                if (GUILayout.Button(content, _buttonStyle))
                {
                    if (EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo())
                    {
                        EditorSceneManager.OpenScene(scenePath);
                    }
                }
            }

            EditorGUILayout.EndScrollView();
        }

        private void RefreshSceneList()
        {
            string[] guids = AssetDatabase.FindAssets("t:Scene");
            _allScenePaths = guids.Select(AssetDatabase.GUIDToAssetPath).ToArray();
            UpdateFilteredList(); // 全リスト更新時にもフィルターをかける
            Repaint();
        }

        // 高速化の要：フィルタリングを別メソッドに分離
        private void UpdateFilteredList()
        {
            if (_allScenePaths == null) return;

            if (string.IsNullOrEmpty(_filterText))
            {
                _filteredScenePaths = _allScenePaths.ToList();
            }
            else
            {
                string lowerFilter = _filterText.ToLower();
                _filteredScenePaths = _allScenePaths
                    .Where(path => path.ToLower().Contains(lowerFilter))
                    .ToList();
            }
        }
    }
}
#endif