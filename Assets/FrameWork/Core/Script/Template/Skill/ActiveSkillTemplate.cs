using FrameWork.Editor;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Temporary.Core
{
    [CreateAssetMenu(menuName = "Templates/Skill/Active Skill", fileName = "ActiveSkill", order = 0)]
    public class ActiveSkillTemplate : ScriptableObject
    {
        [HideInInspector, SerializeField] private Sprite _sprite;

        [HideInInspector, SerializeField] private int _id;
        [HideInInspector, SerializeField] private string _displayName;
        [HideInInspector, SerializeField] private string _description;

        [HideInInspector, SerializeField] private int _needMana;
        [HideInInspector, SerializeField] private float _cooldownTime;
        [HideInInspector, SerializeField] private float _skillRange;

        [HideInInspector, SerializeField] private string _parameterName;
        [HideInInspector, SerializeField] private int _parameterHash;

        [HideInInspector]
        public List<EventSkillEffect> effects;

        #region ������Ƽ
        public Sprite sprite => _sprite;

        public int id => _id;
        public string displayName => _displayName;
        public string description => _description;

        public int needMana => _needMana;
        public float cooldownTime => _cooldownTime;
        public float skillRange => _skillRange;

        public int parameterHash => _parameterHash;
        #endregion

        #region �� ���� �޼���
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

    [CustomEditor(typeof(ActiveSkillTemplate)), CanEditMultipleObjects]
    public class ActiveSkillTemplateEditor : EffectEditor
    {
        private ActiveSkillTemplate _target;

        private SerializedProperty _sprite;
        private SerializedProperty _id;
        private SerializedProperty _displayName;
        private SerializedProperty _description;
        private SerializedProperty _needMana;
        private SerializedProperty _cooldownTime;
        private SerializedProperty _skillRange;
        private SerializedProperty _parameterName;
        private SerializedProperty _parameterHash;

        private ReorderableList _effectsList;
        private EventSkillEffect _currentEffect;

        private void OnEnable()
        {
            _target = target as ActiveSkillTemplate;

            _sprite = serializedObject.FindProperty("_sprite");
            _id = serializedObject.FindProperty("_id");
            _displayName = serializedObject.FindProperty("_displayName");
            _description = serializedObject.FindProperty("_description");
            _needMana = serializedObject.FindProperty("_needMana");
            _cooldownTime = serializedObject.FindProperty("_cooldownTime");
            _skillRange = serializedObject.FindProperty("_skillRange");
            _parameterName = serializedObject.FindProperty("_parameterName");
            _parameterHash = serializedObject.FindProperty("_parameterHash");

            CreateEffectList();
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            GUILayout.BeginHorizontal();
            
            _sprite.objectReferenceValue = EditorGUILayout.ObjectField(_sprite.objectReferenceValue, typeof(Sprite), false, GUILayout.Width(96), GUILayout.Height(96));
            
            GUILayout.BeginVertical();

            GUILayout.BeginHorizontal();
            GUILayout.Label("�ĺ���ȣ", GUILayout.Width(80));
            EditorGUILayout.PropertyField(_id, GUIContent.none);
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.Label("��ų �̸�", GUILayout.Width(80));
            EditorGUILayout.PropertyField(_displayName, GUIContent.none);
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.Label("��ų ����", GUILayout.Width(80));
            _description.stringValue = EditorGUILayout.TextArea(_description.stringValue, GUILayout.Height(50));
            GUILayout.EndHorizontal();

            GUILayout.EndVertical();
            
            GUILayout.EndHorizontal();

            GUILayout.Space(10);

            GUILayout.BeginHorizontal();
            GUILayout.Label("�Ҹ� ������", GUILayout.Width(192));
            EditorGUILayout.PropertyField(_needMana, GUIContent.none);
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.Label("��Ÿ��", GUILayout.Width(192));
            EditorGUILayout.PropertyField(_cooldownTime, GUIContent.none);
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.Label("��ų ����", GUILayout.Width(192));
            EditorGUILayout.PropertyField(_skillRange, GUIContent.none);
            GUILayout.EndHorizontal();

            GUILayout.Space(10);

            GUILayout.BeginHorizontal();
            GUILayout.Label("�ִϸ��̼� �Ķ����", GUILayout.Width(192));
            EditorGUILayout.PropertyField(_parameterName, GUIContent.none);
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.Label("�Ķ���� �ؽ� ��", GUILayout.Width(192));
            GUI.enabled = false;
            EditorGUILayout.PropertyField(_parameterHash, GUIContent.none);
            GUI.enabled = true;
            GUILayout.EndHorizontal();

            GUILayout.Space(4);
            if (GUILayout.Button("�ؽ� �� ����"))
            {
                _parameterHash.intValue = Animator.StringToHash(_parameterName.stringValue);
            }

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

            menu.AddItem(new GUIContent("��� ������ ��ų"), false, CreateEffectCallback, typeof(InstantDamageEventSkillEffect));
            menu.AddItem(new GUIContent("����ü ������ ��ų"), false, CreateEffectCallback, typeof(ProjectileDamageEventSkillEffect));
            menu.AddItem(new GUIContent("��� ȸ�� ��ų"), false, CreateEffectCallback, typeof(InstantHealEventSkillEffect));
            menu.AddItem(new GUIContent("����ü ȸ�� ��ų"), false, CreateEffectCallback, typeof(ProjectileHealEventSkillEffect));
            menu.AddItem(new GUIContent("��� ��ȣ�� ��ų"), false, CreateEffectCallback, typeof(InstantShieldEventSkillEffect));
            menu.AddItem(new GUIContent("����ü ��ȣ�� ��ų"), false, CreateEffectCallback, typeof(ProjectileShieldEventSkillEffect));
            menu.AddItem(new GUIContent("��� ���� ��ų"), false, CreateEffectCallback, typeof(InstantBuffEventSkillEffect));
            menu.AddItem(new GUIContent("����ü ���� ��ų"), false, CreateEffectCallback, typeof(ProjectileBuffEventSkillEffect));
            menu.AddItem(new GUIContent("��� �����̻� ��ų"), false, CreateEffectCallback, typeof(InstantAbnormalStatusEventSkillEffect));
            menu.AddItem(new GUIContent("����ü �����̻� ��ų"), false, CreateEffectCallback, typeof(ProjectileAbnormalStatusEventSkillEffect));

            menu.ShowAsContext();
        }

        private void CreateEffectList()
        {
            _effectsList = SetupReorderableList("Active Skill Effects", _target.effects,
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
            var effect = ScriptableObject.CreateInstance((Type)obj) as EventSkillEffect;

            if (effect != null)
            {
                effect.hideFlags = HideFlags.HideInHierarchy;
                _target.effects.Add(effect);

                var template = target as ActiveSkillTemplate;
                var path = AssetDatabase.GetAssetPath(template);
                AssetDatabase.AddObjectToAsset(effect, path);
                EditorUtility.SetDirty(template);
            }
        }
        #endregion
    }
}
#endif