using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Temporary.Core
{
    /// <summary>
    /// ������ �нú� ��ų ����� �����մϴ�.
    /// </summary>
    public class PassiveSkillAbility : AlwaysAbility
    {
        #region Effect List
        private List<EventSkillEffect> _attackEventEffects = new List<EventSkillEffect>();
        private List<EventSkillEffect> _hitEventEffects = new List<EventSkillEffect>();
        private List<EventSkillEffect> _healEventEffects = new List<EventSkillEffect>();
        private List<EventSkillEffect> _destroyShieldEventEffects = new List<EventSkillEffect>();
        

        #region ������Ƽ
        internal IReadOnlyList<EventSkillEffect> attackEventEffects => _attackEventEffects;
        internal IReadOnlyList<EventSkillEffect> hitEventEffects => _hitEventEffects;
        internal IReadOnlyList<EventSkillEffect> healEventEffects => _healEventEffects;
        internal IReadOnlyList<EventSkillEffect> destroyShieldEventEffects => _destroyShieldEventEffects;
        
        #endregion
        #endregion

        internal void InitializePassiveSkill(SkillTreeGraph skillTree)
        {
            foreach (var node in skillTree.nodes)
            {
                if (node is PassiveSkillNode skill)
                {
                    foreach (var trigger in skill.skillTemplate.triggers)
                    {
                        // ��� ����� ȿ��
                        if (trigger is AlwaysUnitTrigger alwaysUnitTrigger)
                        {
                            foreach (var effect in alwaysUnitTrigger.effects)
                            {
                                if (effect is AlwaysSkillEffect alwaysEffect)
                                {
                                    alwaysEffect.Execute(unit);
                                }
                            }
                        }
                        // �⺻ ����/ȸ�� �� ����� ȿ��
                        else if (trigger is AttackEventUnitTrigger attackEventUnitTrigger)
                        {
                            foreach (var effect in attackEventUnitTrigger.effects)
                            {
                                if (effect is EventSkillEffect eventEffect)
                                {
                                    _attackEventEffects.Add(eventEffect);
                                }
                            }
                        }
                        // �ǰ� �� ����� ȿ��
                        else if (trigger is HitEventUnitTrigger hitEventUnitTrigger)
                        {
                            foreach (var effect in hitEventUnitTrigger.effects)
                            {
                                if (effect is EventSkillEffect eventEffect)
                                {
                                    _hitEventEffects.Add(eventEffect);
                                }
                            }
                        }
                        // ȸ���� ���� �� ����� ȿ��
                        else if (trigger is HealEventUnitTrigger healEventUnitTrigger)
                        {
                            foreach (var effect in healEventUnitTrigger.effects)
                            {
                                if (effect is EventSkillEffect eventEffect)
                                {
                                    _healEventEffects.Add(eventEffect);
                                }
                            }
                        }
                        // ��ȣ���� �ı��� �� ����� ȿ��
                        else if (trigger is DestroyShieldEventUnitTrigger destroyShieldEventUnitTrigger)
                        {
                            foreach (var effect in destroyShieldEventUnitTrigger.effects)
                            {
                                if (effect is EventSkillEffect eventEffect)
                                {
                                    _destroyShieldEventEffects.Add(eventEffect);
                                }
                            }
                        }
                    }
                }
            }
        }

        internal override void Deinitialize()
        {
            _attackEventEffects.Clear();
            _hitEventEffects.Clear();
            _healEventEffects.Clear();
            _destroyShieldEventEffects.Clear();
        }
    }
}