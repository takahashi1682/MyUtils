using UnityEngine;
using UnityEngine.UI;

namespace MyUtils
{
    [RequireComponent(typeof(Button))]
    public abstract class AbstractListItem<T> : MonoBehaviour
    {
        public abstract void Initialize(T data);
    }
}