using FrameWork.Editor;
using System.Collections.Generic;
using UnityEngine;

namespace Temporary.Core
{
    [CreateAssetMenu(menuName = "Templates/Buff", fileName = "Buff", order = 0)]
    public class BuffTemplate : ScriptableObject
    {
        [Label("���� �̸�")] public string displayName;
        [Label("����"), TextArea] public string description;

        [Space(10)]

        [Label("���� �ð�")] public float delay;

        [Space(10)]

        [Label("���ݽ� ������ ��������")] public bool useAttackCountLimit;
        [Condition("useAttackCountLimit", true, false)]
        [Label("���� Ƚ��")] public int attackCount;

        [HideInInspector]
        public List<Effect> effects;

        //[Header("FX")]
        //[Tooltip("���� �� ����Ǵ� ȿ��")]
        //public List<FX> EnableFX = new List<FX>();
        //[Tooltip("���� ����Ǵ� ȿ��")]
        //public List<FX> UpdateFX = new List<FX>();
        //[Tooltip("���� �� ����Ǵ� ȿ��")]
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

            menu.AddItem(new GUIContent("�̵��ӵ� ����"), false, CreateEffectCallback, typeof(MoveIncreaseDataEffect));
            menu.AddItem(new GUIContent("�̵��ӵ� ��¡��϶�"), false, CreateEffectCallback, typeof(MoveMultiplierDataEffect));

            menu.AddItem(new GUIContent("���ݷ� �߰�"), false, CreateEffectCallback, typeof(ATKAdditionalDataEffect));
            menu.AddItem(new GUIContent("���ݷ� ����"), false, CreateEffectCallback, typeof(ATKIncreaseDataEffect));
            menu.AddItem(new GUIContent("���ݷ� ��¡��϶�"), false, CreateEffectCallback, typeof(ATKMultiplierDataEffect));

            menu.AddItem(new GUIContent("�ִ� ���� ���� �� �߰�"), false, CreateEffectCallback, typeof(AttackCountAdditionalDataEffect));

            menu.AddItem(new GUIContent("���ݼӵ� ����"), false, CreateEffectCallback, typeof(AttackSpeedIncreaseDataEffect));
            menu.AddItem(new GUIContent("���ݼӵ� ��¡��϶�"), false, CreateEffectCallback, typeof(AttackSpeedMultiplierDataEffect));

            menu.AddItem(new GUIContent("ȸ���� �߰�"), false, CreateEffectCallback, typeof(AvoidanceAdditionalDataEffect));

            menu.AddItem(new GUIContent("���� ����� �߰�"), false, CreateEffectCallback, typeof(PhysicalPenetrationAdditionalDataEffect));
            menu.AddItem(new GUIContent("���� ����� ����"), false, CreateEffectCallback, typeof(PhysicalPenetrationIncreaseDataEffect));
            menu.AddItem(new GUIContent("���� ����� ��¡��϶�"), false, CreateEffectCallback, typeof(PhysicalPenetrationMultiplierDataEffect));

            menu.AddItem(new GUIContent("���� ���׷� �߰�"), false, CreateEffectCallback, typeof(PhysicalResistanceAdditionalDataEffect));
            menu.AddItem(new GUIContent("���� ���׷� ����"), false, CreateEffectCallback, typeof(PhysicalResistanceIncreaseDataEffect));
            menu.AddItem(new GUIContent("���� ���׷� ��¡��϶�"), false, CreateEffectCallback, typeof(PhysicalResistanceMultiplierDataEffect));

            menu.AddItem(new GUIContent("���� ����� �߰�"), false, CreateEffectCallback, typeof(MagicPenetrationAdditionalDataEffect));
            menu.AddItem(new GUIContent("���� ����� ����"), false, CreateEffectCallback, typeof(MagicPenetrationIncreaseDataEffect));
            menu.AddItem(new GUIContent("���� ����� ��¡��϶�"), false, CreateEffectCallback, typeof(MagicPenetrationMultiplierDataEffect));

