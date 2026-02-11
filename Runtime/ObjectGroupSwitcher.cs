using R3;
using UnityEngine;

namespace MyUtils
{
    public class ObjectGroupSwitcher : MonoBehaviour
    {
        [SerializeField] public SerializableReactiveProperty<int> CurrentObjectIndex = new();
        public ObjectGroup[] Objects;

        private void Start()
        {
            CurrentObjectIndex.AddTo(this);
            CurrentObjectIndex.Subscribe(UpdateActiveObject);
        }

        private void UpdateActiveObject(int activeIndex)
        {
            for (int i = 0; i < Objects.Length; i++)
            {
                Objects[i].SetActiveAll(i == activeIndex);
            }
        }

        public void SetActiveObject(int index)
        {
            if (index < 0 || index >= Objects.Length) return;
            CurrentObjectIndex.Value = index;
        }

        public void NextObject()
        {
            if (Objects.Length == 0) return;
            CurrentObjectIndex.Value = (CurrentObjectIndex.Value + 1) % Objects.Length;
        }

        public void PreviousObject()
        {
            if (Objects.Length == 0) return;
            CurrentObjectIndex.Value = (CurrentObjectIndex.Value - 1 + Objects.Length) % Objects.Length;
        }
    }
}