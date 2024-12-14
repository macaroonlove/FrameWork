using FrameWork.Editor;
using System.Collections.Generic;
using UnityEngine;

namespace Temporary.Core
{
    [CreateAssetMenu(menuName = "Templates/GlobalStatus", fileName = "GlobalStatus", order = 0)]
    public class GlobalStatusTemplate : ScriptableObject
    {
        [HideInInspector, SerializeField] private Sprite _sprite;

        [HideInInspector, SerializeField] private string _displayName;
        [HideInInspector, SerializeField] private string _description;

        [HideInInspector]
        public List<Effect> effects = new List<Effect>();

        //[Header("FX")]
        //[Tooltip("시작 시 적용되는 효과")]
        //public List<FX> EnableFX = new List<FX>();
        //[Tooltip("매턴 적용되는 효과")]
        //public List<FX> UpdateFX = new List<FX>();
        //[Tooltip("해제 시 적용되는 효과")]
        //public List<FX> DisableFX = new List<FX>();

        #region 프로퍼티
        public Sprite sprite => _sprite;
        public string displayName => _displayName;
        public string description => _description;
        #endregion

        #region 값 변경 메서드
        public void SetDisplayName(string name)
        {
            _displayName = name;
        }
        #endregion
    }
}

#if UNITY_EDITOR
namespace Temporary.Editor
{
    using System;
    using Temporary.Core;
    using UnityEditor;
    using UnityEditorInternal;

    [CustomEditor(typeof(GlobalStatusTemplate)), CanEditMultipleObjects]
    public class GlobalStatusTemplateEditor : EffectEditor
    {
        private GlobalStatusTemplate _target;

        private SerializedProperty _sprite;
        private SerializedProperty _displayName;
        private SerializedProperty _description;

        private ReorderableList _effectsList;
        private Effect _currentEffect;

        private void OnEnable()
        {
            _target = target as GlobalStatusTemplate;

            _sprite = serializedObject.FindProperty("_sprite");
            _displayName = serializedObject.FindProperty("_displayName");
            _description = serializedObject.FindProperty("_description");

            CreateEffectList();
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            GUILayout.BeginHorizontal();

            _sprite.objectReferenceValue = EditorGUILayout.ObjectField(_sprite.objectReferenceValue, typeof(Sprite), false, GUILayout.Width(96), GUILayout.Height(96));

            EditorGUILayout.BeginVertical();

            GUILayout.BeginHorizontal();
            GUILayout.Label("전역 상태 이름", GUILayout.Width(80));
            EditorGUILayout.PropertyField(_displayName, GUIContent.none);
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.Label("전역 상태 설명", GUILayout.Width(80));
            _description.stringValue = EditorGUILayout.TextArea(_description.stringValue, GUILayout.Height(74));
            GUILayout.EndHorizontal();

            GUILayout.EndVertical();

            GUILayout.EndHorizontal();

            GUILayout.Space(20);

            _effectsList?.DoLayoutList();

            serializedObject.ApplyModifiedProperties();

            if (GUI.changed)
            {
                EditorUtility.SetDirty(this);
            }
        }

        #region EffectList
        private void InitMenu_Effects()
        {
            var menu = new GenericMenu();

            menu.AddItem(new GUIContent("골드 추가 획득"), false, CreateEffectCallback, typeof(GoldGainAdditionalDataEffect));
            menu.AddItem(new GUIContent("골드 획득량 증감"), false, CreateEffectCallback, typeof(GoldGainIncreaseDataEffect));
            menu.AddItem(new GUIContent("골드 획득량 상승·하락"), false, CreateEffectCallback, typeof(GoldGainMultiplierDataEffect));

            menu.AddItem(new GUIContent("코스트 증가 속도 차감"), false, CreateEffectCallback, typeof(CostRecoveryTimeAdditionalDataEffect));
            menu.AddItem(new GUIContent("코스트 증가 속도 증감"), false, CreateEffectCallback, typeof(CostRecoveryTimeIncreaseDataEffect));
            menu.AddItem(new GUIContent("코스트 증가 속도 상승·하락"), false, CreateEffectCallback, typeof(CostRecoveryTimeMultiplierDataEffect));

            menu.ShowAsContext();
        }

        private void CreateEffectList()
        {
            _effectsList = SetupReorderableList("Global Status Effects", _target.effects,
                (rect, x) =>
                {
                },
                (x) =>
                {
                    _currentEffect = x;
                },
                () =>
                {
                    InitMenu_Effects();
                },
                (x) =>
                {
                    DestroyImmediate(_currentEffect, true);
                    _currentEffect = null;
                    EditorUtility.SetDirty(target);
                });

            _effectsList.drawElementCallback = (rect, index, isActive, isFocused) =>
            {
                var element = _target.effects[index];

                if (element != null)
                {
                    rect.y += 2;
                    rect.width -= 10;
                    rect.height = EditorGUIUtility.singleLineHeight;

                    var label = element.GetDescription();
                    EditorGUI.LabelField(rect, label, EditorStyles.boldLabel);
                    rect.y += 5;
                    rect.y += EditorGUIUtility.singleLineHeight;

                    element.Draw(rect);

                    if (GUI.changed)
                    {
                        EditorUtility.SetDirty(element);
                    }
                }
            };

            _effectsList.elementHeightCallback = (index) =>
            {
                var element = _target.effects[index];
                return element.GetHeight();
            };
        }

        private void CreateEffectCallback(object obj)
        {
            var effect = ScriptableObject.CreateInstance((Type)obj) as Effect;

            if (effect != null)
            {
                effect.hideFlags = HideFlags.HideInHierarchy;
                _target.effects.Add(effect);

                var template = target as GlobalStatusTemplate;
                var path = AssetDatabase.GetAssetPath(template);
                AssetDatabase.AddObjectToAsset(effect, path);
                EditorUtility.SetDirty(template);
            }
        }
        #endregion
    }
}
#endif