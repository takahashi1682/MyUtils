#if UNITY_EDITOR
using System;
using System.Reflection;
using MyUtils.Parameter;
using UnityEditor;
using UnityEngine;

namespace MyUtils
{
    /// <summary>
    /// FlagsParameterBase (およびFlagsParameter&lt;T&gt;) 用のInspector描画。
    /// 通常のフィールド描画に加えて、対応するenumの各フラグをON/OFF表示する。
    /// </summary>
    [CustomPropertyDrawer(typeof(FlagsParameterBase), true)]
    public class FlagsParameterDrawer : PropertyDrawer
    {
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            var currentProp = property.FindPropertyRelative("_current");
            float height = EditorGUI.GetPropertyHeight(currentProp, true);

            var enumType = GetFlagEnumType(property);
            if (enumType != null)
            {
                height += EditorGUIUtility.standardVerticalSpacing + EditorGUIUtility.singleLineHeight;
                int count = Enum.GetNames(enumType).Length;
                height += count * (EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing);
            }

            return height;
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, label, property);

            // labelはGUIContent(参照型)のため、下のPropertyField内で書き換えられることがある。
            // ヘッダーラベル用に、書き換えの影響を受けないdisplayNameを先に控えておく。
            string displayName = property.displayName;

            var currentProp = property.FindPropertyRelative("_current");
            float currentHeight = EditorGUI.GetPropertyHeight(currentProp, true);
            var currentRect = new Rect(position.x, position.y, position.width, currentHeight);
            EditorGUI.PropertyField(currentRect, currentProp, label, true);

            float y = position.y + currentHeight + EditorGUIUtility.standardVerticalSpacing;

            var enumType = GetFlagEnumType(property, out ulong value);
            if (enumType != null)
            {
                var headerRect = new Rect(position.x, y, position.width, EditorGUIUtility.singleLineHeight);
                EditorGUI.LabelField(headerRect, displayName, EditorStyles.boldLabel);
                y += EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;

                var names = Enum.GetNames(enumType);
                var values = Enum.GetValues(enumType);

                using (new EditorGUI.DisabledScope(true))
                {
                    for (int i = 0; i < names.Length; i++)
                    {
                        int index = Convert.ToInt32(values.GetValue(i));
                        bool isSet = index is >= 0 and < 64 && (value & (1UL << index)) != 0;
                        var toggleRect = new Rect(position.x, y, position.width, EditorGUIUtility.singleLineHeight);
                        EditorGUI.Toggle(toggleRect, names[i], isSet);
                        y += EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;
                    }
                }
            }

            EditorGUI.EndProperty();
        }

        private static Type GetFlagEnumType(SerializedProperty property)
        {
            return GetFlagEnumType(property, out _);
        }

        private static Type GetFlagEnumType(SerializedProperty property, out ulong currentValue)
        {
            currentValue = 0UL;
            if (GetTargetObjectOfProperty(property) is not FlagsParameterBase target) return null;
            currentValue = target.Current.CurrentValue;
            var enumType = target.FlagEnumType;
            return enumType != null && enumType.IsEnum ? enumType : null;
        }

        // --- SerializedPropertyから実際のobjectを取得するためのreflectionヘルパー ---
        // PropertyDrawerはSerializedPropertyしか受け取れないが、FlagEnumTypeやCurrent.CurrentValueは
        // 実際のC#インスタンスからしか取得できないため、propertyPathを辿って対象オブジェクトを取得する。

        private static object GetTargetObjectOfProperty(SerializedProperty prop)
        {
            if (prop == null) return null;

            var path = prop.propertyPath.Replace(".Array.data[", "[");
            object obj = prop.serializedObject.targetObject;
            var elements = path.Split('.');

            foreach (var element in elements)
            {
                if (element.Contains("["))
                {
                    var elementName = element[..element.IndexOf("[", StringComparison.Ordinal)];
                    var indexString = element[element.IndexOf("[", StringComparison.Ordinal)..]
                        .Replace("[", "").Replace("]", "");
                    int index = Convert.ToInt32(indexString);
                    obj = GetValueAtIndex(GetFieldOrPropertyValue(obj, elementName), index);
                }
                else
                {
                    obj = GetFieldOrPropertyValue(obj, element);
                }
            }

            return obj;
        }

        private static object GetFieldOrPropertyValue(object source, string name)
        {
            if (source == null) return null;
            var type = source.GetType();
            const BindingFlags flags = BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance;

            while (type != null)
            {
                var field = type.GetField(name, flags);
                if (field != null) return field.GetValue(source);

                var prop = type.GetProperty(name, flags);
                if (prop != null) return prop.GetValue(source, null);

                type = type.BaseType;
            }

            return null;
        }

        private static object GetValueAtIndex(object source, int index)
        {
            if (source is not System.Collections.IEnumerable enumerable) return null;
            var enumerator = enumerable.GetEnumerator();
            for (int i = 0; i <= index; i++)
            {
                if (!enumerator.MoveNext()) return null;
            }

            return enumerator.Current;
        }
    }
}
#endif
