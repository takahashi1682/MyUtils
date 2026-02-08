#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

namespace MyUtils
{
    [CustomPropertyDrawer(typeof(SceneReference.SceneReference))]
    public class SceneReferenceDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            // プロパティの取得
            var sceneAssetProp = property.FindPropertyRelative("SceneAsset");
            var sceneNameProp = property.FindPropertyRelative("_sceneName");

            EditorGUI.BeginProperty(position, label, property);

            // 描画領域を分割
            var currentRect = EditorGUI.PrefixLabel(position, label);

            // SceneAssetの描画
            EditorGUI.BeginChangeCheck();
            EditorGUI.PropertyField(currentRect, sceneAssetProp, GUIContent.none);

            // SceneAssetが変更されたかどうかをチェック
            if (EditorGUI.EndChangeCheck())
            {
                sceneNameProp.stringValue =
                    sceneAssetProp.objectReferenceValue != null
                        ? sceneAssetProp.objectReferenceValue.name
                        : string.Empty;
            }

            EditorGUI.EndProperty();
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
            => EditorGUIUtility.singleLineHeight;
    }
}
#endif