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

            // 인스펙터에 표기될 이름 바꾸기
            label.text = labelAttribute.label;

            // 프로퍼티 그리기
            EditorGUI.PropertyField(position, property, label, true);
        }
    }
#endif
}