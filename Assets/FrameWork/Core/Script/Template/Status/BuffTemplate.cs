using FrameWork.Editor;
using System.Collections.Generic;
using UnityEngine;

namespace Temporary.Core
{
    [CreateAssetMenu(menuName = "Templates/Buff", fileName = "Buff", order = 0)]
    public class BuffTemplate : ScriptableObject
    {
        [Label("버프 이름")] public string displayName;
        [Label("설명"), TextArea] public string description;

        [Space(10)]

        [Label("지연 시간")] public float delay;

        [Space(10)]

        [Label("공격시 버프가 해제될지")] public bool useAttackCountLimit;
        [Condition("useAttackCountLimit", true, false)]
        [Label("공격 횟수")] public int attackCount;

        [HideInInspector]
        public List<Effect> effects;

        //[Header("FX")]
        //[Tooltip("시작 시 적용되는 효과")]
        //public List<FX> EnableFX = new List<FX>();
        //[Tooltip("매턴 적용되는 효과")]
        //public List<FX> UpdateFX = new List<FX>();
        //[Tooltip("해제 시 적용되는 효과")]
        //public List<FX> DisableFX = new List<FX>();
    }
}

#if UNITY_EDITOR
namespace Temporary.Editor
{
    using System;
    using Temporary.Core;
    using UnityEditor;
    using UnityEditorInternal;

    [CustomEditor(typeof(BuffTemplate)), CanEditMultipleObjects]
    public class BuffTemplateEditor : EffectEditor
    {
        private BuffTemplate _target;

        private ReorderableList _effectsList;
        private Effect _currentEffect;

        private void OnEnable()
        {
            _target = target as BuffTemplate;

            CreateEffectList();
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            GUILayout.Space(10);

            _effectsList?.DoLayoutList();

            if (GUI.changed)
            {
                EditorUtility.SetDirty(this);
            }
        }

