using FrameWork;
using System.Collections.Generic;
using UnityEngine;

namespace Temporary.Core
{
    public class FindTargetAbility : AlwaysAbility
    {
        private AgentSystem _agentSystem;
        private EnemySystem _enemySystem;

        internal override void Initialize(Unit unit)
        {
            base.Initialize(unit);

            _agentSystem = BattleManager.Instance.GetSubSystem<AgentSystem>();
            _enemySystem = BattleManager.Instance.GetSubSystem<EnemySystem>();
        }

        #region 범위 타입에 따른 유닛 반환 로직
        internal List<Unit> FindAllTarget(EUnitType unitType)
        {
            List<Unit> targets = new List<Unit>();

            switch (unitType)
            {
                case EUnitType.All:
                    targets.AddRange(_agentSystem.GetAllAgents());
                    targets.AddRange(_enemySystem.GetAllEnemies());
                    break;
                case EUnitType.Agent:
                    targets.AddRange(_agentSystem.GetAllAgents());
                    break;
                case EUnitType.Enemy:
                    targets.AddRange(_enemySystem.GetAllEnemies());
                    break;
            }

            return targets;
        }

        /// <summary>
        /// 원 범위 내에 존재하는 유닛 반환
        /// </summary>
        internal List<Unit> FindTargetInCircle(float radius, EUnitType unitType, int maxCount)
        {
            List<Unit> targets = (maxCount == int.MaxValue) ? new List<Unit>() : new List<Unit>(maxCount);

            switch (unitType)
            {
                case EUnitType.All:
                    targets.AddRange(_agentSystem.GetAgentsInCircle(transform.position, radius));
                    targets.AddRange(_enemySystem.GetEnemiesInCircle(transform.position, radius));
                    if (maxCount != int.MaxValue) GetSortedUnits(targets, transform.position, maxCount);
                    break;
                case EUnitType.Agent:
                    targets.AddRange(_agentSystem.GetAgentsInCircle(transform.position, radius, maxCount));
                    break;
                case EUnitType.Enemy:
                    targets.AddRange(_enemySystem.GetEnemiesInCircle(transform.position, radius, maxCount));
                    break;
            }

            return targets;
        }

        /// <summary>
        /// 직선 범위 내에 존재하는 유닛 반환
        /// </summary>
        internal List<Unit> FindTargetInStraight(EDirectionType directionType, float range, float width, EUnitType unitType, int maxCount)
        {
            directionMap.TryGetValue(directionType, out var direction);

            return FindTargetInStraight(direction, range, width, unitType, maxCount);
        }

        /// <summary>
        /// 직선 범위 내에 존재하는 유닛 반환
        /// </summary>
        internal List<Unit> FindTargetInStraight(Vector3 direction, float range, float width, EUnitType unitType, int maxCount)
        {
            List<Unit> targets = (maxCount == int.MaxValue) ? new List<Unit>() : new List<Unit>(maxCount);

            switch (unitType)
            {
                case EUnitType.All:
                    targets.AddRange(_agentSystem.GetAgentsInStraight(transform.position, direction, range, width));
                    targets.AddRange(_enemySystem.GetEnemiesInStraight(transform.position, direction, range, width));
                    if (maxCount != int.MaxValue) GetSortedUnits(targets, transform.position, maxCount);
                    break;
                case EUnitType.Agent:
                    targets.AddRange(_agentSystem.GetAgentsInStraight(transform.position, direction, range, width, maxCount));
                    break;
                case EUnitType.Enemy:
                    targets.AddRange(_enemySystem.GetEnemiesInStraight(transform.position, direction, range, width, maxCount));
                    break;
            }

            return targets;
        }

        /// <summary>
        /// 콘 범위 내에 존재하는 유닛 반환
        /// </summary>
        internal List<Unit> FindTargetInCone(EDirectionType directionType, float range, int angle, EUnitType unitType, int maxCount)
        {
            directionMap.TryGetValue(directionType, out var direction);

            return FindTargetInCone(direction, range, angle, unitType, maxCount);
        }

