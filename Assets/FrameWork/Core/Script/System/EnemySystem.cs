using FrameWork.Editor;
using System.Collections.Generic;
using UnityEngine;

namespace Temporary.Core
{
    /// <summary>
    /// 적 유닛을 관리하는 클래스
    /// (유틸리티 메서드)
    /// </summary>
    public class EnemySystem : MonoBehaviour, ISubSystem
    {
        [SerializeField, ReadOnly] private List<EnemyUnit> _enemies = new List<EnemyUnit>();

        public void Initialize()
        {

        }

        public void Deinitialize()
        {
            // 유닛 오브젝트 모두 파괴
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

        #region 유틸리티 메서드
        /// <summary>
        /// 등록된 모든 적 유닛을 반환
        /// </summary>
        internal List<EnemyUnit> GetAllEnemies()
        {
            return _enemies;
        }

        /// <summary>
        /// 범위 내에 적 유닛을 반환 (unitPos와 가까운 유닛부터 반환)
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
        /// 공격 가능한 적 유닛을 반환
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
                    // 적이 공중 유닛일 떄, 원거리가 아니라면 공격 불가
                    if (enemy.template.MoveType == EMoveType.Sky && attackType != EAttackType.Far) continue;
                    // 공격 대상이 아니라면 타겟에 추가하지 않음
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
        /// 공격 가능한 모든 적 유닛을 반환
        /// </summary>
        internal List<EnemyUnit> GetAttackableAllEnemies(EAttackType attackType)
        {
            List<EnemyUnit> enemies = new List<EnemyUnit>();

            foreach (EnemyUnit enemy in _enemies)
            {
                if (enemy != null && enemy.isActiveAndEnabled)
                {
                    // 적이 공중 유닛일 떄, 원거리가 아니라면 공격 불가
                    if (enemy.template.MoveType == EMoveType.Sky && attackType != EAttackType.Far) continue;
                    // 공격 대상이 아니라면 타겟에 추가하지 않음
                    if (enemy.GetAbility<HitAbility>().finalTargetOfAttack == false) continue;

                    enemies.Add(enemy);
                }
            }

            return enemies;
        }

        /// <summary>
        /// 회복 가능한 적 유닛을 반환
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
                    // 회복 가능 유닛이 아니라면 타겟에 추가하지 않음
                    if (enemy.GetAbility<HealthAbility>().finalIsHealAble == false) continue;

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
        /// 회복 가능한 모든 적 유닛을 반환
        /// </summary>
        internal List<EnemyUnit> GetHealableAllEnemies()
        {
            List<EnemyUnit> enemies = new List<EnemyUnit>();

            foreach (EnemyUnit enemy in _enemies)
            {
                if (enemy != null && enemy.isActiveAndEnabled)
                {
                    // 회복 가능 유닛이 아니라면 타겟에 추가하지 않음
                    if (enemy.GetAbility<HealthAbility>().finalIsHealAble == false) continue;

                    enemies.Add(enemy);
                }
            }

            return enemies;
        }

        /// <summary>
        /// 범위 내에 가장 가까운 적 유닛을 반환
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