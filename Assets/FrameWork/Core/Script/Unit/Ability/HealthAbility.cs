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

        #region 보호막 필드
        private class ShieldInstance
        {
            public Coroutine coroutine;
            public int id;
            public int amount;
            public float duration;

            // 무한 지속 보호막
            public ShieldInstance(int amount, float duration)
            {
                this.amount = amount;
                this.duration = duration;
            }

            // 지속시간이 있는 보호막
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

        #region 계산 스탯
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

        #region 추가 회복량
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

        #region HP 변경
        internal bool Damaged(int damage, int id)
        {
            //죽었으면 무시
            if (!isAlive) return false;

            // 실드에 막히는 데미지를 제외
            var lostHealth = DamagedShield(damage);
            lostHealth = Mathf.Max(0, lostHealth);

            //잃을 HP 가 있을 때
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
            //죽었으면 무시
            if (!isAlive) return;

            float healingAmount = value;

            // 추가 회복량 적용
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

        #region 보호막 변경
        private int DamagedShield(int damage)
        {
            int finalDamage = damage;

            // 실드가 있을 떄
            if (shieldCount > 0)
            {
                int totalShield = shieldAmount;

                // 보호막으로 방어 했을때
                if (totalShield >= damage)
                {
                    // 최근에 추가된 보호막부터 차감
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

                    #region 오래된 보호막부터 차감
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
                // 보호막으로 방어하지 못하고 피가 까일 때
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
            //죽었으면 무시
            if (!isAlive)
            {
                return;
            }

            _shields.Add(new ShieldInstance(amount, int.MaxValue));
            UpdateShield();
        }

        internal void AddShield(int amount, float duration)
        {
            //죽었으면 무시
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