using UnityEditor;
using UnityEngine;

namespace FrameWork.Editor
{
#if UNITY_EDITOR
    [CustomPropertyDrawer(typeof(PopupAttribute))]
    public class PopupAttributeDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            PopupAttribute popupAttribute = (PopupAttribute)attribute;

            // 타입에 따라 다르게 처리
            if (property.propertyType == SerializedPropertyType.String
                || property.propertyType == SerializedPropertyType.Integer
                || property.propertyType == SerializedPropertyType.Float)
            {
                // 현재 값의 인덱스 찾기
                int selectedIndex = Mathf.Max(0, System.Array.IndexOf(popupAttribute.options, GetPropertyValue(property)));

                // 팝업 그리기
                selectedIndex = EditorGUI.Popup(position, label.text, selectedIndex, GetOptionsAsStrings(popupAttribute.options));

                // 선택된 인덱스에 해당하는 값으로 설정
                SetPropertyValue(property, popupAttribute.options[selectedIndex]);
            }
            else
            {
                EditorGUI.LabelField(position, label.text, "지정되지 않은 팝업 형식");
            }
        }

        // SerializedProperty의 값을 가져오는 메서드
        private object GetPropertyValue(SerializedProperty property)
        {
            switch (property.propertyType)
            {
                case SerializedPropertyType.String:
                    return property.stringValue;
                case SerializedPropertyType.Integer:
                    return property.intValue;
                case SerializedPropertyType.Float:
                    return property.floatValue;
                default:
                    return null;
            }
        }

        // SerializedProperty에 값을 설정하는 메서드
        private void SetPropertyValue(SerializedProperty property, object value)
        {
            switch (property.propertyType)
            {
                case SerializedPropertyType.String:
                    property.stringValue = (string)value;
                    break;
                case SerializedPropertyType.Integer:
                    property.intValue = (int)value;
                    break;
                case SerializedPropertyType.Float:
                    property.floatValue = (float)value;
                    break;
            }
        }

        // options 배열을 string 배열로 변환하는 메서드
        private string[] GetOptionsAsStrings(object[] options)
        {
            string[] stringOptions = new string[options.Length];
            for (int i = 0; i < options.Length; i++)
            {
                stringOptions[i] = options[i].ToString();
            }
            return stringOptions;
        }
    }
#endif
}