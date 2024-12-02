using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Temporary.Core
{
    /// <summary>
    /// 유닛의 패시브 스킬 사용을 제어합니다.
    /// </summary>
    public class PassiveSkillAbility : AlwaysAbility
    {
        #region Effect List
        private List<EventSkillEffect> _attackEventEffects = new List<EventSkillEffect>();
        private List<EventSkillEffect> _hitEventEffects = new List<EventSkillEffect>();
        private List<EventSkillEffect> _healEventEffects = new List<EventSkillEffect>();
        private List<EventSkillEffect> _destroyShieldEventEffects = new List<EventSkillEffect>();
        

        #region 프로퍼티
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
                        // 상시 적용될 효과
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
                        // 기본 공격/회복 시 적용될 효과
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
                        // 피격 시 적용될 효과
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
                        // 회복을 받을 시 적용될 효과
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
                        // 보호막이 파괴될 때 적용될 효과
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