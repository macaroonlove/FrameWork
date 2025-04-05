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

        [HideInInspector, SerializeField] private EActiveSkillType _skillType;
        [HideInInspector, SerializeField] private EUnitType _unitType;
        [HideInInspector, SerializeField] private float _skillRange;

        [HideInInspector, SerializeField] private string _parameterName;
        [HideInInspector, SerializeField] private int _parameterHash;

        [HideInInspector, SerializeField] private FX _casterFX;

        [HideInInspector]
        public List<Effect> effects = new List<Effect>();

        #region 프로퍼티
        public Sprite sprite => _sprite;

        public int id => _id;
        public string displayName => _displayName;
        public string description => _description;

        public int needMana => _needMana;
        public float cooldownTime => _cooldownTime;

        public EActiveSkillType skillType => _skillType;
        public EUnitType unitType => _unitType;
        public float skillRange => _skillRange;

        public int parameterHash => _parameterHash;

        public FX casterFX => _casterFX;
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
        private SerializedProperty _skillType;
        private SerializedProperty _unitType;
        private SerializedProperty _skillRange;
        private SerializedProperty _parameterName;
        private SerializedProperty _parameterHash;
        private SerializedProperty _casterFX;

        private ReorderableList _effectsList;
        private Effect _currentEffect;

        private void OnEnable()
        {
            _target = target as ActiveSkillTemplate;

            _sprite = serializedObject.FindProperty("_sprite");
            _id = serializedObject.FindProperty("_id");
            _displayName = serializedObject.FindProperty("_displayName");
            _description = serializedObject.FindProperty("_description");
            _needMana = serializedObject.FindProperty("_needMana");
            _cooldownTime = serializedObject.FindProperty("_cooldownTime");
            _skillType = serializedObject.FindProperty("_skillType");
            _unitType = serializedObject.FindProperty("_unitType");
            _skillRange = serializedObject.FindProperty("_skillRange");
            _parameterName = serializedObject.FindProperty("_parameterName");
            _parameterHash = serializedObject.FindProperty("_parameterHash");
            _casterFX = serializedObject.FindProperty("_casterFX");

            CreateEffectList();
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            GUILayout.BeginHorizontal();

            _sprite.objectReferenceValue = EditorGUILayout.ObjectField(_sprite.objectReferenceValue, typeof(Sprite), false, GUILayout.Width(96), GUILayout.Height(96));

            GUILayout.BeginVertical();

            GUILayout.BeginHorizontal();
            GUILayout.Label("식별번호", GUILayout.Width(80));
            EditorGUILayout.PropertyField(_id, GUIContent.none);
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.Label("스킬 이름", GUILayout.Width(80));
            EditorGUILayout.PropertyField(_displayName, GUIContent.none);
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.Label("스킬 설명", GUILayout.Width(80));
            _description.stringValue = EditorGUILayout.TextArea(_description.stringValue, GUILayout.Height(50));
            GUILayout.EndHorizontal();

            GUILayout.EndVertical();

            GUILayout.EndHorizontal();

            GUILayout.Space(10);

            GUILayout.BeginHorizontal();
            GUILayout.Label("소모 마나량", GUILayout.Width(192));
            EditorGUILayout.PropertyField(_needMana, GUIContent.none);
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.Label("쿨타임", GUILayout.Width(192));
            EditorGUILayout.PropertyField(_cooldownTime, GUIContent.none);
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.Label("스킬 방식", GUILayout.Width(192));
            EditorGUILayout.PropertyField(_skillType, GUIContent.none);
            GUILayout.EndHorizontal();

            var skillType = _skillType.enumValueIndex;

            if (skillType == (int)EActiveSkillType.MouseTargeting)
            {
                GUILayout.BeginHorizontal();
                GUILayout.Label("마우스가 인식할 스킬 범위", GUILayout.Width(192));
                EditorGUILayout.PropertyField(_skillRange, GUIContent.none);
                GUILayout.EndHorizontal();

                GUILayout.BeginHorizontal();
                GUILayout.Label("마우스가 인식할 유닛 타입", GUILayout.Width(192));
                EditorGUILayout.PropertyField(_unitType, GUIContent.none);
                GUILayout.EndHorizontal();
            }

            GUILayout.Space(10);

            GUILayout.BeginHorizontal();
            GUILayout.Label("애니메이션 파라미터", GUILayout.Width(192));
            EditorGUILayout.PropertyField(_parameterName, GUIContent.none);
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.Label("파라미터 해시 값", GUILayout.Width(192));
            GUI.enabled = false;
            EditorGUILayout.PropertyField(_parameterHash, GUIContent.none);
            GUI.enabled = true;
            GUILayout.EndHorizontal();

            GUILayout.Space(4);
            if (GUILayout.Button("해시 값 생성"))
            {
                _parameterHash.intValue = Animator.StringToHash(_parameterName.stringValue);
            }

            GUILayout.Space(10);

            GUILayout.BeginHorizontal();
            GUILayout.Label("스킬 사용 시, 시전자 FX", GUILayout.Width(192));
            EditorGUILayout.PropertyField(_casterFX, GUIContent.none);
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

            switch (_skillType.enumValueIndex)
            {
                case (int)EActiveSkillType.InstantNonTargeting:
                case (int)EActiveSkillType.MouseNonTargeting:
                    menu.AddItem(new GUIContent("즉시 데미지 스킬 (논타겟팅)"), false, CreateEffectCallback, typeof(InstantDamagePointEffect));
                    menu.AddItem(new GUIContent("투사체 데미지 스킬 (논타겟팅)"), false, CreateEffectCallback, typeof(ProjectileDamagePointEffect));
                    menu.AddItem(new GUIContent("즉시 회복 스킬 (논타겟팅)"), false, CreateEffectCallback, typeof(InstantHealPointEffect));
                    menu.AddItem(new GUIContent("투사체 회복 스킬 (논타겟팅)"), false, CreateEffectCallback, typeof(ProjectileHealPointEffect));
                    menu.AddItem(new GUIContent("즉시 보호막 스킬 (논타겟팅)"), false, CreateEffectCallback, typeof(InstantShieldPointEffect));
                    menu.AddItem(new GUIContent("투사체 보호막 스킬 (논타겟팅)"), false, CreateEffectCallback, typeof(ProjectileShieldPointEffect));
                    menu.AddItem(new GUIContent("즉시 버프 스킬 (논타겟팅)"), false, CreateEffectCallback, typeof(InstantBuffPointEffect));
                    menu.AddItem(new GUIContent("투사체 버프 스킬 (논타겟팅)"), false, CreateEffectCallback, typeof(ProjectileBuffPointEffect));
                    menu.AddItem(new GUIContent("즉시 상태이상 스킬 (논타겟팅)"), false, CreateEffectCallback, typeof(InstantAbnormalStatusPointEffect));
                    menu.AddItem(new GUIContent("투사체 상태이상 스킬 (논타겟팅)"), false, CreateEffectCallback, typeof(ProjectileAbnormalStatusPointEffect));
                    break;
                case (int)EActiveSkillType.InstantTargeting:
                case (int)EActiveSkillType.MouseTargeting:
                    menu.AddItem(new GUIContent("즉시 데미지 스킬 (타겟팅)"), false, CreateEffectCallback, typeof(InstantDamageByTargetUnitEffect));
                    menu.AddItem(new GUIContent("투사체 데미지 스킬 (타겟팅)"), false, CreateEffectCallback, typeof(ProjectileDamageByTargetUnitEffect));
                    menu.AddItem(new GUIContent("즉시 회복 스킬 (타겟팅)"), false, CreateEffectCallback, typeof(InstantHealByTargetUnitEffect));
                    menu.AddItem(new GUIContent("투사체 회복 스킬 (타겟팅)"), false, CreateEffectCallback, typeof(ProjectileHealByTargetUnitEffect));
                    menu.AddItem(new GUIContent("즉시 보호막 스킬 (타겟팅)"), false, CreateEffectCallback, typeof(InstantShieldByTargetUnitEffect));
                    menu.AddItem(new GUIContent("투사체 보호막 스킬 (타겟팅)"), false, CreateEffectCallback, typeof(ProjectileShieldByTargetUnitEffect));
                    menu.AddItem(new GUIContent("즉시 버프 스킬 (타겟팅)"), false, CreateEffectCallback, typeof(InstantBuffByTargetUnitEffect));
                    menu.AddItem(new GUIContent("투사체 버프 스킬 (타겟팅)"), false, CreateEffectCallback, typeof(ProjectileBuffByTargetUnitEffect));
                    menu.AddItem(new GUIContent("즉시 상태이상 스킬 (타겟팅)"), false, CreateEffectCallback, typeof(InstantAbnormalStatusByTargetUnitEffect));
                    menu.AddItem(new GUIContent("투사체 상태이상 스킬 (타겟팅)"), false, CreateEffectCallback, typeof(ProjectileAbnormalStatusByTargetUnitEffect));
                    break;
            }

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
            var effect = ScriptableObject.CreateInstance((Type)obj) as Effect;

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