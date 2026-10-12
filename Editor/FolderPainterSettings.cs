#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace MyUtils
{
    /// <summary>
    /// FolderPainterの色ルール(フォルダ名と色の対応)を保存する設定。
    /// ProjectSettings配下に保存するので、プロジェクトごとに設定でき、gitで共有できる。
    /// </summary>
    [FilePath("ProjectSettings/MyUtils.FolderPainter.asset", FilePathAttribute.Location.ProjectFolder)]
    public class FolderPainterSettings : ScriptableSingleton<FolderPainterSettings>
    {
        [Serializable]
        public class Rule
        {
            public string FolderName;
            public Color Color = Color.white;
        }

        [SerializeField] private List<Rule> _rules = new();
        [SerializeField] private bool _initialized;

        private Dictionary<string, Color> _lookup;

        public IReadOnlyList<Rule> Rules => _rules;

        private void OnEnable()
        {
            // 初回だけ、従来の固定ルールを初期値として入れる
            if (_initialized) return;

            _initialized = true;
            _rules.Add(new Rule { FolderName = "_Projects", Color = new Color(0.8f, 0.4f, 1.0f) });
            _rules.Add(new Rule { FolderName = "Projects", Color = new Color(0.8f, 0.4f, 1.0f) });
            _rules.Add(new Rule { FolderName = "Prefabs", Color = new Color(0.9f, 0.9f, 0f) });
            _rules.Add(new Rule { FolderName = "Scripts", Color = new Color(0.2f, 0.5f, 1.0f) });
            _rules.Add(new Rule { FolderName = "Sprites", Color = new Color(0.4f, 1.0f, 0.4f) });
        }

        public bool TryGetColor(string folderName, out Color color)
        {
            if (_lookup == null)
            {
                _lookup = new Dictionary<string, Color>();
                foreach (var rule in _rules)
                {
                    if (!string.IsNullOrEmpty(rule.FolderName)) _lookup[rule.FolderName] = rule.Color;
                }
            }

            return _lookup.TryGetValue(folderName, out color);
        }

        public void SetColor(string folderName, Color color)
        {
            var rule = _rules.Find(r => r.FolderName == folderName);
            if (rule == null)
            {
                rule = new Rule { FolderName = folderName };
                _rules.Add(rule);
            }

            rule.Color = color;
            Apply();
        }

        public void Remove(string folderName)
        {
            if (_rules.RemoveAll(r => r.FolderName == folderName) > 0) Apply();
        }

        private void Apply()
        {
            _lookup = null;
            Save(true);
            EditorApplication.RepaintProjectWindow();
        }
    }
}
#endif
