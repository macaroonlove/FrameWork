using FrameWork.Editor;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Temporary.Core
{
    /// <summary>
    /// ����, ����ü ������ �����ϴ� �����Ƽ
    /// ���ݷ�, ���ݼӵ�, ���� ��Ÿ�, ���� ���� ��, ���� ����� ����մϴ�.
    /// </summary>
    public class AttackAbility : AlwaysAbility
    {
        [SerializeField] private bool isProjectileAttack;
        [SerializeField, Condition("isProjectileAttack", true, true)] private GameObject projectilePrefab;
        

        private UnitAnimationAbility _unitAnimationAbility;
        private BuffAbility _buffAbility;
        private AbnormalStatusAbility _abnormalStatusAbility;
        private FindTargetAbility _findTargetAbility;
        private ProjectileAbility _projectileAbility;

        private int _baseATK;
        private float _baseAttackTerm;
        private float _baseAttackRange;
        private EAttackType _baseAttackType;
        private float _attackCooldown;

        private AttackEventHandler _attackEventHandler;
        private bool _isEventAttack;

        private EAttackType _currentAttackType;
        private List<Unit> _currentTarget = new List<Unit>();

        #region ������Ƽ
        internal int baseATK => _baseATK;
        #endregion

        #region ���� ���
        internal int finalATK
        {
            get
            {
                float result = _baseATK;

                #region �߰�������
                foreach (var effect in _buffAbility.ATKAdditionalDataEffects)
                {
                    result += effect.value;
                }
                #endregion

                #region ����������
                float increase = 1;

                foreach (var effect in _buffAbility.ATKIncreaseDataEffects)
                {
                    increase += effect.value;
                }

                result *= increase;
                #endregion

                #region ��¡��϶�
                foreach (var effect in _buffAbility.ATKMultiplierDataEffects)
                {
                    result *= effect.value;
                }
                #endregion

                return (int)result;
            }
        }

        private float finalAttackTerm
        {
            get
            {
                float result = _baseAttackTerm;

                #region ����������
                float increase = 1;

                foreach (var effect in _buffAbility.AttackSpeedIncreaseDataEffects)
                {
                    increase += effect.value;
                }

                result *= increase;
                #endregion

                #region ��¡��϶�
                foreach (var effect in _buffAbility.AttackSpeedMultiplierDataEffects)
                {
                    result *= effect.value;
                }
                #endregion

                return result;
            }
        }

        private float finalAttackRange
        {
            get
            {
                float result = _baseAttackRange;

                return result;
            }
        }

        private int finalAttackCount
        {
            get
            {
                // �ִ� ���� ���� ���� 1��
                int result = 1;

                foreach (var effect in _buffAbility.AttackCountAdditionalDataEffects)
                {
                    result += effect.value;
                }

                return result;
            }
        }

        private EAttackType finalAttackType
        {
            get
            {
                EAttackType result = _baseAttackType;

                foreach (var effect in _buffAbility.SetAttackTypeEffects)
                {
                    result = effect.value;
                }

                return result;
            }
        }

        private bool finalIsAttackAble
        {
            get
            {
                // ���� �Ұ� �����̻� �ɷȴٸ�
                if (_abnormalStatusAbility.UnableToAttackEffects.Count > 0) return false;

                return true;
            }
        }
        #endregion

        internal event UnityAction onAttack;

        internal override void Initialize(Unit unit)
        {
            base.Initialize(unit);

            _unitAnimationAbility = unit.GetAbility<UnitAnimationAbility>();
            _buffAbility = unit.GetAbility<BuffAbility>();
            _abnormalStatusAbility = unit.GetAbility<AbnormalStatusAbility>();
            _findTargetAbility = unit.GetAbility<FindTargetAbility>();
            _projectileAbility = unit.GetAbility<ProjectileAbility>();

            if (unit is AgentUnit agentUnit)
            {
                _baseATK = agentUnit.template.ATK;
                _baseAttackTerm = agentUnit.template.AttackTerm;
                _baseAttackRange = agentUnit.template.AttackRange;
                _baseAttackType = agentUnit.template.AttackType;
            }
            else if (unit is EnemyUnit enemyUnit)
            {
                _baseATK = enemyUnit.template.ATK;
                _baseAttackTerm = enemyUnit.template.AttackTerm;
                _baseAttackRange = enemyUnit.template.AttackRange;
                _baseAttackType = enemyUnit.template.AttackType;
            }

            _attackCooldown = finalAttackTerm;

            _attackEventHandler = GetComponentInChildren<AttackEventHandler>();
            if (_attackEventHandler != null)
            {
                _attackEventHandler.onAttack += OnAttackEvent;
                _isEventAttack = true;
            }
        }

        internal override void Deinitialize()
        {
            if (_attackEventHandler != null)
            {
                _attackEventHandler.onAttack -= OnAttackEvent;
                _isEventAttack = false;
            }
        }

        internal override void UpdateAbility()
        {
            // ���� �Ұ� ���¶�� �޼��� ������
            if (finalIsAttackAble == false || finalAttackType == EAttackType.None) return;

            _currentAttackType = finalAttackType;

            // ���� ��Ÿ�� ����
            if (_attackCooldown > 0)
            {
                _attackCooldown -= Time.deltaTime;
                return;
            }

            // ������ �������θ� ��ȯ
            bool isAction = Action();

            // ������ �������� ���
            if (isAction)
            {
                // ��Ÿ�� ���
                _attackCooldown = finalAttackTerm;
            }
        }

        private bool Action()
        {
            // �ٰŸ��� ���Ÿ� �����̶��
            if (_currentAttackType == EAttackType.Near || _currentAttackType == EAttackType.Far)
            {
                return Attack();
            }
            // ȸ�� �����̶��
            else if (_currentAttackType == EAttackType.Heal)
            {
                return Heal();
            }

            return false;
        }

        #region ����, ȸ�� ���� ����
        private void AttackAnimation()
        {
            _unitAnimationAbility.Attack();

            if (!_isEventAttack)
            {
                ExecuteAction();
            }
        }

        private void OnAttackEvent()
        {
            ExecuteAction();
        }

        private void ExecuteAction()
        {
            // �ٰŸ��� ���Ÿ� �����̶��
            if (_currentAttackType == EAttackType.Near || _currentAttackType == EAttackType.Far)
            {
                ExecuteAttack();
            }
            // ȸ�� �����̶��
            else if (_currentAttackType == EAttackType.Heal)
            {
                ExecuteHeal();
            }
        }

        internal void ApplyAction(Unit target)
        {
            // �ٰŸ��� ���Ÿ� �����̶��
            if (_currentAttackType == EAttackType.Near || _currentAttackType == EAttackType.Far)
            {
                ApplyAttack(target);
            }
            // ȸ�� �����̶��
            else if (_currentAttackType == EAttackType.Heal)
            {
                ApplyHeal(target);
            }
        }
        #endregion

        #region ���� ����
        private bool Attack()
        {
            _currentTarget.Clear();

            var attackTargets = _findTargetAbility.FindAttackableTarget(ETarget.NumTargetInRange, finalAttackRange, _currentAttackType, finalAttackCount);

            if (attackTargets.Count > 0)
            {
                _currentTarget.AddRange(attackTargets);
            }

            // ���� ��� ����
            if (_currentTarget.Count > 0)
            {
                AttackAnimation();
                return true;
            }
            else
            {
                return false;
            }
        }

        private void ExecuteAttack()
        {
            // ����ü ������ ���
            if (isProjectileAttack)
            {
                // ����ü ����
                foreach (var attackTarget in _currentTarget)
                {
                    _projectileAbility.SpawnProjectile(projectilePrefab, attackTarget, (caster, target) => { ApplyAction(target); });
                }
            }
            // ��� ������ ���
            else
            {
                foreach (var attackTarget in _currentTarget)
                {
                    ApplyAttack(attackTarget);
                }
            }
        }

        private void ApplyAttack(Unit attackTarget)
        {
            attackTarget.GetAbility<HitAbility>().Hit(unit);
            onAttack?.Invoke();
        }
        #endregion

        #region ȸ�� ����
        private bool Heal()
        {
            _currentTarget.Clear();

            var attackTargets = _findTargetAbility.FindHealableTarget(ETarget.NumTargetInRange, finalAttackRange, finalAttackCount);

            if (attackTargets.Count > 0)
            {
                _currentTarget.AddRange(attackTargets);
            }

            // ȸ�� ��� ����
            if (_currentTarget.Count > 0)
            {
                AttackAnimation();
                return true;
            }
            else
            {
                return false;
            }
        }

        private void ExecuteHeal()
        {
            // ����ü ȸ���� ���
            if (isProjectileAttack)
            {
                // ����ü ����
                foreach (var healTarget in _currentTarget)
                {
                    _projectileAbility.SpawnProjectile(projectilePrefab, healTarget, (caster, target) => { ApplyAction(target); });
                }
            }
            // ��� ȸ���� ���
            else
            {
                foreach (var healTarget in _currentTarget)
                {
                    ApplyHeal(healTarget);
                }
            }
        }

        private void ApplyHeal(Unit healTarget)
        {
            healTarget.GetAbility<HealthAbility>().Healed(finalATK, unit.id);
            onAttack?.Invoke();
        }
        #endregion
    }
}