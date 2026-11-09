#if UNITY_EDITOR
using System;
using MyUtils.Misc;
using UnityEditor;
using UnityEngine;

namespace MyUtils
{
    /// <summary>
    /// DebugFlag のメモを保存する設定。
    /// ProjectSettings配下に保存するので、プロジェクトごとに設定でき、gitで共有できる。
    /// </summary>
    [FilePath("ProjectSettings/MyUtils.DebugFlag.asset", FilePathAttribute.Location.ProjectFolder)]
    public class DebugFlagSettings : ScriptableSingleton<DebugFlagSettings>
    {
        [SerializeField] private string[] _memos = new string[DebugFlag.Count];

        // 保存済みのメモがフラグの数より少ないとき(フラグを増やす前のファイルなど)は、足りない分を空で足す
        private string[] Memos
        {
            get
            {
                if (_memos == null || _memos.Length < DebugFlag.Count) Array.Resize(ref _memos, DebugFlag.Count);
                return _memos;
            }
        }

        public string GetMemo(int no) => Memos[no - 1];

        public void SetMemo(int no, string memo)
        {
            Memos[no - 1] = memo;
            Save(true);
        }
    }
}
#endif
