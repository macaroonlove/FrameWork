using FrameWork.Editor;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
        [SerializeField, Condition("isProjectileAttack", true, true)] private Transform projectileSpawnPoint;

        private AgentSystem _agentSystem;
        private EnemySystem _enemySystem;
        private PoolSystem _poolSystem;

        private UnitAnimationAbility _unitAnimationAbility;

        private int _baseATK;
        private float _baseAttackTerm;
        private float _baseAttackRange;
        private EAttackType _baseAttackType;
        private float _attackCooldown;

        private AttackEventHandler _attackEventHandler;
        private bool _isEventAttack;

        private EAttackType _currentAttackType;
        private List<Unit> _currentTarget = new List<Unit>();

        #region ��� ����
        internal int finalATK
        {
            get
            {
                int result = _baseATK;

                return result;
            }
        }

        private float finalAttackTerm
        {
            get
            {
                float result = _baseAttackTerm;

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

                return result;
            }
        }

        private EAttackType finalAttackType
        {
            get
            {
                EAttackType result = _baseAttackType;

                return result;
            }
        }
        #endregion

        internal override void Initialize(Unit unit)
        {
            base.Initialize(unit);

            _agentSystem = BattleManager.Instance.GetSubSystem<AgentSystem>();
            _enemySystem = BattleManager.Instance.GetSubSystem<EnemySystem>();

            _unitAnimationAbility = unit.GetAbility<UnitAnimationAbility>();

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
            if (finalAttackType == EAttackType.None) return;

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

            // ���� ������ �� ã��
            if (unit is AgentUnit)
            {
                var attackTargets = _enemySystem.GetAttackableEnemiesInRadius(transform.position, finalAttackRange, _currentAttackType, finalAttackCount);

                if (attackTargets.Count > 0)
                {
                    _currentTarget.AddRange(attackTargets);
                }
            }
            else if (unit is EnemyUnit)
            {
                var attackTargets = _agentSystem.GetAttackableAgentsInRadius(transform.position, finalAttackRange, _currentAttackType, finalAttackCount);

                if (attackTargets.Count > 0)
                {
                    _currentTarget.AddRange(attackTargets);
                }
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
                    var projectile = _poolSystem.Spawn(projectilePrefab).GetComponent<Projectile>();
                    projectile.transform.SetPositionAndRotation(projectileSpawnPoint.position, Quaternion.identity);
                    projectile.Initialize(this, attackTarget);
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
        }
        #endregion

        #region ȸ�� ����
        private bool Heal()
        {
            _currentTarget.Clear();

            // ȸ���� ������ �Ʊ� ã��
            // TODO: ���� => ȸ���� ������ ��������, Ǯ���� ��������, ����ִ���
            // ü�� ���� ���� ������ �����Ͽ� ȸ��
            if (unit is AgentUnit)
            {
                var attackTargets = _agentSystem.GetHealableAgentsInRadius(transform.position, finalAttackRange, finalAttackCount); 

                if (attackTargets.Count > 0)
                {
                    _currentTarget.AddRange(attackTargets);
                }
            }
            else if (unit is EnemyUnit)
            {
                var attackTargets = _enemySystem.GetHealableEnemiesInRadius(transform.position, finalAttackRange, finalAttackCount);

                if (attackTargets.Count > 0)
                {
                    _currentTarget.AddRange(attackTargets);
                }
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
                    var projectile = _poolSystem.Spawn(projectilePrefab).GetComponent<Projectile>();
                    projectile.transform.SetPositionAndRotation(projectileSpawnPoint.position, Quaternion.identity);
                    projectile.Initialize(this, healTarget);
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
        }
        #endregion
    }
}