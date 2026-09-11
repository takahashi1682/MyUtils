using UnityEngine;

namespace MyUtils.ObjectGroup
{
    /// <summary>
    /// Objects内のObjectGroupを、Indexで指定した1つだけ有効化するクラス。
    /// Indexの変更はSetActiveObject/NextObject/PreviousObject経由でのみ行う。
    /// </summary>
    public class ObjectGroupSwitcher : MonoBehaviour
    {
        public ObjectGroup[] Objects;

        [SerializeField, ReadOnly] private int _index;
        public int Index => _index;

        private void Awake()
        {
            UpdateActiveObject(_index);
        }

        private void UpdateActiveObject(int activeIndex)
        {
            for (int i = 0; i < Objects.Length; i++)
            {
                Objects[i].SetAllActive(i == activeIndex);
            }
        }

        public void SetActiveObject(int index)
        {
            if (index < 0 || index >= Objects.Length) return;
            _index = index;
            UpdateActiveObject(_index);
        }

        public void NextObject()
        {
            if (Objects.Length == 0) return;
            _index = (_index + 1) % Objects.Length;
            UpdateActiveObject(_index);
        }

        public void PreviousObject()
        {
            if (Objects.Length == 0) return;
            _index = (_index - 1 + Objects.Length) % Objects.Length;
            UpdateActiveObject(_index);
        }
    }
}