using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Temporary.Core
{
    public class HitAbility : AlwaysAbility
    {
        private DamageCalculateAbility _damageCalculateAbility;
        private HealthAbility _healthAbility;

        #region ���� �鿪 �ʵ�
        private class DamageImmunityInstance
        {
            public Coroutine coroutine;
            public int id;
            public int count;
            public float duration;
            public EDamageType damageType;

            public DamageImmunityInstance(EDamageType damageType, int count)
            {
                this.damageType = damageType;
                this.count = count;
            }

            public DamageImmunityInstance(EDamageType damageType, int id, Coroutine coroutine, float duration)
            {
                this.damageType = damageType;
                this.id = id;
                this.coroutine = coroutine;
                this.duration = duration;
            }

            public DamageImmunityInstance(EDamageType damageType, int id, Coroutine coroutine, float duration, int count)
            {
                this.damageType = damageType;
                this.id = id;
                this.coroutine = coroutine;
                this.duration = duration;
                this.count = count;
            }
        }

        private List<DamageImmunityInstance> _damageImmunities = new List<DamageImmunityInstance>();
        private int _damageImmunityIdCounter = 0;

        internal int _damageImmunityCount => _damageImmunities.Count;
        #endregion

        #region ���� ���
        /// <summary>
        /// ���� ������� (���� ���� ����)
        /// </summary>
        internal bool finalTargetOfAttack
        {
            get
            {
                bool result = true;

                return result;
            }
        }

        private bool finalIsAvoidance
        {
            get
            {
                float avoidance = 0;



                if (avoidance > 0)
                {
                    return Random.value < avoidance;
                }
                else
                {
                    return false;
                }
            }
        }
        #endregion

        internal override void Initialize(Unit unit)
        {
            base.Initialize(unit);

            _damageCalculateAbility = unit.GetAbility<DamageCalculateAbility>();
            _healthAbility = unit.GetAbility<HealthAbility>();
        }

        internal override void Deinitialize()
        {
            _damageCalculateAbility = null;
            _healthAbility = null;
        }

        /// <summary>
        /// ������ �⺻ ������ ���, �ǰ� �޼���
        /// </summary>
        internal void Hit(Unit attackedUnit)
        {
            if (finalTargetOfAttack == false) return;

            if (finalIsAvoidance)
            {
                // �̽���� �˾� ǥ��
            }
            else
            {
                EDamageType damageType = attackedUnit.GetAbility<DamageCalculateAbility>().finalDamageType;

                // ���� �鿪 ����
                if (UsedDamageImmunity(damageType)) return;

                int damage = _damageCalculateAbility.GetDamage(attackedUnit, damageType);
                _healthAbility.Damaged(damage, attackedUnit.id);
            }
        }

        /// <summary>
        /// ������ Ÿ�Կ� ���� ����
        /// </summary>
        internal void Hit(int damage, EDamageType damageType, int id = 0)
        {
            // ���� �鿪 ����
            if (UsedDamageImmunity(damageType)) return;
            damage = _damageCalculateAbility.GetDamage(damage, damageType);
            _healthAbility.Damaged(damage, id);
        }

        /// <summary>
        /// ���� ���� ����
        /// </summary>
        internal void Hit(int damage, int id = 0)
        {
            _healthAbility.Damaged(damage, id);
        }

        #region ���� �鿪 ����
        /// <summary>
        /// ���� �鿪�� ���Ǿ�����
        /// </summary>
        private bool UsedDamageImmunity(EDamageType damageType)
        {
            for (int i = _damageImmunityCount - 1; i >= 0; i--)
            {
                var immunity = _damageImmunities[i];

                // �ش� ���� Ÿ�Կ� ���� �鿪�� �ִ��� Ȯ��
                if (immunity.damageType == damageType)
                {
                    // Ƚ�� ��� �鿪 ó��
                    if (immunity.count > 0)
                    {
                        immunity.count--;
                        if (immunity.count == 0)
                        {
                            _damageImmunities.RemoveAt(i);
                        }
                        return true;
                    }
                }
            }

            return false;
        }

        /// <summary>
        /// �������� ���� �鿪
        /// </summary>
        internal void AddDamageImmunity(EDamageType damageType, int count)
        {
            _damageImmunities.Add(new DamageImmunityInstance(damageType, count));
        }

        /// <summary>
        /// ���ӽð� �ִ� ���� �鿪
        /// </summary>
        internal void AddDamageImmunity(EDamageType damageType, float duration)
        {
            var coroutine = StartCoroutine(CoDamageImmunity(_damageImmunityIdCounter, duration));
            _damageImmunities.Add(new DamageImmunityInstance(damageType, _damageImmunityIdCounter, coroutine, duration));
            _damageImmunityIdCounter++;
        }

        /// <summary>
        /// ���ӽð� + Ƚ�� ������ �����Ǵ� ���� �鿪
        /// </summary>
        internal void AddDamageImmunity(EDamageType damageType, int count, float duration)
        {
            var coroutine = StartCoroutine(CoDamageImmunity(_damageImmunityIdCounter, duration));
            _damageImmunities.Add(new DamageImmunityInstance(damageType, _damageImmunityIdCounter, coroutine, duration, count));
            _damageImmunityIdCounter++;
        }

        private IEnumerator CoDamageImmunity(int id, float duration)
        {
            yield return new WaitForSeconds(duration);

            for (int i = 0; i < _damageImmunityCount; i++)
            {
                if (_damageImmunities[i].id == id)
                {
                    _damageImmunities.RemoveAt(i);
                    break;
                }
            }
        }
        #endregion
    }
}