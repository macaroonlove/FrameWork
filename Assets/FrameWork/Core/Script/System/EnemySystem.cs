using FrameWork.Editor;
using System.Collections.Generic;
using UnityEngine;

namespace Temporary.Core
{
    /// <summary>
    /// �� ������ �����ϴ� Ŭ����
    /// (��ƿ��Ƽ �޼���)
    /// </summary>
    public class EnemySystem : MonoBehaviour, ISubSystem
    {
        [SerializeField, ReadOnly] private List<EnemyUnit> _enemies = new List<EnemyUnit>();

        public void Initialize()
        {

        }

        public void Deinitialize()
        {
            // ���� ������Ʈ ��� �ı�
            foreach (var enemy in _enemies)
            {
                Destroy(enemy.gameObject);
            }
        }

        internal void Regist(EnemyUnit enemy)
        {
            _enemies.Add(enemy);
        }

        internal void Deregist(EnemyUnit enemy)
        {
            _enemies.Remove(enemy);
        }

        #region ��ƿ��Ƽ �޼���
        /// <summary>
        /// ��ϵ� ��� �� ������ ��ȯ
        /// </summary>
        internal List<EnemyUnit> GetAllEnemies()
        {
            return _enemies;
        }

        /// <summary>
        /// ���� ���� �� ������ ��ȯ (unitPos�� ����� ���ֺ��� ��ȯ)
        /// </summary>
        internal List<EnemyUnit> GetEnemiesInRadius(Vector3 unitPos, float radius, int maxCount = int.MaxValue)
        {
            List<EnemyUnit> enemies = new List<EnemyUnit>();
            List<(EnemyUnit enemy, float distance)> enemiesWithDistance = new List<(EnemyUnit, float)>();

            radius *= radius;

            foreach (EnemyUnit enemy in _enemies)
            {
                if (enemy != null && enemy.isActiveAndEnabled)
                {
                    var distance = (enemy.transform.position - unitPos).sqrMagnitude;

                    if (distance <= radius)
                    {
                        enemiesWithDistance.Add((enemy, distance));
                    }
                }
            }

            if (enemiesWithDistance.Count > maxCount)
            {
                enemiesWithDistance.Sort((a, b) => a.distance.CompareTo(b.distance));
            }

            foreach (var (enemy, _) in enemiesWithDistance)
            {
                if (enemies.Count >= maxCount) break;
                enemies.Add(enemy);
            }

            return enemies;
        }

        /// <summary>
        /// ���� ������ �� ������ ��ȯ
        /// </summary>
        internal List<EnemyUnit> GetAttackableEnemiesInRadius(Vector3 unitPos, float radius, EAttackType attackType, int maxCount = int.MaxValue)
        {
            List<EnemyUnit> enemies = new List<EnemyUnit>();
            List<(EnemyUnit enemy, float distance)> enemiesWithDistance = new List<(EnemyUnit, float)>();

            radius *= radius;

            foreach (EnemyUnit enemy in _enemies)
            {
                if (enemy != null && enemy.isActiveAndEnabled)
                {
                    // ���� ���� ������ ��, ���Ÿ��� �ƴ϶�� ���� �Ұ�
                    if (enemy.template.MoveType == EMoveType.Sky && attackType != EAttackType.Far) continue;
                    // ���� ����� �ƴ϶�� Ÿ�ٿ� �߰����� ����
                    if (enemy.GetAbility<HitAbility>().finalTargetOfAttack == false) continue;

                    var distance = (enemy.transform.position - unitPos).sqrMagnitude;

                    if (distance <= radius)
                    {
                        enemiesWithDistance.Add((enemy, distance));
                    }
                }
            }

            if (enemiesWithDistance.Count > maxCount)
            {
                enemiesWithDistance.Sort((a, b) => a.distance.CompareTo(b.distance));
            }

            foreach (var (enemy, _) in enemiesWithDistance)
            {
                if (enemies.Count >= maxCount) break;
                enemies.Add(enemy);
            }

            return enemies;
        }

        /// <summary>
        /// ȸ�� ������ �� ������ ��ȯ
        /// </summary>
        internal List<EnemyUnit> GetHealableEnemiesInRadius(Vector3 unitPos, float radius, int maxCount = int.MaxValue)
        {
            List<EnemyUnit> enemies = new List<EnemyUnit>();
            List<(EnemyUnit enemy, float distance)> enemiesWithDistance = new List<(EnemyUnit, float)>();

            radius *= radius;

            foreach (EnemyUnit enemy in _enemies)
            {
                if (enemy != null && enemy.isActiveAndEnabled)
                {
                    var distance = (enemy.transform.position - unitPos).sqrMagnitude;

                    if (distance <= radius)
                    {
                        enemiesWithDistance.Add((enemy, distance));
                    }
                }
            }

            if (enemiesWithDistance.Count > maxCount)
            {
                enemiesWithDistance.Sort((a, b) => a.distance.CompareTo(b.distance));
            }

            foreach (var (enemy, _) in enemiesWithDistance)
            {
                if (enemies.Count >= maxCount) break;
                enemies.Add(enemy);
            }

            return enemies;
        }

        /// <summary>
        /// ���� ���� ���� ����� �� ������ ��ȯ
        /// </summary>
        internal EnemyUnit GetNearestEnemy(Vector3 unitPos, float radius)
        {
            EnemyUnit enemyUnit = null;
            radius *= radius;
            float nearestDistance = Mathf.Infinity;

            foreach (EnemyUnit enemy in _enemies)
            {
                if (enemy != null && enemy.isActiveAndEnabled)
                {
                    float distance = (enemy.transform.position - unitPos).sqrMagnitude;

                    if (distance < nearestDistance && distance <= radius)
                    {
                        enemyUnit = enemy;
                        nearestDistance = distance;
                    }
                }
            }

            return enemyUnit;
        }
        #endregion
    }
}