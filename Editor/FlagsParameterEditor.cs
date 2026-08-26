#if UNITY_EDITOR
using System;
using MyUtils.Parameter;
using UnityEditor;
using UnityEngine;

namespace MyUtils
{
    [CustomEditor(typeof(AbstractFlagsParameterBase), true)]
    public class FlagsParameterEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();

            var flags = (AbstractFlagsParameterBase)target;
            var enumType = flags.FlagEnumType;
            if (enumType == null || !enumType.IsEnum) return;

            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Current Flags", EditorStyles.boldLabel);

            ulong value = flags.Current.CurrentValue;
            var names = Enum.GetNames(enumType);
            var values = Enum.GetValues(enumType);

            using (new EditorGUI.DisabledScope(true))
            {
                for (int i = 0; i < names.Length; i++)
                {
                    int index = Convert.ToInt32(values.GetValue(i));
                    bool isSet = index is >= 0 and < 64 && (value & (1UL << index)) != 0;
                    EditorGUILayout.Toggle(names[i], isSet);
                }
            }

            // Play中はフラグがフレーム毎に変わるので、常時再描画して追従させる
            if (Application.isPlaying) Repaint();
        }
    }
}
#endif