        /// <summary>
        /// 콘 범위 내에 존재하는 유닛 반환
        /// </summary>
        internal List<Unit> FindTargetInCone(Vector3 direction, float range, int angle, EUnitType unitType, int maxCount)
        {
            List<Unit> targets = (maxCount == int.MaxValue) ? new List<Unit>() : new List<Unit>(maxCount);

            switch (unitType)
            {
                case EUnitType.All:
                    targets.AddRange(_agentSystem.GetAgentsInCone(transform.position, direction, range, angle));
                    targets.AddRange(_enemySystem.GetEnemiesInCone(transform.position, direction, range, angle));
                    if (maxCount != int.MaxValue) GetSortedUnits(targets, transform.position, maxCount);
                    break;
                case EUnitType.Agent:
                    targets.AddRange(_agentSystem.GetAgentsInCone(transform.position, direction, range, angle, maxCount));
                    break;
                case EUnitType.Enemy:
                    targets.AddRange(_enemySystem.GetEnemiesInCone(transform.position, direction, range, angle, maxCount));
                    break;
            }

            return targets;
        }

        /// <summary>
        /// 격자 범위 내에 존재하는 유닛 반환
        /// </summary>
        internal List<Unit> FindTargetInGrid(List<Vector2Int> grid, EUnitType unitType, int maxCount)
        {
            List<Unit> targets = (maxCount == int.MaxValue) ? new List<Unit>() : new List<Unit>(maxCount);

            switch (unitType)
            {
                case EUnitType.All:
                    targets.AddRange(_agentSystem.GetAgentsInGrid(unit.cellPos, grid));
                    targets.AddRange(_enemySystem.GetEnemiesInGrid(unit.cellPos, grid));
                    if (maxCount != int.MaxValue) GetSortedUnits(targets, transform.position, maxCount);
                    break;
                case EUnitType.Agent:
                    targets.AddRange(_agentSystem.GetAgentsInGrid(unit.cellPos, grid, maxCount));
                    break;
                case EUnitType.Enemy:
                    targets.AddRange(_enemySystem.GetEnemiesInGrid(unit.cellPos, grid, maxCount));
                    break;
            }

            return targets;
        }
        #endregion

        #region 범위 타입에 따른 공격 가능한 유닛 반환 로직
        /// <summary>
        /// 공격 가능한 유닛 찾기
        /// </summary>
        internal List<Unit> FindAttackableTarget(float range, EAttackType attackType, int attackCount)
        {
            if (unit is AgentUnit)
            {
                return FindAttackableTargetInCircle(range, EUnitType.Enemy, attackType, attackCount);
            }
            else if (unit is EnemyUnit)
            {
                return FindAttackableTargetInCircle(range, EUnitType.Agent, attackType, attackCount);
            }

            return null;
        }

        internal List<Unit> FindAllAttackableTarget(EUnitType unitType, EAttackType attackType)
        {
            List<Unit> targets = new List<Unit>();

            switch (unitType)
            {
                case EUnitType.All:
                    targets.AddRange(_agentSystem.GetAllAttackableAgents(attackType));
                    targets.AddRange(_enemySystem.GetAllAttackableEnemies(attackType));
                    break;
                case EUnitType.Agent:
                    targets.AddRange(_agentSystem.GetAllAttackableAgents(attackType));
                    break;
                case EUnitType.Enemy:
                    targets.AddRange(_enemySystem.GetAllAttackableEnemies(attackType));
                    break;
            }

            return targets;
        }

        internal List<Unit> FindAttackableTargetInCircle(float radius, EUnitType unitType, EAttackType attackType, int maxCount)
        {
            List<Unit> targets = (maxCount == int.MaxValue) ? new List<Unit>() : new List<Unit>(maxCount);

            switch (unitType)
            {
                case EUnitType.All:
                    targets.AddRange(_agentSystem.GetAttackableAgents(transform.position, radius, attackType));
                    targets.AddRange(_enemySystem.GetAttackableEnemies(transform.position, radius, attackType));
                    if (maxCount != int.MaxValue) GetSortedUnits(targets, transform.position, maxCount);
                    break;
                case EUnitType.Agent:
                    targets.AddRange(_agentSystem.GetAttackableAgents(transform.position, radius, attackType, maxCount));
                    break;
                case EUnitType.Enemy:
                    targets.AddRange(_enemySystem.GetAttackableEnemies(transform.position, radius, attackType, maxCount));
                    break;
            }

            return targets;
        }

