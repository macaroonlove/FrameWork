using FrameWork.Editor;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Temporary.Core
{
    public class AbnormalStatusAbility : AlwaysAbility
    {
        #region Effect List
        [NonSerialized] public List<UnableToMoveEffect> UnableToMoveEffects = new List<UnableToMoveEffect>();
        [NonSerialized] public List<UnableToAttackEffect> UnableToAttackEffects = new List<UnableToAttackEffect>();
        [NonSerialized] public List<UnableToHealEffect> UnableToHealEffects = new List<UnableToHealEffect>();
        [NonSerialized] public List<UnableToSkillEffect> UnableToSkillEffects = new List<UnableToSkillEffect>();
        [NonSerialized] public List<MoveIncreaseDataEffect> MoveIncreaseDataEffects = new List<MoveIncreaseDataEffect>();
        [NonSerialized] public List<PhysicalResistanceIncreaseDataEffect> PhysicalResistanceIncreaseDataEffects = new List<PhysicalResistanceIncreaseDataEffect>();
        [NonSerialized] public List<MagicResistanceIncreaseDataEffect> MagicResistanceIncreaseDataEffects = new List<MagicResistanceIncreaseDataEffect>();
        [NonSerialized] public List<ReceiveDamageIncreaseDataEffect> ReceiveDamageIncreaseDataEffects = new List<ReceiveDamageIncreaseDataEffect>();
        [NonSerialized] public List<HPRecoveryPerSecByMaxHPIncreaseDataEffect> HPRecoveryPerSecByMaxHPIncreaseDataEffects = new List<HPRecoveryPerSecByMaxHPIncreaseDataEffect>();
        #endregion

        private Dictionary<AbnormalStatusTemplate, StatusInstance> statusDic = new Dictionary<AbnormalStatusTemplate, StatusInstance>();

#if UNITY_EDITOR
        [SerializeField, ReadOnly] private List<AbnormalStatusTemplate> statusList = new List<AbnormalStatusTemplate>();
#endif

        internal override void Initialize(Unit unit)
        {
            base.Initialize(unit);

            unit.GetAbility<HitAbility>().onHit += RemoveStatusByHit;
            unit.GetAbility<HealthAbility>().onDeath += ClearStatusEffects;
        }

        internal void DeInitialize()
        {
            unit.GetAbility<HitAbility>().onHit -= RemoveStatusByHit;
            unit.GetAbility<HealthAbility>().onDeath -= ClearStatusEffects;
        }

        internal void ApplyAbnormalStatus(AbnormalStatusTemplate template, float duration)
        {
            if (this == null || gameObject == null) return;

            var isContained = false;

            if (statusDic.ContainsKey(template))
            {
                isContained = true;

                var instance = statusDic[template];
                if (instance.IsOld(duration))
                {
                    instance.duration = duration;
                    instance.startTime = Time.time;

                    if (template.useHitCountLimit)
                    {
                        instance.useCountLimit = true;
                        instance.count = template.hitCount;
                    }
                    return;
                }
                else
                {
                    return;
                }
            }

            if (template.delay > 0)
            {
                StartCoroutine(CoAddStatus(template, duration, isContained));
            }
            else
            {
                AddStatus(template, duration, isContained);
            }
        }

        private IEnumerator CoAddStatus(AbnormalStatusTemplate template, float duration, bool isContained)
        {
            yield return new WaitForSeconds(template.delay);
            AddStatus(template, duration, isContained);
        }

        /// <summary>
        /// 상태이상 추가
        /// </summary>
        private void AddStatus(AbnormalStatusTemplate template, float duration, bool isContained)
        {
            StatusInstance statusInstance = new StatusInstance(duration, Time.time);
            
            // 무한지속이 아니라면
            if (duration != int.MaxValue)
            {
                var corutine = StartCoroutine(CoStatus(statusInstance, template));
                statusInstance.corutine = corutine;
            }
            
            // 피격시 상태이상이 해제되야 한다면
            if (template.useHitCountLimit)
            {
                statusInstance.useCountLimit = true;
                statusInstance.count = template.hitCount;
            }

            statusDic.Add(template, statusInstance);

#if UNITY_EDITOR
            statusList.Add(template);
#endif

            // 상태이상 효과 적용 (동일한 상태이상 효과는 중복되지 않음)
            if (isContained == false)
            {
                foreach (var effect in template.effects)
                {
                    if (effect is UnableToMoveEffect unableToMoveEffect)
                    {
                        UnableToMoveEffects.Add(unableToMoveEffect);
                    }
                    else if (effect is UnableToAttackEffect unableToAttackEffect)
                    {
                        UnableToAttackEffects.Add(unableToAttackEffect);
                    }
                    else if (effect is UnableToHealEffect unableToHealEffects)
                    {
                        UnableToHealEffects.Add(unableToHealEffects);
                    }
                    else if (effect is UnableToSkillEffect unableToSkillEffect)
                    {
                        UnableToSkillEffects.Add(unableToSkillEffect);
                    }
                    else if (effect is MoveIncreaseDataEffect moveIncreaseDataEffect)
                    {
                        MoveIncreaseDataEffects.Add(moveIncreaseDataEffect);
                    }
                    else if (effect is PhysicalResistanceIncreaseDataEffect physicalResistanceIncreaseDataEffect)
                    {
                        PhysicalResistanceIncreaseDataEffects.Add(physicalResistanceIncreaseDataEffect);
                    }
                    else if (effect is MagicResistanceIncreaseDataEffect magicResistanceIncreaseDataEffect)
                    {
                        MagicResistanceIncreaseDataEffects.Add(magicResistanceIncreaseDataEffect);
                    }
                    else if (effect is ReceiveDamageIncreaseDataEffect receiveDamageIncreaseDataEffect)
                    {
                        ReceiveDamageIncreaseDataEffects.Add(receiveDamageIncreaseDataEffect);
                    }
                    else if (effect is HPRecoveryPerSecByMaxHPIncreaseDataEffect hpRecoveryPerSecByMaxHPIncreaseDataEffect)
                    {
                        HPRecoveryPerSecByMaxHPIncreaseDataEffects.Add(hpRecoveryPerSecByMaxHPIncreaseDataEffect);
                    }
                }
            }
        }

        private IEnumerator CoStatus(StatusInstance statusInstance, AbnormalStatusTemplate template)
        {
            while (statusInstance.IsCompete == false)
            {
                yield return null;
            }

            RemoveStatus(template.effects);

            if (statusDic.ContainsKey(template))
            {
                statusDic.Remove(template);

#if UNITY_EDITOR
                statusList.Remove(template);
#endif
            }
        }

        #region 콜백 메서드
        private void RemoveStatusByHit()
        {
            List<AbnormalStatusTemplate> templates = new List<AbnormalStatusTemplate>();
            foreach (var status in statusDic)
            {
                var template = status.Key;
                var instance = status.Value;

                if (instance.useCountLimit)
                {
                    instance.count--;

                    if (instance.count == 0)
                    {
                        RemoveStatus(template.effects);
                        
                        if (instance.corutine != null)
                        {
                            StopCoroutine(instance.corutine);
                            instance.corutine = null;
                        }
                        templates.Add(template);
                    }
                }
            }

            foreach (var template in templates)
            {
                if (statusDic.ContainsKey(template))
                {
                    statusDic.Remove(template);

#if UNITY_EDITOR
                    statusList.Remove(template);
#endif
                }
            }
        }

        private void ClearStatusEffects()
        {
            foreach (var status in statusDic)
            {
                var instance = status.Value;

                RemoveStatus(status.Key.effects);

                if (instance.corutine != null)
                {
                    StopCoroutine(instance.corutine);
                    instance.corutine = null;
                }
            }

            statusDic.Clear();

#if UNITY_EDITOR
            statusList.Clear();
#endif
        }
        #endregion

        /// <summary>
        /// 상태이상 제거
        /// </summary>
        private void RemoveStatus(List<Effect> effects)
        {
            foreach (var effect in effects)
            {
                if (effect is UnableToMoveEffect unableToMoveEffect)
                {
                    UnableToMoveEffects.Remove(unableToMoveEffect);
                }
                else if (effect is UnableToAttackEffect unableToAttackEffect)
                {
                    UnableToAttackEffects.Remove(unableToAttackEffect);
                }
                else if (effect is UnableToHealEffect unableToHealEffects)
                {
                    UnableToHealEffects.Remove(unableToHealEffects);
                }
                else if (effect is UnableToSkillEffect unableToSkillEffect)
                {
                    UnableToSkillEffects.Remove(unableToSkillEffect);
                }
                else if (effect is MoveIncreaseDataEffect moveIncreaseDataEffect)
                {
                    MoveIncreaseDataEffects.Remove(moveIncreaseDataEffect);
                }
                else if (effect is PhysicalResistanceIncreaseDataEffect physicalResistanceIncreaseDataEffect)
                {
                    PhysicalResistanceIncreaseDataEffects.Remove(physicalResistanceIncreaseDataEffect);
                }
                else if (effect is MagicResistanceIncreaseDataEffect magicResistanceIncreaseDataEffect)
                {
                    MagicResistanceIncreaseDataEffects.Remove(magicResistanceIncreaseDataEffect);
                }
                else if (effect is ReceiveDamageIncreaseDataEffect receiveDamageIncreaseDataEffect)
                {
                    ReceiveDamageIncreaseDataEffects.Remove(receiveDamageIncreaseDataEffect);
                }
                else if (effect is HPRecoveryPerSecByMaxHPIncreaseDataEffect hpRecoveryPerSecByMaxHPIncreaseDataEffect)
                {
                    HPRecoveryPerSecByMaxHPIncreaseDataEffects.Remove(hpRecoveryPerSecByMaxHPIncreaseDataEffect);
                }
            }
        }

        #region 유틸리티 메서드
        internal bool Contains(AbnormalStatusTemplate template)
        {
            return statusDic.ContainsKey(template);
        }

        internal bool Contains(List<AbnormalStatusTemplate> templates)
        {
            var isContains = false;
            foreach (var template in templates)
            {
                if (statusDic.ContainsKey(template))
                {
                    isContains = true;
                }
            }
            return isContains;
        }
        #endregion
    }
}