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

        #region ��� ����
        #region ������ Ÿ��
        internal EDamageType finalDamageType
        {
            get
            {
                EDamageType result = _baseDamageType;

                return result;
            }
        }
        #endregion

        #region ������ ���� & ��¡��϶�
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

        #region ���׷�
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

        #region �����
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

        #region ġ��Ÿ
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
        /// ���� �⺻ ���ݿ� ���� ������ ��, ������ ���
        /// </summary>
        internal int GetDamage(Unit attackedUnit, EDamageType damageType)
        {
            int finalATK = attackedUnit.GetAbility<AttackAbility>().finalATK;

            // ���׷� & �����
            float finalDamage = GetDamageByDamageType(attackedUnit, finalATK, damageType);

            // ������ ���� & ��¡��϶�
            var attackedUnitOfDamageCalculateAbility = attackedUnit.GetAbility<DamageCalculateAbility>();
            finalDamage *= attackedUnitOfDamageCalculateAbility.finalDamageIncrease;
            finalDamage *= attackedUnitOfDamageCalculateAbility.finalDamageMultiplier;

            // ġ��Ÿ�� �����ٸ�
            if (attackedUnitOfDamageCalculateAbility.finalIsCriticalHit)
            {
                // ġ��Ÿ ������
                finalDamage *= attackedUnitOfDamageCalculateAbility.finalCriticalHitDamage;
            }

            return (int)finalDamage;
        }

        /// <summary>
        /// ���� ��ų ���ݿ� ���� ������ ��, ������ ���
        /// (�⺻ �������� �̹� ������ ����)
        /// </summary>
        internal int GetDamage(Unit attackedUnit, int damage, EDamageType damageType)
        {
            // ���׷� & �����
            float finalDamage = GetDamageByDamageType(attackedUnit, damage, damageType);

            // ������ ���� & ��¡��϶�
            var attackedUnitOfDamageCalculateAbility = attackedUnit.GetAbility<DamageCalculateAbility>();
            finalDamage *= attackedUnitOfDamageCalculateAbility.finalDamageIncrease;
            finalDamage *= attackedUnitOfDamageCalculateAbility.finalDamageMultiplier;

            // ġ��Ÿ�� �����ٸ�
            if (attackedUnitOfDamageCalculateAbility.finalIsCriticalHit)
            {
                // ġ��Ÿ ������
                finalDamage *= attackedUnitOfDamageCalculateAbility.finalCriticalHitDamage;
            }

            return (int)finalDamage;
        }

        /// <summary>
        /// ������, ���� ���� ���ݿ� ���� ������ ��, ������ ���
        /// </summary>
        internal int GetDamage(int damage, EDamageType damageType)
        {
            int finalDamage = GetDamageByResistance(damage, damageType);

            return finalDamage;
        }

        #region ���׷¡������
        /// <summary>
        /// ���׷¸� ����
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
        /// ���׷°� ����� ��� ����
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