        internal List<Unit> FindAttackableTargetInStraight(EDirectionType directionType, float range, float width, EUnitType unitType, EAttackType attackType, int maxCount)
        {
            directionMap.TryGetValue(directionType, out var direction);

            return FindAttackableTargetInStraight(direction, range, width, unitType, attackType, maxCount);
        }

        internal List<Unit> FindAttackableTargetInStraight(Vector3 direction, float range, float width, EUnitType unitType, EAttackType attackType, int maxCount)
        {
            List<Unit> targets = (maxCount == int.MaxValue) ? new List<Unit>() : new List<Unit>(maxCount);

            switch (unitType)
            {
                case EUnitType.All:
                    targets.AddRange(_agentSystem.GetAttackableAgents(transform.position, direction, range, width, attackType));
                    targets.AddRange(_enemySystem.GetAttackableEnemies(transform.position, direction, range, width, attackType));
                    if (maxCount != int.MaxValue) GetSortedUnits(targets, transform.position, maxCount);
                    break;
                case EUnitType.Agent:
                    targets.AddRange(_agentSystem.GetAttackableAgents(transform.position, direction, range, width, attackType, maxCount));
                    break;
                case EUnitType.Enemy:
                    targets.AddRange(_enemySystem.GetAttackableEnemies(transform.position, direction, range, width, attackType, maxCount));
                    break;
            }

            return targets;
        }

        internal List<Unit> FindAttackableTargetInCone(EDirectionType directionType, float range, int angle, EUnitType unitType, EAttackType attackType, int maxCount)
        {
            directionMap.TryGetValue(directionType, out var direction);

            return FindAttackableTargetInCone(direction, range, angle, unitType, attackType, maxCount);
        }

        internal List<Unit> FindAttackableTargetInCone(Vector3 direction, float range, int angle, EUnitType unitType, EAttackType attackType, int maxCount)
        {
            List<Unit> targets = (maxCount == int.MaxValue) ? new List<Unit>() : new List<Unit>(maxCount);

            switch (unitType)
            {
                case EUnitType.All:
                    targets.AddRange(_agentSystem.GetAttackableAgents(transform.position, direction, range, angle, attackType));
                    targets.AddRange(_enemySystem.GetAttackableEnemies(transform.position, direction, range, angle, attackType));
                    if (maxCount != int.MaxValue) GetSortedUnits(targets, transform.position, maxCount);
                    break;
                case EUnitType.Agent:
                    targets.AddRange(_agentSystem.GetAttackableAgents(transform.position, direction, range, angle, attackType, maxCount));
                    break;
                case EUnitType.Enemy:
                    targets.AddRange(_enemySystem.GetAttackableEnemies(transform.position, direction, range, angle, attackType, maxCount));
                    break;
            }

            return targets;
        }

        internal List<Unit> FindAttackableTargetInGrid(List<Vector2Int> grid, EUnitType unitType, EAttackType attackType, int maxCount)
        {
            List<Unit> targets = (maxCount == int.MaxValue) ? new List<Unit>() : new List<Unit>(maxCount);

            switch (unitType)
            {
                case EUnitType.All:
                    targets.AddRange(_agentSystem.GetAttackableAgents(unit.cellPos, grid, attackType));
                    targets.AddRange(_enemySystem.GetAttackableEnemies(unit.cellPos, grid, attackType));
                    if (maxCount != int.MaxValue) GetSortedUnits(targets, transform.position, maxCount);
                    break;
                case EUnitType.Agent:
                    targets.AddRange(_agentSystem.GetAttackableAgents(unit.cellPos, grid, attackType, maxCount));
                    break;
                case EUnitType.Enemy:
                    targets.AddRange(_enemySystem.GetAttackableEnemies(unit.cellPos, grid, attackType, maxCount));
                    break;
            }

            return targets;
        }
        #endregion