            menu.AddItem(new GUIContent("���� ���׷� �߰�"), false, CreateEffectCallback, typeof(MagicResistanceAdditionalDataEffect));
            menu.AddItem(new GUIContent("���� ���׷� ����"), false, CreateEffectCallback, typeof(MagicResistanceIncreaseDataEffect));
            menu.AddItem(new GUIContent("���� ���׷� ��¡��϶�"), false, CreateEffectCallback, typeof(MagicResistanceMultiplierDataEffect));

            menu.AddItem(new GUIContent("���ط� �߰�"), false, CreateEffectCallback, typeof(DamageAdditionalDataEffect));
            menu.AddItem(new GUIContent("���ط� ����"), false, CreateEffectCallback, typeof(DamageIncreaseDataEffect));
            menu.AddItem(new GUIContent("���ط� ��¡��϶�"), false, CreateEffectCallback, typeof(DamageMultiplierDataEffect));

            menu.AddItem(new GUIContent("�޴� ���ط� �߰�"), false, CreateEffectCallback, typeof(ReceiveDamageAdditionalDataEffect));
            menu.AddItem(new GUIContent("�޴� ���ط� ����"), false, CreateEffectCallback, typeof(ReceiveDamageIncreaseDataEffect));
            menu.AddItem(new GUIContent("�޴� ���ط� ��¡��϶�"), false, CreateEffectCallback, typeof(ReceiveDamageMultiplierDataEffect));

            menu.AddItem(new GUIContent("ġ��Ÿ Ȯ�� �߰�"), false, CreateEffectCallback, typeof(CriticalHitChanceAdditionalDataEffect));
            menu.AddItem(new GUIContent("ġ��Ÿ ������ �߰�"), false, CreateEffectCallback, typeof(CriticalHitDamageAdditionalDataEffect));
            menu.AddItem(new GUIContent("ġ��Ÿ ������ ����"), false, CreateEffectCallback, typeof(CriticalHitDamageIncreaseDataEffect));
            menu.AddItem(new GUIContent("ġ��Ÿ ������ ��¡��϶�"), false, CreateEffectCallback, typeof(CriticalHitDamageMultiplierDataEffect));

            menu.AddItem(new GUIContent("�ִ� ü�� �߰�"), false, CreateEffectCallback, typeof(MaxHPAdditionalDataEffect));
            menu.AddItem(new GUIContent("�ִ� ü�� ����"), false, CreateEffectCallback, typeof(MaxHPIncreaseDataEffect));
            menu.AddItem(new GUIContent("�ִ� ü�� ��¡��϶�"), false, CreateEffectCallback, typeof(MaxHPMultiplierDataEffect));
            
            menu.AddItem(new GUIContent("ȸ���� �߰�"), false, CreateEffectCallback, typeof(HealingAdditionalDataEffect));
            menu.AddItem(new GUIContent("ȸ���� ����"), false, CreateEffectCallback, typeof(HealingIncreaseDataEffect));
            menu.AddItem(new GUIContent("ȸ���� ��¡��϶�"), false, CreateEffectCallback, typeof(HealingMultiplierDataEffect));

            menu.AddItem(new GUIContent("�ʴ� ȸ���� ����"), false, CreateEffectCallback, typeof(HPRecoveryPerSecByMaxHPIncreaseDataEffect));

            menu.AddItem(new GUIContent("�ּ� ü�� ����"), false, CreateEffectCallback, typeof(SetMinHPEffect));
            menu.AddItem(new GUIContent("���� ��� ����"), false, CreateEffectCallback, typeof(SetAttackTypeEffect));
            menu.AddItem(new GUIContent("���ط� ���� ��� ����"), false, CreateEffectCallback, typeof(SetDamageTypeEffect));

            menu.AddItem(new GUIContent("���� ����� ���� �ʽ��ϴ�."), false, CreateEffectCallback, typeof(UnableToTargetOfAttackEffect));

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