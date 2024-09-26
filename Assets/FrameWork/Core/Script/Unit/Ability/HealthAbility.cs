using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

namespace Temporary.Core
{
    public class HealthAbility : AlwaysAbility
    {
        private PoolSystem _poolSystem;

        private int _baseMaxHP;
        private int _currentHp;
        private int _baseHPRecoveryPerSec;

        internal bool isAlive => _currentHp > 0;

        #region ��ȣ�� �ʵ�
        private class ShieldInstance
        {
            public Coroutine coroutine;
            public int id;
            public int amount;
            public float duration;

            // ���� ���� ��ȣ��
            public ShieldInstance(int amount, float duration)
            {
                this.amount = amount;
                this.duration = duration;
            }

            // ���ӽð��� �ִ� ��ȣ��
            public ShieldInstance(Coroutine coroutine, int id, int amount, float duration)
            {
                this.coroutine = coroutine;
                this.id = id;
                this.amount = amount;
                this.duration = duration;
            }
        }

        [SerializeField] private GameObject _shieldFX;

        private List<ShieldInstance> _shields = new List<ShieldInstance>();
        private GameObject _shieldObject;
        private int _shieldIdCounter = 0;

        internal int shieldCount => _shields.Count;
        internal int shieldAmount => _shields.Sum(shield => shield.amount);
        #endregion

        internal event UnityAction<int, int> onDamage;
        internal event UnityAction<int> onChangedHealth;
        internal event UnityAction<int> onChangedShield;
        internal event UnityAction onDeath;

        #region ��� ����
        private int finalMaxHP
        {
            get
            {
                int result = _baseMaxHP;

                return result;
            }
        }

        private int finalMinHP
        {
            get
            {
                int result = 0;

                return result;
            }
        }

        #region �߰� ȸ����
        private int healingAdditional
        {
            get
            {
                int result = 0;

                return result;
            }
        }

        private float healingIncrease
        {
            get
            {
                float result = 1.0f;

                return result;
            }
        }

        private float healingMultiplier
        {
            get
            {
                float result = 1.0f;

                return result;
            }
        }
        #endregion
        #endregion

        internal override void Initialize(Unit unit)
        {
            base.Initialize(unit);

            _poolSystem = BattleManager.Instance.GetSubSystem<PoolSystem>();

            if (unit is AgentUnit agentUnit)
            {
                _baseMaxHP = agentUnit.template.MaxHP;
                _baseHPRecoveryPerSec = agentUnit.template.HPRecoveryPerSec;
            }
            else if (unit is EnemyUnit enemyUnit)
            {
                _baseMaxHP = enemyUnit.template.MaxHP;
                _baseHPRecoveryPerSec = enemyUnit.template.HPRecoveryPerSec;
            }

            SetHP(finalMaxHP);
        }

        internal override void Deinitialize()
        {
            _poolSystem = null;
        }

        internal override void UpdateAbility()
        {
            
        }

        #region HP ����
        internal bool Damaged(int damage, int id)
        {
            //�׾����� ����
            if (!isAlive) return false;

            // �ǵ忡 ������ �������� ����
            var lostHealth = DamagedShield(damage);
            lostHealth = Mathf.Max(0, lostHealth);

            //���� HP �� ���� ��
            if (lostHealth > 0)
            {
                SetHP(_currentHp - lostHealth);
                onDamage?.Invoke(id, lostHealth);

                return true;
            }

            return false;
        }

        internal void Healed(int value, int id)
        {
            //�׾����� ����
            if (!isAlive) return;

            float healingAmount = value;

            // �߰� ȸ���� ����
            healingAmount += healingAdditional;
            healingAmount *= healingIncrease;
            healingAmount *= healingMultiplier;

            var lastHp = Mathf.RoundToInt(_currentHp + healingAmount);
            lastHp = Mathf.Clamp(lastHp, 0, finalMaxHP);

            SetHP(lastHp);
        }

        private void SetHP(int hp)
        {
            _currentHp = Mathf.Max(finalMinHP, hp);
            if (_currentHp == 0)
            {
                onDeath?.Invoke();
                return;
            }
            onChangedHealth?.Invoke(_currentHp);
        }
        #endregion

        #region ��ȣ�� ����
        private int DamagedShield(int damage)
        {
            int finalDamage = damage;

            // �ǵ尡 ���� ��
            if (shieldCount > 0)
            {
                int totalShield = shieldAmount;

                // ��ȣ������ ��� ������
                if (totalShield >= damage)
                {
                    // �ֱٿ� �߰��� ��ȣ������ ����
                    for (int i = shieldCount - 1; i >= 0; i--)
                    {
                        var shield = _shields[i];
                        int remainingShield = shield.amount - damage;

                        if (remainingShield >= 0)
                        {
                            shield.amount = remainingShield;
                            onChangedShield?.Invoke(totalShield - damage);
                            return 0;
                        }
                        else
                        {
                            damage -= shield.amount;

                            if (shield.coroutine != null)
                            {
                                StopCoroutine(shield.coroutine);
                            }
                            _shields.RemoveAt(i);
                            UpdateShield();
                        }
                    }

                    #region ������ ��ȣ������ ����
                    //for (int i = 0; i < shieldCount; i++)
                    //{
                    //    var shield = _shields[i];
                    //    int remainingShield = shield.amount - damage;
                    //    if (remainingShield >= 0)
                    //    {
                    //        shield.amount = remainingShield;
                    //        onChangedShield?.Invoke(totalShield - damage);
                    //        return 0;
                    //    }
                    //    else
                    //    {
                    //        damage -= shield.amount;

                    //        if (shield.coroutine != null)
                    //        {
                    //            StopCoroutine(shield.coroutine);
                    //        }
                    //        _shields.RemoveAt(i);
                    //        UpdateShield();
                    //        i--;
                    //    }
                    //}
                    #endregion
                }
                // ��ȣ������ ������� ���ϰ� �ǰ� ���� ��
                else
                {
                    _shields.Clear();
                    UpdateShield();
                    onChangedShield?.Invoke(0);
                }

                finalDamage -= totalShield;
            }

            return finalDamage;
        }

        internal void AddShield(int amount)
        {
            //�׾����� ����
            if (!isAlive)
            {
                return;
            }

            _shields.Add(new ShieldInstance(amount, int.MaxValue));
            UpdateShield();
        }

        internal void AddShield(int amount, float duration)
        {
            //�׾����� ����
            if (!isAlive)
            {
                return;
            }

            var coroutine = StartCoroutine(CoShield(_shieldIdCounter, duration));
            _shields.Add(new ShieldInstance(coroutine, _shieldIdCounter, amount, duration));
            UpdateShield();
            _shieldIdCounter++;
        }

        private void UpdateShield()
        {
            onChangedShield?.Invoke(shieldAmount);

            if (shieldCount > 0)
            {
                _shieldObject = _poolSystem.Spawn(_shieldFX, this.transform);
            }
            else
            {
                _poolSystem.DeSpawn(_shieldObject);
            }
        }

        private IEnumerator CoShield(int id, float duration)
        {
            yield return new WaitForSeconds(duration);

            for (int i = 0; i < shieldCount; i++)
            {
                if (_shields[i].id == id)
                {
                    _shields.RemoveAt(i);
                    break;
                }
            }
            UpdateShield();
        }
        #endregion
    }
}