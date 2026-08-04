using System;
using UnityEngine;
using UnityEngine.UI;

namespace MyUtils.AbstractList
{
    [RequireComponent(typeof(Button))]
    public abstract class AbstractListItem<T> : MonoBehaviour
    {
        public int Index { get; private set; }
        public T Data { get; private set; }

        public virtual void Initialize(int index, T data)
        {
            if (index < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(index), index, "Index must be 0 or greater.");
            }

            Index = index;
            Data = data;
            Bind(index, data);
        }

        protected abstract void Bind(int index, T data);
    }
}