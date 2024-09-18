using UnityEngine;
using UnityEditor;

namespace FrameWork.Editor
{
#if UNITY_EDITOR
    [CustomPropertyDrawer(typeof(ConditionAttribute))]
    public class ConditionAttributeDrawer : PropertyDrawer
    {

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            ConditionAttribute conditionAttribute = (ConditionAttribute)attribute;
            bool enabled = GetConditionAttributeResult(conditionAttribute, property);
            bool previouslyEnabled = GUI.enabled;
            GUI.enabled = enabled;
            if (!conditionAttribute.Hidden || enabled)
            {
                EditorGUI.PropertyField(position, property, label, true);
            }
            GUI.enabled = previouslyEnabled;
        }

        private bool GetConditionAttributeResult(ConditionAttribute condHAtt, SerializedProperty property)
        {
            bool enabled = true;
            string propertyPath = property.propertyPath;
            string conditionPath = propertyPath.Replace(property.name, condHAtt.ConditionBoolean);
            SerializedProperty sourcePropertyValue = property.serializedObject.FindProperty(conditionPath);

            if (sourcePropertyValue != null)
            {
                enabled = sourcePropertyValue.boolValue;
            }
            else
            {
                Debug.LogWarning("bool 타입이 아닙니다. " + condHAtt.ConditionBoolean);
            }

            return enabled;
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            ConditionAttribute conditionAttribute = (ConditionAttribute)attribute;
            bool enabled = GetConditionAttributeResult(conditionAttribute, property);

            if (!conditionAttribute.Hidden || enabled)
            {
                return EditorGUI.GetPropertyHeight(property, label);
            }
            else
            {
                return -EditorGUIUtility.standardVerticalSpacing;
            }
        }
    }
#endif
}