        #region 범위 타입에 따른 회복 가능한 유닛 반환 로직
        /// <summary>
        /// 회복 가능한 유닛 찾기
        /// </summary>
        internal List<Unit> FindHealableTarget(float range, int healCount)
        {
            if (unit is AgentUnit)
            {
                return FindHealableTargetInCircle(range, EUnitType.Enemy, healCount);
            }
            else if (unit is EnemyUnit)
            {
                return FindHealableTargetInCircle(range, EUnitType.Agent, healCount);
            }

            return null;
        }

        internal List<Unit> FindAllHealableTarget(EUnitType unitType)
        {
            List<Unit> targets = new List<Unit>();

            switch (unitType)
            {
                case EUnitType.All:
                    targets.AddRange(_agentSystem.GetAllHealableAgents());
                    targets.AddRange(_enemySystem.GetAllHealableEnemies());
                    break;
                case EUnitType.Agent:
                    targets.AddRange(_agentSystem.GetAllHealableAgents());
                    break;
                case EUnitType.Enemy:
                    targets.AddRange(_enemySystem.GetAllHealableEnemies());
                    break;
            }

            return targets;
        }

        internal List<Unit> FindHealableTargetInCircle(float radius, EUnitType unitType, int maxCount)
        {
            List<Unit> targets = (maxCount == int.MaxValue) ? new List<Unit>() : new List<Unit>(maxCount);

            switch (unitType)
            {
                case EUnitType.All:
                    targets.AddRange(_agentSystem.GetHealableAgents(transform.position, radius));
                    targets.AddRange(_enemySystem.GetHealableEnemies(transform.position, radius));
                    if (maxCount != int.MaxValue) GetSortedUnits(targets, transform.position, maxCount);
                    break;
                case EUnitType.Agent:
                    targets.AddRange(_agentSystem.GetHealableAgents(transform.position, radius, maxCount));
                    break;
                case EUnitType.Enemy:
                    targets.AddRange(_enemySystem.GetHealableEnemies(transform.position, radius, maxCount));
                    break;
            }

            return targets;
        }

        internal List<Unit> FindHealableTargetInStraight(EDirectionType directionType, float range, float width, EUnitType unitType, int maxCount)
        {
            directionMap.TryGetValue(directionType, out var direction);

            return FindHealableTargetInStraight(direction, range, width, unitType, maxCount);
        }

        internal List<Unit> FindHealableTargetInStraight(Vector3 direction, float range, float width, EUnitType unitType, int maxCount)
        {
            List<Unit> targets = (maxCount == int.MaxValue) ? new List<Unit>() : new List<Unit>(maxCount);

            switch (unitType)
            {
                case EUnitType.All:
                    targets.AddRange(_agentSystem.GetHealableAgents(transform.position, direction, range, width));
                    targets.AddRange(_enemySystem.GetHealableEnemies(transform.position, direction, range, width));
                    if (maxCount != int.MaxValue) GetSortedUnits(targets, transform.position, maxCount);
                    break;
                case EUnitType.Agent:
                    targets.AddRange(_agentSystem.GetHealableAgents(transform.position, direction, range, width, maxCount));
                    break;
                case EUnitType.Enemy:
                    targets.AddRange(_enemySystem.GetHealableEnemies(transform.position, direction, range, width, maxCount));
                    break;
            }

            return targets;
        }

        internal List<Unit> FindHealableTargetInCone(EDirectionType directionType, float range, int angle, EUnitType unitType, int maxCount)
        {
            directionMap.TryGetValue(directionType, out var direction);

            return FindHealableTargetInCone(direction, range, angle, unitType, maxCount);
        }

