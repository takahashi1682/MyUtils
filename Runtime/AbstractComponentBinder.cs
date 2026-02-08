using UnityEngine;

namespace MyUtils
{
    public abstract class AbstractComponentBinder : MonoBehaviour
    {
        public abstract string ButtonName { get; }
        public abstract void Bind();
    }
}