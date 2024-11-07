using FrameWork.Editor;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Temporary.Core
{
    [CreateAssetMenu(menuName = "Templates/Skill", fileName = "Skill", order = 0)]
    public class SkillTemplate : ScriptableObject
    {
        [HideInInspector] public Sprite sprite;
        [HideInInspector] public string displayName;
        [HideInInspector] public string description;

        [HideInInspector] public int needMana;
        [HideInInspector] public float cooldownTime;
        [HideInInspector] public float skillRange;

        [HideInInspector] public string parameterName;
        [HideInInspector] public int parameterHash;

        [HideInInspector]
        public List<SkillEffect> effects;
    }
}

#if UNITY_EDITOR
namespace Temporary.Editor
{
    using System;
    using Temporary.Core;
    using UnityEditor;
    using UnityEditorInternal;

    [CustomEditor(typeof(SkillTemplate)), CanEditMultipleObjects]
    public class SkillTemplateEditor : EffectEditor
    {
        private SkillTemplate _target;

        private ReorderableList _effectsList;
        private SkillEffect _currentEffect;

        private void OnEnable()
        {
            _target = target as SkillTemplate;

            CreateEffectList();
        }

        public override void OnInspectorGUI()
        {
            GUILayout.BeginHorizontal();
            _target.sprite = EditorGUILayout.ObjectField(_target.sprite, typeof(Sprite), false, GUILayout.Width(64), GUILayout.Height(64)) as Sprite;
            GUILayout.BeginVertical();

            GUILayout.BeginHorizontal();
            GUILayout.Label("스킬 이름");
            var valueRect = GUILayoutUtility.GetLastRect();
            valueRect.x += 80;
            valueRect.width -= 80;
            _target.displayName = GUI.TextField(valueRect, _target.displayName);
            GUILayout.EndHorizontal();
            GUILayout.Space(4);
            GUILayout.BeginHorizontal();
            GUILayout.Label("스킬 설명");
            valueRect.y += EditorGUIUtility.singleLineHeight + 4;
            valueRect.height = 40;
            _target.description = GUI.TextArea(valueRect, _target.description);
            GUILayout.EndHorizontal();

            GUILayout.EndVertical();
            GUILayout.EndHorizontal();

            GUILayout.Space(10);
            GUILayout.BeginHorizontal();
            GUILayout.Label("소모 마나량");
            valueRect = GUILayoutUtility.GetLastRect();
            valueRect.x += 146;
            valueRect.width -= 146;
            _target.needMana = EditorGUI.IntField(valueRect, _target.needMana);
            GUILayout.EndHorizontal();
            
            GUILayout.Space(4);
            GUILayout.BeginHorizontal();
            GUILayout.Label("쿨타임");
            valueRect.y += EditorGUIUtility.singleLineHeight + 4;
            _target.cooldownTime = EditorGUI.FloatField(valueRect, _target.cooldownTime);
            GUILayout.EndHorizontal();

            GUILayout.Space(4);
            GUILayout.BeginHorizontal();
            GUILayout.Label("스킯 범위");
            valueRect.y += EditorGUIUtility.singleLineHeight + 4;
            _target.skillRange = EditorGUI.FloatField(valueRect, _target.skillRange);
            GUILayout.EndHorizontal();

            GUILayout.Space(10);
            GUILayout.BeginHorizontal();
            GUILayout.Label("애니메이션 파라미터");
            valueRect.y += EditorGUIUtility.singleLineHeight + 10;
            _target.parameterName = EditorGUI.TextField(valueRect, _target.parameterName);
            GUILayout.EndHorizontal();

            GUILayout.Space(4);
            GUILayout.BeginHorizontal();
            GUILayout.Label("파라미터 해시 값");
            valueRect.y += EditorGUIUtility.singleLineHeight + 4;
            _target.parameterHash = EditorGUI.IntField(valueRect, _target.parameterHash);
            GUILayout.EndHorizontal();

            GUILayout.Space(4);
            if (GUILayout.Button("해시 값 생성"))
            {
                _target.parameterHash = Animator.StringToHash(_target.parameterName);
            }

            GUILayout.Space(20);

            _effectsList?.DoLayoutList();

            if (GUI.changed)
            {
                EditorUtility.SetDirty(this);
            }
        }

        private void InitMenu_Effects()
        {
            var menu = new GenericMenu();

            //menu.AddItem(new GUIContent("이동 불가"), false, CreateEffectCallback, typeof(UnableToMoveEffect));

            menu.ShowAsContext();
        }

        private void CreateEffectList()
        {
            _effectsList = SetupReorderableList("Skill Effects", _target.effects,
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
            var effect = ScriptableObject.CreateInstance((Type)obj) as SkillEffect;

            if (effect != null)
            {
                effect.hideFlags = HideFlags.HideInHierarchy;
                _target.effects.Add(effect);

                var template = target as AbnormalStatusTemplate;
                var path = AssetDatabase.GetAssetPath(template);
                AssetDatabase.AddObjectToAsset(effect, path);
                EditorUtility.SetDirty(template);
            }
        }
    }
}
#endif