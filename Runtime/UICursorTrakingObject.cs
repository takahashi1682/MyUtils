using UnityEngine;
using UnityEngine.InputSystem;

namespace MyUtils
{
    public class UICursorTrakingObject : AbstractTargetBehaviour<RectTransform>
    {
        private void LateUpdate()
        {
            if (Target == null) return;

            Vector3 pos = Mouse.current.position.ReadValue();
            pos.z = Target.position.z;
            transform.position = pos;
        }
    }
}