using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace Temporary.Core
{
    [CreateAssetMenu(menuName = "Templates/Unit/Skin", fileName = "Skin_", order = 0)]
    public class SkinTemplate : ScriptableObject
    {
        public int id;
        public string displayName;
        [Multiline(4)]
        public string description;
        public Sprite sprite_face;

        public AssetReference lobbyResource;
        public AssetReference battleResource;

        public void Draw(Rect rect)
        {
            SerializedObject serializedObject = new SerializedObject(this);

            var labelRect = new Rect(rect.x + 110, rect.y, 100, rect.height);
            var valueRect = new Rect(rect.x + 210, rect.y, rect.width - 210, rect.height);

            sprite_face = (Sprite)EditorGUI.ObjectField(new Rect(rect.x, rect.y, 100, 100), sprite_face, typeof(Sprite), false);

            GUI.Label(labelRect, "스킨 식별번호");
            id = EditorGUI.IntField(valueRect, id);

            labelRect.y += 20;
            valueRect.y += 20;
            GUI.Label(labelRect, "스킨 이름");
            displayName = EditorGUI.TextField(valueRect, displayName);

            labelRect.y += 20;
            valueRect.y += 20;
            valueRect.height = 60;
            GUI.Label(labelRect, "스킨 설명");
            description = EditorGUI.TextArea(valueRect, description);

            labelRect.x = rect.x;
            labelRect.y += 80;
            valueRect.y += 80;
            valueRect.height = rect.height;
            GUI.Label(labelRect, "로비 리소스");
            SerializedProperty lobbyProperty = serializedObject.FindProperty("lobbyResource");
            EditorGUI.PropertyField(valueRect, lobbyProperty, GUIContent.none);

            labelRect.y += 20;
            valueRect.y += 20;
            GUI.Label(labelRect, "전투 리소스");
            SerializedProperty battleProperty = serializedObject.FindProperty("battleResource");
            EditorGUI.PropertyField(valueRect, battleProperty, GUIContent.none);
        }


    }
}