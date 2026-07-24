using UnityEditor;
using UnityEngine;

namespace MyUtils.Separator
{
    public static class CreateSeparatorObject
    {
        public static void Execute(string name)
        {
            var go = new GameObject($"───── {name} ─────");

            if (Selection.activeTransform != null)
            {
                go.transform.SetParent(Selection.activeTransform);
                go.layer = Selection.activeTransform.gameObject.layer;
            }

            go.transform.localPosition = Vector3.zero;
            go.transform.localRotation = Quaternion.identity;
            go.transform.localScale = Vector3.one;

            Undo.RegisterCreatedObjectUndo(go, $"Create {name} Separator");
            Selection.activeGameObject = go;
        }
    }
}