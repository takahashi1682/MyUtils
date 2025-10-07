using UnityEngine;

namespace MyUtils
{
    /// <summary>
    ///  シーン遷移時に破棄されないようにする
    /// </summary>
    public class DontDestroyOnLoad : MonoBehaviour
    {
        private void Awake() => DontDestroyOnLoad(this);
    }
}