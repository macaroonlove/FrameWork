using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Temporary.Core
{
    public class DamageCalculateAbility : AlwaysAbility
    {
        private EDamageType _baseDamageType;
        private int _basePhysicalResistance;
        private int _baseMagicResistance;
        private int _basePhysicalPenetration;
        private int _baseMagicPenetration;
        private float _baseCriticalHitChance;
        private float _baseCriticalHitDamage;

        #region 계산 스탯
        #region 데미지 타입
        internal EDamageType finalDamageType
        {
            get
            {
                EDamageType result = _baseDamageType;

                return result;
            }
        }
        #endregion

        #region 데미지 증감 & 상승·하락
        private float finalDamageIncrease
        {
            get
            {
                float result = 1;

                return result;
            }
        }

        private float finalDamageMultiplier
        {
            get
            {
                float result = 1;

                return result;
            }
        }
        #endregion

        #region 저항력
        private int finalPhysicalResistance
        {
            get
            {
                int result = _basePhysicalResistance;

                return result;
            }
        }

        private int finalMagicResistance
        {
            get
            {
                int result = _baseMagicResistance;

                return result;
            }
        }
        #endregion

        #region 관통력
        private int finalPhysicalPenetration
        {
            get
            {
                int result = _basePhysicalPenetration;

                return result;
            }
        }

        private int finalMagicPenetration
        {
            get
            {
                int result = _baseMagicPenetration;

                return result;
            }
        }
        #endregion

        #region 치명타
        private bool finalIsCriticalHit
        {
            get
            {
                float chance = _baseCriticalHitChance;

                if (chance > 0)
                {
                    return Random.value < chance;
                }
                else
                {
                    return false;
                }
            }
        }

        private float finalCriticalHitDamage
        {
            get
            {
                float result = _baseCriticalHitDamage;

                return result;
            }
        }
        #endregion
        #endregion

        internal override void Initialize(Unit unit)
        {
            base.Initialize(unit);

            if (unit is AgentUnit agentUnit)
            {
                _baseDamageType = agentUnit.template.DamageType;
                _basePhysicalResistance = agentUnit.template.PhysicalResistance;
                _baseMagicResistance = agentUnit.template.MagicResistance;
                _basePhysicalPenetration = agentUnit.template.PhysicalPenetration;
                _baseMagicPenetration = agentUnit.template.MagicPenetration;
                _baseCriticalHitChance = agentUnit.template.CriticalHitChance;
                _baseCriticalHitDamage = agentUnit.template.CriticalHitDamage;
            }
            else if (unit is EnemyUnit enemyUnit)
            {
                _baseDamageType = enemyUnit.template.DamageType;
                _basePhysicalResistance = enemyUnit.template.PhysicalResistance;
                _baseMagicResistance = enemyUnit.template.MagicResistance;
                _basePhysicalPenetration = enemyUnit.template.PhysicalPenetration;
                _baseMagicPenetration = enemyUnit.template.MagicPenetration;
                _baseCriticalHitChance = enemyUnit.template.CriticalHitChance;
                _baseCriticalHitDamage = enemyUnit.template.CriticalHitDamage;
            }
        }

        /// <summary>
        /// 유닛 기본 공격에 의한 피해일 때, 데미지 계산
        /// </summary>
        internal int GetDamage(Unit attackedUnit, EDamageType damageType)
        {
            int finalATK = attackedUnit.GetAbility<AttackAbility>().finalATK;

            // 저항력 & 관통력
            float finalDamage = GetDamageByDamageType(attackedUnit, finalATK, damageType);

            // 데미지 증감 & 상승·하락
            var attackedUnitOfDamageCalculateAbility = attackedUnit.GetAbility<DamageCalculateAbility>();
            finalDamage *= attackedUnitOfDamageCalculateAbility.finalDamageIncrease;
            finalDamage *= attackedUnitOfDamageCalculateAbility.finalDamageMultiplier;

            // 치명타가 터졌다면
            if (attackedUnitOfDamageCalculateAbility.finalIsCriticalHit)
            {
                // 치명타 데미지
                finalDamage *= attackedUnitOfDamageCalculateAbility.finalCriticalHitDamage;
            }

            return (int)finalDamage;
        }

        /// <summary>
        /// 유닛 스킬 공격에 의한 피해일 때, 데미지 계산
        /// (기본 데미지가 이미 정해져 있음)
        /// </summary>
        internal int GetDamage(Unit attackedUnit, int damage, EDamageType damageType)
        {
            // 저항력 & 관통력
            float finalDamage = GetDamageByDamageType(attackedUnit, damage, damageType);

            // 데미지 증감 & 상승·하락
            var attackedUnitOfDamageCalculateAbility = attackedUnit.GetAbility<DamageCalculateAbility>();
            finalDamage *= attackedUnitOfDamageCalculateAbility.finalDamageIncrease;
            finalDamage *= attackedUnitOfDamageCalculateAbility.finalDamageMultiplier;

            // 치명타가 터졌다면
            if (attackedUnitOfDamageCalculateAbility.finalIsCriticalHit)
            {
                // 치명타 데미지
                finalDamage *= attackedUnitOfDamageCalculateAbility.finalCriticalHitDamage;
            }

            return (int)finalDamage;
        }

        /// <summary>
        /// 아이템, 유물 등의 공격에 의한 피해일 때, 데미지 계산
        /// </summary>
        internal int GetDamage(int damage, EDamageType damageType)
        {
            int finalDamage = GetDamageByResistance(damage, damageType);

            return finalDamage;
        }

        #region 저항력·관통력
        /// <summary>
        /// 저항력만 적용
        /// </summary>
        private int GetDamageByResistance(int finalATK, EDamageType damageType)
        {
            int damage = finalATK;

            switch (damageType)
            {
                case EDamageType.PhysicalDamage:
                    damage = (int)(finalATK * (100 - finalPhysicalResistance) * 0.01f);
                    break;
                case EDamageType.MagicDamage:
                    damage = (int)(finalATK * (100 - finalMagicResistance) * 0.01f);
                    break;
            }

            return damage;
        }

        /// <summary>
        /// 저항력과 관통력 모두 적용
        /// </summary>
        private int GetDamageByDamageType(Unit attackedUnit, int finalATK, EDamageType damageType)
        {
            int damage = finalATK;

            switch (damageType)
            {
                case EDamageType.PhysicalDamage:
                    int finalPhysicalPenetration = attackedUnit.GetAbility<DamageCalculateAbility>().finalPhysicalPenetration;
                    damage = (int)(finalATK * (100 - (finalPhysicalResistance - finalPhysicalPenetration)) * 0.01f);
                    break;
                case EDamageType.MagicDamage:
                    int finalMagicPenetration = attackedUnit.GetAbility<DamageCalculateAbility>().finalMagicPenetration;
                    damage = (int)(finalATK * (100 - (finalMagicResistance - finalMagicPenetration)) * 0.01f);
                    break;
            }

            return damage;
        }
        #endregion
    }
}