        internal List<Unit> FindHealableTargetInCone(Vector3 direction, float range, int angle, EUnitType unitType, int maxCount)
        {
            List<Unit> targets = (maxCount == int.MaxValue) ? new List<Unit>() : new List<Unit>(maxCount);

            switch (unitType)
            {
                case EUnitType.All:
                    targets.AddRange(_agentSystem.GetHealableAgents(transform.position, direction, range, angle));
                    targets.AddRange(_enemySystem.GetHealableEnemies(transform.position, direction, range, angle));
                    if (maxCount != int.MaxValue) GetSortedUnits(targets, transform.position, maxCount);
                    break;
                case EUnitType.Agent:
                    targets.AddRange(_agentSystem.GetHealableAgents(transform.position, direction, range, angle, maxCount));
                    break;
                case EUnitType.Enemy:
                    targets.AddRange(_enemySystem.GetHealableEnemies(transform.position, direction, range, angle, maxCount));
                    break;
            }

            return targets;
        }

        internal List<Unit> FindHealableTargetInGrid(List<Vector2Int> grid, EUnitType unitType, int maxCount)
        {
            List<Unit> targets = (maxCount == int.MaxValue) ? new List<Unit>() : new List<Unit>(maxCount);

            switch (unitType)
            {
                case EUnitType.All:
                    targets.AddRange(_agentSystem.GetHealableAgents(unit.cellPos, grid));
                    targets.AddRange(_enemySystem.GetHealableEnemies(unit.cellPos, grid));
                    if (maxCount != int.MaxValue) GetSortedUnits(targets, transform.position, maxCount);
                    break;
                case EUnitType.Agent:
                    targets.AddRange(_agentSystem.GetHealableAgents(unit.cellPos, grid, maxCount));
                    break;
                case EUnitType.Enemy:
                    targets.AddRange(_enemySystem.GetHealableEnemies(unit.cellPos, grid, maxCount));
                    break;
            }

            return targets;
        }
        #endregion

        #region 유틸리티
        private void GetSortedUnits(List<Unit> targets, Vector3 unitPos, int maxCount)
        {
            PriorityQueue<Unit> priorityQueue = new PriorityQueue<Unit>();

            foreach (Unit unit in targets)
            {
                var distance = (unit.transform.position - unitPos).sqrMagnitude;

                priorityQueue.Enqueue(unit, distance);

                if (priorityQueue.Count > maxCount)
                {
                    priorityQueue.Dequeue();
                }
            }

            targets.Clear();

            while (priorityQueue.Count > 0)
            {
                targets.Add(priorityQueue.Dequeue());
            }
        }

        public static IReadOnlyDictionary<EDirectionType, Vector3> directionMap => _directionMap;

        private static readonly Dictionary<EDirectionType, Vector3> _directionMap = new Dictionary<EDirectionType, Vector3>()
        {
            { EDirectionType.Up, Vector3.up },
            { EDirectionType.Down, Vector3.down },
            { EDirectionType.Left, Vector3.left },
            { EDirectionType.Right, Vector3.right },
            { EDirectionType.Front, Vector3.forward },
            { EDirectionType.Back, Vector3.back },
            { EDirectionType.UpLeft, new Vector3(-1, 1, 0).normalized },
            { EDirectionType.UpRight, new Vector3(1, 1, 0).normalized },
            { EDirectionType.DownLeft, new Vector3(-1, -1, 0).normalized },
            { EDirectionType.DownRight, new Vector3(1, -1, 0).normalized },
            { EDirectionType.UpFront, new Vector3(0, 1, 1).normalized },
            { EDirectionType.UpBack, new Vector3(0, 1, -1).normalized },
            { EDirectionType.DownFront, new Vector3(0, -1, 1).normalized },
            { EDirectionType.DownBack, new Vector3(0, -1, -1).normalized },
            { EDirectionType.UpLeftFront, new Vector3(-1, 1, 1).normalized },
            { EDirectionType.UpLeftBack, new Vector3(-1, 1, -1).normalized },
            { EDirectionType.UpRightFront, new Vector3(1, 1, 1).normalized },
            { EDirectionType.UpRightBack, new Vector3(1, 1, -1).normalized },
            { EDirectionType.DownLeftFront, new Vector3(-1, -1, 1).normalized },
            { EDirectionType.DownLeftBack, new Vector3(-1, -1, -1).normalized },
            { EDirectionType.DownRightFront, new Vector3(1, -1, 1).normalized },
            { EDirectionType.DownRightBack, new Vector3(1, -1, -1).normalized },
        };

        #endregion
    }
}