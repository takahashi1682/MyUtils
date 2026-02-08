using UnityEditor;
using UnityEngine;

namespace MyUtils
{
    [CustomEditor(typeof(AbstractComponentBinder), true)]
    public class AbstractComponentBinderEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            var binder = (AbstractComponentBinder)target;
            if (GUILayout.Button(binder.ButtonName))
            {
                binder.Bind();
            }

            EditorGUILayout.Space(10);
            DrawDefaultInspector();
        }
    }
}