        private void InitMenu_Effects()
        {
            var menu = new GenericMenu();

            menu.AddItem(new GUIContent("이동속도 증감"), false, CreateEffectCallback, typeof(MoveIncreaseDataEffect));
            menu.AddItem(new GUIContent("이동속도 상승·하락"), false, CreateEffectCallback, typeof(MoveMultiplierDataEffect));

            menu.AddItem(new GUIContent("공격력 추가"), false, CreateEffectCallback, typeof(ATKAdditionalDataEffect));
            menu.AddItem(new GUIContent("공격력 증감"), false, CreateEffectCallback, typeof(ATKIncreaseDataEffect));
            menu.AddItem(new GUIContent("공격력 상승·하락"), false, CreateEffectCallback, typeof(ATKMultiplierDataEffect));

            menu.AddItem(new GUIContent("최대 공격 가능 수 추가"), false, CreateEffectCallback, typeof(AttackCountAdditionalDataEffect));

            menu.AddItem(new GUIContent("공격속도 증감"), false, CreateEffectCallback, typeof(AttackSpeedIncreaseDataEffect));
            menu.AddItem(new GUIContent("공격속도 상승·하락"), false, CreateEffectCallback, typeof(AttackSpeedMultiplierDataEffect));

            menu.AddItem(new GUIContent("회피율 추가"), false, CreateEffectCallback, typeof(AvoidanceAdditionalDataEffect));

            menu.AddItem(new GUIContent("물리 관통력 추가"), false, CreateEffectCallback, typeof(PhysicalPenetrationAdditionalDataEffect));
            menu.AddItem(new GUIContent("물리 관통력 증감"), false, CreateEffectCallback, typeof(PhysicalPenetrationIncreaseDataEffect));
            menu.AddItem(new GUIContent("물리 관통력 상승·하락"), false, CreateEffectCallback, typeof(PhysicalPenetrationMultiplierDataEffect));

            menu.AddItem(new GUIContent("물리 저항력 추가"), false, CreateEffectCallback, typeof(PhysicalResistanceAdditionalDataEffect));
            menu.AddItem(new GUIContent("물리 저항력 증감"), false, CreateEffectCallback, typeof(PhysicalResistanceIncreaseDataEffect));
            menu.AddItem(new GUIContent("물리 저항력 상승·하락"), false, CreateEffectCallback, typeof(PhysicalResistanceMultiplierDataEffect));

            menu.AddItem(new GUIContent("마법 관통력 추가"), false, CreateEffectCallback, typeof(MagicPenetrationAdditionalDataEffect));
            menu.AddItem(new GUIContent("마법 관통력 증감"), false, CreateEffectCallback, typeof(MagicPenetrationIncreaseDataEffect));
            menu.AddItem(new GUIContent("마법 관통력 상승·하락"), false, CreateEffectCallback, typeof(MagicPenetrationMultiplierDataEffect));

            menu.AddItem(new GUIContent("마법 저항력 추가"), false, CreateEffectCallback, typeof(MagicResistanceAdditionalDataEffect));
            menu.AddItem(new GUIContent("마법 저항력 증감"), false, CreateEffectCallback, typeof(MagicResistanceIncreaseDataEffect));
            menu.AddItem(new GUIContent("마법 저항력 상승·하락"), false, CreateEffectCallback, typeof(MagicResistanceMultiplierDataEffect));

            menu.AddItem(new GUIContent("피해량 추가"), false, CreateEffectCallback, typeof(DamageAdditionalDataEffect));
            menu.AddItem(new GUIContent("피해량 증감"), false, CreateEffectCallback, typeof(DamageIncreaseDataEffect));
            menu.AddItem(new GUIContent("피해량 상승·하락"), false, CreateEffectCallback, typeof(DamageMultiplierDataEffect));

            menu.AddItem(new GUIContent("받는 피해량 추가"), false, CreateEffectCallback, typeof(ReceiveDamageAdditionalDataEffect));
            menu.AddItem(new GUIContent("받는 피해량 증감"), false, CreateEffectCallback, typeof(ReceiveDamageIncreaseDataEffect));
            menu.AddItem(new GUIContent("받는 피해량 상승·하락"), false, CreateEffectCallback, typeof(ReceiveDamageMultiplierDataEffect));

            menu.AddItem(new GUIContent("치명타 확률 추가"), false, CreateEffectCallback, typeof(CriticalHitChanceAdditionalDataEffect));
            menu.AddItem(new GUIContent("치명타 데미지 추가"), false, CreateEffectCallback, typeof(CriticalHitDamageAdditionalDataEffect));
            menu.AddItem(new GUIContent("치명타 데미지 증감"), false, CreateEffectCallback, typeof(CriticalHitDamageIncreaseDataEffect));
            menu.AddItem(new GUIContent("치명타 데미지 상승·하락"), false, CreateEffectCallback, typeof(CriticalHitDamageMultiplierDataEffect));

            menu.AddItem(new GUIContent("최대 체력 추가"), false, CreateEffectCallback, typeof(MaxHPAdditionalDataEffect));
            menu.AddItem(new GUIContent("최대 체력 증감"), false, CreateEffectCallback, typeof(MaxHPIncreaseDataEffect));
            menu.AddItem(new GUIContent("최대 체력 상승·하락"), false, CreateEffectCallback, typeof(MaxHPMultiplierDataEffect));
            
            menu.AddItem(new GUIContent("회복량 추가"), false, CreateEffectCallback, typeof(HealingAdditionalDataEffect));
            menu.AddItem(new GUIContent("회복량 증감"), false, CreateEffectCallback, typeof(HealingIncreaseDataEffect));
            menu.AddItem(new GUIContent("회복량 상승·하락"), false, CreateEffectCallback, typeof(HealingMultiplierDataEffect));

            menu.AddItem(new GUIContent("초당 회복량 증감"), false, CreateEffectCallback, typeof(HPRecoveryPerSecByMaxHPIncreaseDataEffect));

            menu.AddItem(new GUIContent("최소 체력 설정"), false, CreateEffectCallback, typeof(SetMinHPEffect));
            menu.AddItem(new GUIContent("공격 방식 설정"), false, CreateEffectCallback, typeof(SetAttackTypeEffect));
            menu.AddItem(new GUIContent("피해량 적용 방식 설정"), false, CreateEffectCallback, typeof(SetDamageTypeEffect));

            menu.AddItem(new GUIContent("공격 대상이 되지 않습니다."), false, CreateEffectCallback, typeof(UnableToTargetOfAttackEffect));

            menu.ShowAsContext();
        }

        private void CreateEffectList()
        {
            _effectsList = SetupReorderableList("Buff Effects", _target.effects,
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

                var template = target as BuffTemplate;
                var path = AssetDatabase.GetAssetPath(template);
                AssetDatabase.AddObjectToAsset(effect, path);
                EditorUtility.SetDirty(template);
            }
        }
    }
}
#endif