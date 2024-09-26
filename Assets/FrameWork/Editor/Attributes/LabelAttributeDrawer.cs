using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace FrameWork.Editor
{
#if UNITY_EDITOR
    [CustomPropertyDrawer(typeof(LabelAttribute))]
    public class LabelAttributeDrawer : PropertyDrawer
    {

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            LabelAttribute labelAttribute = (LabelAttribute)attribute;

            // �ν����Ϳ� ǥ��� �̸� �ٲٱ�
            label.text = labelAttribute.label;

            // ������Ƽ �׸���
            if (property.propertyType == SerializedPropertyType.String)
            {
                MultilineAttribute multiline = (MultilineAttribute)fieldInfo.GetCustomAttribute(typeof(MultilineAttribute));
                if (multiline != null)
                {
                    position.height = EditorGUIUtility.singleLineHeight * multiline.lines;
                }
            }

            EditorGUI.PropertyField(position, property, label, true);
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            MultilineAttribute multiline = (MultilineAttribute)fieldInfo.GetCustomAttribute(typeof(MultilineAttribute));
            if (multiline != null)
            {
                return EditorGUIUtility.singleLineHeight * multiline.lines;
            }

            return base.GetPropertyHeight(property, label);
        }
    }
#endif
}