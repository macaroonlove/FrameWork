using FrameWork.Editor;
using System.Collections.Generic;
using UnityEngine;

namespace Temporary.Core
{
    [CreateAssetMenu(menuName = "Templates/AbnormalStatus", fileName = "AbnormalStatus", order = 0)]
    public class AbnormalStatusTemplate : ScriptableObject
    {
        [Label("�����̻� �̸�")] public string displayName;
        [Label("����"), TextArea] public string description;

        [Space(10)]

        [Label("���� �ð�")] public float delay;

        [Space(10)]

        [Label("�ǰݽ� ���»��� ��������")] public bool useHitCountLimit;
        [Condition("useHitCountLimit", true, false)]
        [Label("�ǰ� Ƚ��")] public int hitCount;

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

    [CustomEditor(typeof(AbnormalStatusTemplate)), CanEditMultipleObjects]
    public class AbnormalStatusTemplateEditor : EffectEditor
    {
        private AbnormalStatusTemplate _target;

        private ReorderableList _effectsList;
        private Effect _currentEffect;

        private void OnEnable()
        {
            _target = target as AbnormalStatusTemplate;

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

            menu.AddItem(new GUIContent("�̵� �Ұ�"), false, CreateEffectCallback, typeof(UnableToMoveEffect));
            menu.AddItem(new GUIContent("���� �Ұ�"), false, CreateEffectCallback, typeof(UnableToAttackEffect));
            menu.AddItem(new GUIContent("ȸ�� �Ұ�"), false, CreateEffectCallback, typeof(UnableToHealEffect));
            menu.AddItem(new GUIContent("��ų ��� �Ұ�"), false, CreateEffectCallback, typeof(UnableToSkillEffect));
            menu.AddItem(new GUIContent("�̵��ӵ� ����"), false, CreateEffectCallback, typeof(MoveIncreaseDataEffect));
            menu.AddItem(new GUIContent("���� ���׷� ����"), false, CreateEffectCallback, typeof(PhysicalResistanceIncreaseDataEffect));
            menu.AddItem(new GUIContent("���� ���׷� ����"), false, CreateEffectCallback, typeof(MagicResistanceIncreaseDataEffect));
            menu.AddItem(new GUIContent("�޴� ���ط� ����"), false, CreateEffectCallback, typeof(ReceiveDamageIncreaseDataEffect));
            menu.AddItem(new GUIContent("�ִ� ü�� ��� �ʴ� ü�� ȸ����"), false, CreateEffectCallback, typeof(HPRecoveryPerSecByMaxHPIncreaseDataEffect));

            menu.ShowAsContext();
        }

        private void CreateEffectList()
        {
            _effectsList = SetupReorderableList("Abnormal Status Effects", _target.effects,
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

                var template = target as AbnormalStatusTemplate;
                var path = AssetDatabase.GetAssetPath(template);
                AssetDatabase.AddObjectToAsset(effect, path);
                EditorUtility.SetDirty(template);
            }
        }
    }
}
#endif