using FrameWork.Editor;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Temporary.Core
{
    public class BuffAbility : AlwaysAbility
    {
        #region Effect List
        [NonSerialized] public List<MoveIncreaseDataEffect>   MoveIncreaseDataEffects = new List<MoveIncreaseDataEffect>();
        [NonSerialized] public List<MoveMultiplierDataEffect> MoveMultiplierDataEffects = new List<MoveMultiplierDataEffect>();
        
        [NonSerialized] public List<ATKAdditionalDataEffect> ATKAdditionalDataEffects = new List<ATKAdditionalDataEffect>();
        [NonSerialized] public List<ATKIncreaseDataEffect>   ATKIncreaseDataEffects = new List<ATKIncreaseDataEffect>();
        [NonSerialized] public List<ATKMultiplierDataEffect> ATKMultiplierDataEffects = new List<ATKMultiplierDataEffect>();
        
        [NonSerialized] public List<AttackCountAdditionalDataEffect> AttackCountAdditionalDataEffects = new List<AttackCountAdditionalDataEffect>();
        
        [NonSerialized] public List<AttackSpeedIncreaseDataEffect>   AttackSpeedIncreaseDataEffects = new List<AttackSpeedIncreaseDataEffect>();
        [NonSerialized] public List<AttackSpeedMultiplierDataEffect> AttackSpeedMultiplierDataEffects = new List<AttackSpeedMultiplierDataEffect>();
        
        [NonSerialized] public List<AvoidanceAdditionalDataEffect> AvoidanceAdditionalDataEffects = new List<AvoidanceAdditionalDataEffect>();
        
        [NonSerialized] public List<PhysicalPenetrationAdditionalDataEffect> PhysicalPenetrationAdditionalDataEffects = new List<PhysicalPenetrationAdditionalDataEffect>();
        [NonSerialized] public List<PhysicalPenetrationIncreaseDataEffect>   PhysicalPenetrationIncreaseDataEffects = new List<PhysicalPenetrationIncreaseDataEffect>();
        [NonSerialized] public List<PhysicalPenetrationMultiplierDataEffect> PhysicalPenetrationMultiplierDataEffects = new List<PhysicalPenetrationMultiplierDataEffect>();
        
        [NonSerialized] public List<PhysicalResistanceAdditionalDataEffect> PhysicalResistanceAdditionalDataEffects = new List<PhysicalResistanceAdditionalDataEffect>();
        [NonSerialized] public List<PhysicalResistanceIncreaseDataEffect>   PhysicalResistanceIncreaseDataEffects = new List<PhysicalResistanceIncreaseDataEffect>();
        [NonSerialized] public List<PhysicalResistanceMultiplierDataEffect> PhysicalResistanceMultiplierDataEffects = new List<PhysicalResistanceMultiplierDataEffect>();
        
        [NonSerialized] public List<MagicPenetrationAdditionalDataEffect> MagicPenetrationAdditionalDataEffects = new List<MagicPenetrationAdditionalDataEffect>();
        [NonSerialized] public List<MagicPenetrationIncreaseDataEffect>   MagicPenetrationIncreaseDataEffects = new List<MagicPenetrationIncreaseDataEffect>();
        [NonSerialized] public List<MagicPenetrationMultiplierDataEffect> MagicPenetrationMultiplierDataEffects = new List<MagicPenetrationMultiplierDataEffect>();
        
        [NonSerialized] public List<MagicResistanceAdditionalDataEffect> MagicResistanceAdditionalDataEffects = new List<MagicResistanceAdditionalDataEffect>();
        [NonSerialized] public List<MagicResistanceIncreaseDataEffect>   MagicResistanceIncreaseDataEffects = new List<MagicResistanceIncreaseDataEffect>();
        [NonSerialized] public List<MagicResistanceMultiplierDataEffect> MagicResistanceMultiplierDataEffects = new List<MagicResistanceMultiplierDataEffect>();
        
        [NonSerialized] public List<DamageAdditionalDataEffect> DamageAdditionalDataEffects = new List<DamageAdditionalDataEffect>();
        [NonSerialized] public List<DamageIncreaseDataEffect>   DamageIncreaseDataEffects = new List<DamageIncreaseDataEffect>();
        [NonSerialized] public List<DamageMultiplierDataEffect> DamageMultiplierDataEffects = new List<DamageMultiplierDataEffect>();
        
        [NonSerialized] public List<ReceiveDamageAdditionalDataEffect> ReceiveDamageAdditionalDataEffects = new List<ReceiveDamageAdditionalDataEffect>();
        [NonSerialized] public List<ReceiveDamageIncreaseDataEffect>   ReceiveDamageIncreaseDataEffects = new List<ReceiveDamageIncreaseDataEffect>();
        [NonSerialized] public List<ReceiveDamageMultiplierDataEffect> ReceiveDamageMultiplierDataEffects = new List<ReceiveDamageMultiplierDataEffect>();

        [NonSerialized] public List<CriticalHitChanceAdditionalDataEffect> CriticalHitChanceAdditionalDataEffects = new List<CriticalHitChanceAdditionalDataEffect>();
        [NonSerialized] public List<CriticalHitDamageAdditionalDataEffect> CriticalHitDamageAdditionalDataEffects = new List<CriticalHitDamageAdditionalDataEffect>();
        [NonSerialized] public List<CriticalHitDamageIncreaseDataEffect>   CriticalHitDamageIncreaseDataEffects = new List<CriticalHitDamageIncreaseDataEffect>();
        [NonSerialized] public List<CriticalHitDamageMultiplierDataEffect> CriticalHitDamageMultiplierDataEffects = new List<CriticalHitDamageMultiplierDataEffect>();

        [NonSerialized] public List<MaxHPAdditionalDataEffect> MaxHPAdditionalDataEffects = new List<MaxHPAdditionalDataEffect>();
        [NonSerialized] public List<MaxHPIncreaseDataEffect>   MaxHPIncreaseDataEffects = new List<MaxHPIncreaseDataEffect>();
        [NonSerialized] public List<MaxHPMultiplierDataEffect> MaxHPMultiplierDataEffects = new List<MaxHPMultiplierDataEffect>();

        [NonSerialized] public List<HealingAdditionalDataEffect> HealingAdditionalDataEffects = new List<HealingAdditionalDataEffect>();
        [NonSerialized] public List<HealingIncreaseDataEffect>   HealingIncreaseDataEffects = new List<HealingIncreaseDataEffect>();
        [NonSerialized] public List<HealingMultiplierDataEffect> HealingMultiplierDataEffects = new List<HealingMultiplierDataEffect>();

        [NonSerialized] public List<ManaRecoveryPerSecAdditionalDataEffect> ManaRecoveryPerSecAdditionalDataEffects = new List<ManaRecoveryPerSecAdditionalDataEffect>();
        [NonSerialized] public List<ManaRecoveryPerSecIncreaseDataEffect> ManaRecoveryPerSecIncreaseDataEffects = new List<ManaRecoveryPerSecIncreaseDataEffect>();
        [NonSerialized] public List<ManaRecoveryPerSecMultiplierDataEffect> ManaRecoveryPerSecMultiplierDataEffects = new List<ManaRecoveryPerSecMultiplierDataEffect>();

        [NonSerialized] public List<SetMinHPEffect> SetMinHPEffects = new List<SetMinHPEffect>();
        [NonSerialized] public List<SetAttackTypeEffect> SetAttackTypeEffects = new List<SetAttackTypeEffect>();
        [NonSerialized] public List<SetDamageTypeEffect> SetDamageTypeEffects = new List<SetDamageTypeEffect>();

        [NonSerialized] public List<UnableToTargetOfAttackEffect> UnableToTargetOfAttackEffects = new List<UnableToTargetOfAttackEffect>();
        #endregion

        private Dictionary<BuffTemplate, StatusInstance> statusDic = new Dictionary<BuffTemplate, StatusInstance>();

#if UNITY_EDITOR
        [SerializeField, ReadOnly] private List<BuffTemplate> statusList = new List<BuffTemplate>();
#endif

        internal override void Initialize(Unit unit)
        {
            base.Initialize(unit);

            unit.GetAbility<AttackAbility>().onAttack += RemoveStatusByAttack;
            unit.GetAbility<HealthAbility>().onDeath += ClearStatusEffects;
        }

        internal void DeInitialize()
        {
            unit.GetAbility<AttackAbility>().onAttack -= RemoveStatusByAttack;
            unit.GetAbility<HealthAbility>().onDeath -= ClearStatusEffects;
        }

        internal void ApplyBuff(BuffTemplate template, float duration)
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

                    if (template.useAttackCountLimit)
                    {
                        instance.useCountLimit = true;
                        instance.count = template.attackCount;
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

        private IEnumerator CoAddStatus(BuffTemplate template, float duration, bool isContained)
        {
            yield return new WaitForSeconds(template.delay);
            AddStatus(template, duration, isContained);
        }

        /// <summary>
        /// 버프 추가
        /// </summary>
        private void AddStatus(BuffTemplate template, float duration, bool isContained)
        {
            StatusInstance statusInstance = new StatusInstance(duration, Time.time);
            
            // 무한지속이 아니라면
            if (duration != int.MaxValue)
            {
                var corutine = StartCoroutine(CoStatus(statusInstance, template));
                statusInstance.corutine = corutine;
            }
            
            // 공격시 상태이상이 해제되야 한다면
            if (template.useAttackCountLimit)
            {
                statusInstance.useCountLimit = true;
                statusInstance.count = template.attackCount;
            }

            statusDic.Add(template, statusInstance);

#if UNITY_EDITOR
            statusList.Add(template);
#endif

            // 버프 효과 적용 (동일한 버프 효과는 중복되지 않음)
            if (isContained == false)
            {
                foreach (var effect in template.effects)
                {
                    if (effect is MoveIncreaseDataEffect moveIncreaseDataEffect)
                    {
                        MoveIncreaseDataEffects.Add(moveIncreaseDataEffect);
                    }
                    else if (effect is MoveMultiplierDataEffect moveMultiplierDataEffect)
                    {
                        MoveMultiplierDataEffects.Add(moveMultiplierDataEffect);
                    }

                    else if (effect is ATKAdditionalDataEffect atkAdditionalDataEffects)
                    {
                        ATKAdditionalDataEffects.Add(atkAdditionalDataEffects);
                    }
                    else if (effect is ATKIncreaseDataEffect atkIncreaseDataEffect)
                    {
                        ATKIncreaseDataEffects.Add(atkIncreaseDataEffect);
                    }
                    else if (effect is ATKMultiplierDataEffect atkMultiplierDataEffect)
                    {
                        ATKMultiplierDataEffects.Add(atkMultiplierDataEffect);
                    }

                    else if (effect is AttackCountAdditionalDataEffect attackCountAdditionalDataEffect)
                    {
                        AttackCountAdditionalDataEffects.Add(attackCountAdditionalDataEffect);
                    }

                    else if (effect is AttackSpeedIncreaseDataEffect AttackSpeedIncreaseDataEffect)
                    {
                        AttackSpeedIncreaseDataEffects.Add(AttackSpeedIncreaseDataEffect);
                    }
                    else if (effect is AttackSpeedMultiplierDataEffect AttackSpeedMultiplierDataEffect)
                    {
                        AttackSpeedMultiplierDataEffects.Add(AttackSpeedMultiplierDataEffect);
                    }

                    else if (effect is AvoidanceAdditionalDataEffect AvoidanceAdditionalDataEffect)
                    {
                        AvoidanceAdditionalDataEffects.Add(AvoidanceAdditionalDataEffect);
                    }

                    else if (effect is PhysicalPenetrationAdditionalDataEffect physicalPenetrationAdditionalDataEffect)
                    {
                        PhysicalPenetrationAdditionalDataEffects.Add(physicalPenetrationAdditionalDataEffect);
                    }
                    else if (effect is PhysicalPenetrationIncreaseDataEffect physicalPenetrationIncreaseDataEffect)
                    {
                        PhysicalPenetrationIncreaseDataEffects.Add(physicalPenetrationIncreaseDataEffect);
                    }
                    else if (effect is PhysicalPenetrationMultiplierDataEffect physicalPenetrationMultiplierDataEffect)
                    {
                        PhysicalPenetrationMultiplierDataEffects.Add(physicalPenetrationMultiplierDataEffect);
                    }

                    else if (effect is PhysicalResistanceAdditionalDataEffect physicalResistanceAdditionalDataEffect)
                    {
                        PhysicalResistanceAdditionalDataEffects.Add(physicalResistanceAdditionalDataEffect);
                    }
                    else if (effect is PhysicalResistanceIncreaseDataEffect physicalResistanceIncreaseDataEffect)
                    {
                        PhysicalResistanceIncreaseDataEffects.Add(physicalResistanceIncreaseDataEffect);
                    }
                    else if (effect is PhysicalResistanceMultiplierDataEffect physicalResistanceMultiplierDataEffect)
                    {
                        PhysicalResistanceMultiplierDataEffects.Add(physicalResistanceMultiplierDataEffect);
                    }

                    else if (effect is MagicPenetrationAdditionalDataEffect magicPenetrationAdditionalDataEffect)
                    {
                        MagicPenetrationAdditionalDataEffects.Add(magicPenetrationAdditionalDataEffect);
                    }
                    else if (effect is MagicPenetrationIncreaseDataEffect magicPenetrationIncreaseDataEffect)
                    {
                        MagicPenetrationIncreaseDataEffects.Add(magicPenetrationIncreaseDataEffect);
                    }
                    else if (effect is MagicPenetrationMultiplierDataEffect magicPenetrationMultiplierDataEffect)
                    {
                        MagicPenetrationMultiplierDataEffects.Add(magicPenetrationMultiplierDataEffect);
                    }

                    else if (effect is MagicResistanceAdditionalDataEffect magicResistanceAdditionalDataEffect)
                    {
                        MagicResistanceAdditionalDataEffects.Add(magicResistanceAdditionalDataEffect);
                    }
                    else if (effect is MagicResistanceIncreaseDataEffect magicResistanceIncreaseDataEffect)
                    {
                        MagicResistanceIncreaseDataEffects.Add(magicResistanceIncreaseDataEffect);
                    }
                    else if (effect is MagicResistanceMultiplierDataEffect magicResistanceMultiplierDataEffect)
                    {
                        MagicResistanceMultiplierDataEffects.Add(magicResistanceMultiplierDataEffect);
                    }

                    else if (effect is DamageAdditionalDataEffect damageAdditionalDataEffect)
                    {
                        DamageAdditionalDataEffects.Add(damageAdditionalDataEffect);
                    }
                    else if (effect is DamageIncreaseDataEffect damageIncreaseDataEffect)
                    {
                        DamageIncreaseDataEffects.Add(damageIncreaseDataEffect);
                    }
                    else if (effect is DamageMultiplierDataEffect damageMultiplierDataEffect)
                    {
                        DamageMultiplierDataEffects.Add(damageMultiplierDataEffect);
                    }

                    else if (effect is ReceiveDamageAdditionalDataEffect receiveDamageAdditionalDataEffect)
                    {
                        ReceiveDamageAdditionalDataEffects.Add(receiveDamageAdditionalDataEffect);
                    }
                    else if (effect is ReceiveDamageIncreaseDataEffect receiveDamageIncreaseDataEffect)
                    {
                        ReceiveDamageIncreaseDataEffects.Add(receiveDamageIncreaseDataEffect);
                    }
                    else if (effect is ReceiveDamageMultiplierDataEffect receiveDamageMultiplierDataEffect)
                    {
                        ReceiveDamageMultiplierDataEffects.Add(receiveDamageMultiplierDataEffect);
                    }

                    else if (effect is CriticalHitChanceAdditionalDataEffect criticalHitChanceAdditionalDataEffect)
                    {
                        CriticalHitChanceAdditionalDataEffects.Add(criticalHitChanceAdditionalDataEffect);
                    }
                    else if (effect is CriticalHitDamageAdditionalDataEffect criticalHitDamageAdditionalDataEffect)
                    {
                        CriticalHitDamageAdditionalDataEffects.Add(criticalHitDamageAdditionalDataEffect);
                    }
                    else if (effect is CriticalHitDamageIncreaseDataEffect criticalHitDamageIncreaseDataEffect)
                    {
                        CriticalHitDamageIncreaseDataEffects.Add(criticalHitDamageIncreaseDataEffect);
                    }
                    else if (effect is CriticalHitDamageMultiplierDataEffect criticalHitDamageMultiplierDataEffect)
                    {
                        CriticalHitDamageMultiplierDataEffects.Add(criticalHitDamageMultiplierDataEffect);
                    }

                    else if (effect is MaxHPAdditionalDataEffect maxHPAdditionalDataEffect)
                    {
                        MaxHPAdditionalDataEffects.Add(maxHPAdditionalDataEffect);
                    }
                    else if (effect is MaxHPIncreaseDataEffect maxHPIncreaseDataEffect)
                    {
                        MaxHPIncreaseDataEffects.Add(maxHPIncreaseDataEffect);
                    }
                    else if (effect is MaxHPMultiplierDataEffect maxHPMultiplierDataEffect)
                    {
                        MaxHPMultiplierDataEffects.Add(maxHPMultiplierDataEffect);
                    }

                    else if (effect is HealingAdditionalDataEffect healingAdditionalDataEffect)
                    {
                        HealingAdditionalDataEffects.Add(healingAdditionalDataEffect);
                    }
                    else if (effect is HealingIncreaseDataEffect healingIncreaseDataEffect)
                    {
                        HealingIncreaseDataEffects.Add(healingIncreaseDataEffect);
                    }
                    else if (effect is HealingMultiplierDataEffect healingMultiplierDataEffect)
                    {
                        HealingMultiplierDataEffects.Add(healingMultiplierDataEffect);
                    }
                    
                    else if (effect is ManaRecoveryPerSecAdditionalDataEffect manaRecoveryPerSecAdditionalDataEffect)
                    {
                        ManaRecoveryPerSecAdditionalDataEffects.Add(manaRecoveryPerSecAdditionalDataEffect);
                    }
                    else if (effect is ManaRecoveryPerSecIncreaseDataEffect manaRecoveryPerSecIncreaseDataEffect)
                    {
                        ManaRecoveryPerSecIncreaseDataEffects.Add(manaRecoveryPerSecIncreaseDataEffect);
                    }
                    else if (effect is ManaRecoveryPerSecMultiplierDataEffect manaRecoveryPerSecMultiplierDataEffect)
                    {
                        ManaRecoveryPerSecMultiplierDataEffects.Add(manaRecoveryPerSecMultiplierDataEffect);
                    }

                    else if (effect is SetMinHPEffect setMinHPEffect)
                    {
                        SetMinHPEffects.Add(setMinHPEffect);
                    }
                    else if (effect is SetAttackTypeEffect setAttackTypeEffect)
                    {
                        SetAttackTypeEffects.Add(setAttackTypeEffect);
                    }
                    else if (effect is SetDamageTypeEffect setDamageTypeEffect)
                    {
                        SetDamageTypeEffects.Add(setDamageTypeEffect);
                    }

                    else if (effect is UnableToTargetOfAttackEffect unableToTargetOfAttackEffect)
                    {
                        UnableToTargetOfAttackEffects.Add(unableToTargetOfAttackEffect);
                    }
                }
            }
        }

        private IEnumerator CoStatus(StatusInstance statusInstance, BuffTemplate template)
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
        private void RemoveStatusByAttack()
        {
            List<BuffTemplate> templates = new List<BuffTemplate>();
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
                if (effect is MoveIncreaseDataEffect moveIncreaseDataEffect)
                {
                    MoveIncreaseDataEffects.Remove(moveIncreaseDataEffect);
                }
                else if (effect is MoveMultiplierDataEffect moveMultiplierDataEffect)
                {
                    MoveMultiplierDataEffects.Remove(moveMultiplierDataEffect);
                }

                else if (effect is ATKAdditionalDataEffect atkAdditionalDataEffects)
                {
                    ATKAdditionalDataEffects.Remove(atkAdditionalDataEffects);
                }
                else if (effect is ATKIncreaseDataEffect atkIncreaseDataEffect)
                {
                    ATKIncreaseDataEffects.Remove(atkIncreaseDataEffect);
                }
                else if (effect is ATKMultiplierDataEffect atkMultiplierDataEffect)
                {
                    ATKMultiplierDataEffects.Remove(atkMultiplierDataEffect);
                }

                else if (effect is AttackCountAdditionalDataEffect attackCountAdditionalDataEffect)
                {
                    AttackCountAdditionalDataEffects.Remove(attackCountAdditionalDataEffect);
                }

                else if (effect is AttackSpeedIncreaseDataEffect AttackSpeedIncreaseDataEffect)
                {
                    AttackSpeedIncreaseDataEffects.Remove(AttackSpeedIncreaseDataEffect);
                }
                else if (effect is AttackSpeedMultiplierDataEffect AttackSpeedMultiplierDataEffect)
                {
                    AttackSpeedMultiplierDataEffects.Remove(AttackSpeedMultiplierDataEffect);
                }

                else if (effect is AvoidanceAdditionalDataEffect AvoidanceAdditionalDataEffect)
                {
                    AvoidanceAdditionalDataEffects.Remove(AvoidanceAdditionalDataEffect);
                }

                else if (effect is PhysicalPenetrationAdditionalDataEffect physicalPenetrationAdditionalDataEffect)
                {
                    PhysicalPenetrationAdditionalDataEffects.Remove(physicalPenetrationAdditionalDataEffect);
                }
                else if (effect is PhysicalPenetrationIncreaseDataEffect physicalPenetrationIncreaseDataEffect)
                {
                    PhysicalPenetrationIncreaseDataEffects.Remove(physicalPenetrationIncreaseDataEffect);
                }
                else if (effect is PhysicalPenetrationMultiplierDataEffect physicalPenetrationMultiplierDataEffect)
                {
                    PhysicalPenetrationMultiplierDataEffects.Remove(physicalPenetrationMultiplierDataEffect);
                }

                else if (effect is PhysicalResistanceAdditionalDataEffect physicalResistanceAdditionalDataEffect)
                {
                    PhysicalResistanceAdditionalDataEffects.Remove(physicalResistanceAdditionalDataEffect);
                }
                else if (effect is PhysicalResistanceIncreaseDataEffect physicalResistanceIncreaseDataEffect)
                {
                    PhysicalResistanceIncreaseDataEffects.Remove(physicalResistanceIncreaseDataEffect);
                }
                else if (effect is PhysicalResistanceMultiplierDataEffect physicalResistanceMultiplierDataEffect)
                {
                    PhysicalResistanceMultiplierDataEffects.Remove(physicalResistanceMultiplierDataEffect);
                }

                else if (effect is MagicPenetrationAdditionalDataEffect magicPenetrationAdditionalDataEffect)
                {
                    MagicPenetrationAdditionalDataEffects.Remove(magicPenetrationAdditionalDataEffect);
                }
                else if (effect is MagicPenetrationIncreaseDataEffect magicPenetrationIncreaseDataEffect)
                {
                    MagicPenetrationIncreaseDataEffects.Remove(magicPenetrationIncreaseDataEffect);
                }
                else if (effect is MagicPenetrationMultiplierDataEffect magicPenetrationMultiplierDataEffect)
                {
                    MagicPenetrationMultiplierDataEffects.Remove(magicPenetrationMultiplierDataEffect);
                }

                else if (effect is MagicResistanceAdditionalDataEffect magicResistanceAdditionalDataEffect)
                {
                    MagicResistanceAdditionalDataEffects.Remove(magicResistanceAdditionalDataEffect);
                }
                else if (effect is MagicResistanceIncreaseDataEffect magicResistanceIncreaseDataEffect)
                {
                    MagicResistanceIncreaseDataEffects.Remove(magicResistanceIncreaseDataEffect);
                }
                else if (effect is MagicResistanceMultiplierDataEffect magicResistanceMultiplierDataEffect)
                {
                    MagicResistanceMultiplierDataEffects.Remove(magicResistanceMultiplierDataEffect);
                }

                else if (effect is DamageAdditionalDataEffect damageAdditionalDataEffect)
                {
                    DamageAdditionalDataEffects.Remove(damageAdditionalDataEffect);
                }
                else if (effect is DamageIncreaseDataEffect damageIncreaseDataEffect)
                {
                    DamageIncreaseDataEffects.Remove(damageIncreaseDataEffect);
                }
                else if (effect is DamageMultiplierDataEffect damageMultiplierDataEffect)
                {
                    DamageMultiplierDataEffects.Remove(damageMultiplierDataEffect);
                }

                else if (effect is ReceiveDamageAdditionalDataEffect receiveDamageAdditionalDataEffect)
                {
                    ReceiveDamageAdditionalDataEffects.Remove(receiveDamageAdditionalDataEffect);
                }
                else if (effect is ReceiveDamageIncreaseDataEffect receiveDamageIncreaseDataEffect)
                {
                    ReceiveDamageIncreaseDataEffects.Remove(receiveDamageIncreaseDataEffect);
                }
                else if (effect is ReceiveDamageMultiplierDataEffect receiveDamageMultiplierDataEffect)
                {
                    ReceiveDamageMultiplierDataEffects.Remove(receiveDamageMultiplierDataEffect);
                }

                else if (effect is CriticalHitChanceAdditionalDataEffect criticalHitChanceAdditionalDataEffect)
                {
                    CriticalHitChanceAdditionalDataEffects.Remove(criticalHitChanceAdditionalDataEffect);
                }
                else if (effect is CriticalHitDamageAdditionalDataEffect criticalHitDamageAdditionalDataEffect)
                {
                    CriticalHitDamageAdditionalDataEffects.Remove(criticalHitDamageAdditionalDataEffect);
                }
                else if (effect is CriticalHitDamageIncreaseDataEffect criticalHitDamageIncreaseDataEffect)
                {
                    CriticalHitDamageIncreaseDataEffects.Remove(criticalHitDamageIncreaseDataEffect);
                }
                else if (effect is CriticalHitDamageMultiplierDataEffect criticalHitDamageMultiplierDataEffect)
                {
                    CriticalHitDamageMultiplierDataEffects.Remove(criticalHitDamageMultiplierDataEffect);
                }

                else if (effect is MaxHPAdditionalDataEffect maxHPAdditionalDataEffect)
                {
                    MaxHPAdditionalDataEffects.Remove(maxHPAdditionalDataEffect);
                }
                else if (effect is MaxHPIncreaseDataEffect maxHPIncreaseDataEffect)
                {
                    MaxHPIncreaseDataEffects.Remove(maxHPIncreaseDataEffect);
                }
                else if (effect is MaxHPMultiplierDataEffect maxHPMultiplierDataEffect)
                {
                    MaxHPMultiplierDataEffects.Remove(maxHPMultiplierDataEffect);
                }

                else if (effect is HealingAdditionalDataEffect healingAdditionalDataEffect)
                {
                    HealingAdditionalDataEffects.Remove(healingAdditionalDataEffect);
                }
                else if (effect is HealingIncreaseDataEffect healingIncreaseDataEffect)
                {
                    HealingIncreaseDataEffects.Remove(healingIncreaseDataEffect);
                }
                else if (effect is HealingMultiplierDataEffect healingMultiplierDataEffect)
                {
                    HealingMultiplierDataEffects.Remove(healingMultiplierDataEffect);
                }

                else if (effect is ManaRecoveryPerSecAdditionalDataEffect manaRecoveryPerSecAdditionalDataEffect)
                {
                    ManaRecoveryPerSecAdditionalDataEffects.Remove(manaRecoveryPerSecAdditionalDataEffect);
                }
                else if (effect is ManaRecoveryPerSecIncreaseDataEffect manaRecoveryPerSecIncreaseDataEffect)
                {
                    ManaRecoveryPerSecIncreaseDataEffects.Remove(manaRecoveryPerSecIncreaseDataEffect);
                }
                else if (effect is ManaRecoveryPerSecMultiplierDataEffect manaRecoveryPerSecMultiplierDataEffect)
                {
                    ManaRecoveryPerSecMultiplierDataEffects.Remove(manaRecoveryPerSecMultiplierDataEffect);
                }

                else if (effect is SetMinHPEffect setMinHPEffect)
                {
                    SetMinHPEffects.Remove(setMinHPEffect);
                }
                else if (effect is SetAttackTypeEffect setAttackTypeEffect)
                {
                    SetAttackTypeEffects.Remove(setAttackTypeEffect);
                }
                else if (effect is SetDamageTypeEffect setDamageTypeEffect)
                {
                    SetDamageTypeEffects.Remove(setDamageTypeEffect);
                }

                else if (effect is UnableToTargetOfAttackEffect unableToTargetOfAttackEffect)
                {
                    UnableToTargetOfAttackEffects.Remove(unableToTargetOfAttackEffect);
                }
            }
        }

        #region 유틸리티 메서드
        internal bool Contains(BuffTemplate template)
        {
            return statusDic.ContainsKey(template);
        }

        internal bool Contains(List<BuffTemplate> templates)
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