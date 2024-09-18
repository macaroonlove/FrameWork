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
            EditorGUI.PropertyField(position, property, label, true);
        }
    }
#endif
}