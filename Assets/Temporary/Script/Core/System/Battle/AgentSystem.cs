using FrameWork;
using FrameWork.Editor;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Temporary.Core
{
    /// <summary>
    /// 아군 유닛을 관리하는 클래스
    /// (유틸리티 메서드)
    /// </summary>
    public class AgentSystem : MonoBehaviour, IBattleSystem
    {
        [SerializeField, ReadOnly] private List<AgentUnit> _agents = new List<AgentUnit>();

        internal event UnityAction<Unit> onRegist;

        public void Initialize()
        {

        }

        public void Deinitialize()
        {
            // 유닛 오브젝트 모두 파괴
            foreach (var agent in _agents)
            {
                Destroy(agent.gameObject);
            }
        }

        internal void Regist(AgentUnit agent)
        {
            _agents.Add(agent);

            onRegist?.Invoke(agent);
        }

        internal void Deregist(AgentUnit agent)
        {
            _agents.Remove(agent);
        }

        #region 유틸리티 메서드
        /// <summary>
        /// 등록된 모든 아군 유닛을 반환
        /// </summary>
        internal List<AgentUnit> GetAllAgents()
        {
            return _agents;
        }

        #region 원 범위 안쪽의 아군 유닛을 반환
        /// <summary>
        /// 원 범위 안쪽의 아군 유닛을 반환 (unitPos와 가까운 유닛부터 반환)
        /// </summary>
        internal List<AgentUnit> GetAgentsInCircle(Vector3 unitPos, float radius, int maxCount = int.MaxValue)
        {
            if (maxCount == int.MaxValue)
            {
                return GetAllAgentsInCircle(unitPos, radius);
            }
            else
            {
                return GetSortedAgentsInCircle(unitPos, radius, maxCount);
            }
        }

        private List<AgentUnit> GetAllAgentsInCircle(Vector3 unitPos, float radius)
        {
            List<AgentUnit> agents = new List<AgentUnit>();

            radius *= radius;

            foreach (AgentUnit agent in _agents)
            {
                if (agent != null && agent.isActiveAndEnabled)
                {
                    var distance = (agent.transform.position - unitPos).sqrMagnitude;

                    if (distance <= radius)
                    {
                        agents.Add((agent));
                    }
                }
            }

            return agents;
        }

        private List<AgentUnit> GetSortedAgentsInCircle(Vector3 unitPos, float radius)
        {
            PriorityQueue<AgentUnit> priorityQueue = new PriorityQueue<AgentUnit>();

            radius *= radius;

            foreach (AgentUnit agent in _agents)
            {
                if (agent != null && agent.isActiveAndEnabled)
                {
                    var distance = (agent.transform.position - unitPos).sqrMagnitude;

                    if (distance <= radius)
                    {
                        priorityQueue.Enqueue(agent, distance);
                    }
                }
            }

            List<AgentUnit> agents = new List<AgentUnit>(priorityQueue.Count);

            while (priorityQueue.Count > 0)
            {
                agents.Add(priorityQueue.Dequeue());
            }

            return agents;
        }

        private List<AgentUnit> GetSortedAgentsInCircle(Vector3 unitPos, float radius, int maxCount)
        {
            PriorityQueue<AgentUnit> priorityQueue = new PriorityQueue<AgentUnit>();

            radius *= radius;

            foreach (AgentUnit agent in _agents)
            {
                if (agent != null && agent.isActiveAndEnabled)
                {
                    var distance = (agent.transform.position - unitPos).sqrMagnitude;

                    if (distance <= radius)
                    {
                        priorityQueue.Enqueue(agent, distance);

                        if (priorityQueue.Count > maxCount)
                        {
                            priorityQueue.Dequeue();
                        }
                    }
                }
            }

            List<AgentUnit> agents = new List<AgentUnit>(priorityQueue.Count);

            while (priorityQueue.Count > 0)
            {
                agents.Add(priorityQueue.Dequeue());
            }

            return agents;
        }
        #endregion

        #region 직선 범위 안쪽의 아군 유닛을 반환
        /// <summary>
        /// 직선 범위 안쪽의 아군 유닛을 반환 (unitPos와 가까운 유닛부터 반환)
        /// </summary>
        internal List<AgentUnit> GetAgentsInStraight(Vector3 unitPos, Vector3 targetDir, float range, float width, int maxCount = int.MaxValue)
        {
            if (maxCount == int.MaxValue)
            {
                return GetAllAgentsInStraight(unitPos, targetDir, range, width);
            }
            else
            {
                return GetSortedAgentsInStraight(unitPos, targetDir, range, width, maxCount);
            }
        }

        private List<AgentUnit> GetAllAgentsInStraight(Vector3 unitPos, Vector3 targetDir, float range, float width)
        {
            List<AgentUnit> agents = new List<AgentUnit>();

            range *= range;
            float widthThreshold = ((width * width) / 4f) * targetDir.sqrMagnitude;

            foreach (AgentUnit agent in _agents)
            {
                if (agent != null && agent.isActiveAndEnabled)
                {
                    Vector3 dirVector = agent.transform.position - unitPos;

                    var distance = dirVector.sqrMagnitude;

                    // 범위 밖이라면
                    if (distance > range) continue;

                    // 뒤쪽에 존재한다면
                    if (Vector3.Dot(targetDir, dirVector) <= 0) continue;

                    // 직선의 폭 내에 존재한다면
                    if (Vector3.Cross(targetDir, dirVector).sqrMagnitude <= widthThreshold)
                    {
                        agents.Add(agent);
                    }
                }
            }

            return agents;
        }

        private List<AgentUnit> GetSortedAgentsInStraight(Vector3 unitPos, Vector3 targetDir, float range, float width)
        {
            PriorityQueue<AgentUnit> priorityQueue = new PriorityQueue<AgentUnit>();

            range *= range;
            float widthThreshold = ((width * width) / 4f) * targetDir.sqrMagnitude;

            foreach (AgentUnit agent in _agents)
            {
                if (agent != null && agent.isActiveAndEnabled)
                {
                    Vector3 dirVector = agent.transform.position - unitPos;
                    float distance = dirVector.sqrMagnitude;

                    // 범위 밖이라면
                    if (distance > range) continue;

                    // 뒤쪽에 존재한다면
                    if (Vector3.Dot(targetDir, dirVector) <= 0) continue;

                    // 직선의 폭 내에 존재한다면
                    if (Vector3.Cross(targetDir, dirVector).sqrMagnitude <= widthThreshold)
                    {
                        priorityQueue.Enqueue(agent, distance);
                    }
                }
            }

            List<AgentUnit> agents = new List<AgentUnit>(priorityQueue.Count);

            while (priorityQueue.Count > 0)
            {
                agents.Add(priorityQueue.Dequeue());
            }

            return agents;
        }

        private List<AgentUnit> GetSortedAgentsInStraight(Vector3 unitPos, Vector3 targetDir, float range, float width, int maxCount)
        {
            PriorityQueue<AgentUnit> priorityQueue = new PriorityQueue<AgentUnit>();

            range *= range;
            float widthThreshold = ((width * width) / 4f) * targetDir.sqrMagnitude;

            foreach (AgentUnit agent in _agents)
            {
                if (agent != null && agent.isActiveAndEnabled)
                {
                    Vector3 dirVector = agent.transform.position - unitPos;
                    float distance = dirVector.sqrMagnitude;

                    // 범위 밖이라면
                    if (distance > range) continue;

                    // 뒤쪽에 존재한다면
                    if (Vector3.Dot(targetDir, dirVector) <= 0) continue;

                    // 직선의 폭 내에 존재한다면
                    if (Vector3.Cross(targetDir, dirVector).sqrMagnitude <= widthThreshold)
                    {
                        priorityQueue.Enqueue(agent, distance);

                        if (priorityQueue.Count > maxCount)
                        {
                            priorityQueue.Dequeue();
                        }
                    }
                }
            }

            List<AgentUnit> agents = new List<AgentUnit>(priorityQueue.Count);

            while (priorityQueue.Count > 0)
            {
                agents.Add(priorityQueue.Dequeue());
            }

            return agents;
        }
        #endregion

        #region 시야각 안쪽의 아군 유닛을 반환
        /// <summary>
        /// 시야각 안쪽의 아군 유닛을 반환 (unitPos와 가까운 유닛부터 반환)
        /// </summary>
        internal List<AgentUnit> GetAgentsInCone(Vector3 unitPos, Vector3 targetDir, float range, int angle, int maxCount = int.MaxValue)
        {
            if (maxCount == int.MaxValue)
            {
                return GetAllAgentsInCone(unitPos, targetDir, range, angle);
            }
            else
            {
                return GetSortedAgentsInCone(unitPos, targetDir, range, angle, maxCount);
            }
        }

        private List<AgentUnit> GetAllAgentsInCone(Vector3 unitPos, Vector3 targetDir, float range, int angle)
        {
            List<AgentUnit> agents = new List<AgentUnit>();

            range *= range;
            float cos = Mathf.Cos((angle / 2) * Mathf.Deg2Rad);
            cos *= cos;
            targetDir.Normalize();

            foreach (AgentUnit agent in _agents)
            {
                if (agent != null && agent.isActiveAndEnabled)
                {
                    Vector3 dirVector = agent.transform.position - unitPos;
                    float distance = dirVector.sqrMagnitude;

                    if (distance <= range)
                    {
                        float dot = Vector3.Dot(targetDir, dirVector);

                        if (dot * dot >= cos * distance)
                        {
                            agents.Add(agent);
                        }
                    }
                }
            }

            return agents;
        }

        private List<AgentUnit> GetSortedAgentsInCone(Vector3 unitPos, Vector3 targetDir, float range, int angle)
        {
            PriorityQueue<AgentUnit> priorityQueue = new PriorityQueue<AgentUnit>();

            range *= range;
            float cos = Mathf.Cos((angle / 2) * Mathf.Deg2Rad);
            cos *= cos;
            targetDir.Normalize();

            foreach (AgentUnit agent in _agents)
            {
                if (agent != null && agent.isActiveAndEnabled)
                {
                    Vector3 dirVector = agent.transform.position - unitPos;
                    float distance = dirVector.sqrMagnitude;

                    if (distance <= range)
                    {
                        float dot = Vector3.Dot(targetDir, dirVector);

                        if (dot * dot >= cos * distance)
                        {
                            priorityQueue.Enqueue(agent, distance);
                        }
                    }
                }
            }

            List<AgentUnit> agents = new List<AgentUnit>(priorityQueue.Count);

            while (priorityQueue.Count > 0)
            {
                agents.Add(priorityQueue.Dequeue());
            }

            return agents;
        }

        private List<AgentUnit> GetSortedAgentsInCone(Vector3 unitPos, Vector3 targetDir, float range, float angle, int maxCount)
        {
            PriorityQueue<AgentUnit> priorityQueue = new PriorityQueue<AgentUnit>();

            range *= range;
            float cos = Mathf.Cos((angle / 2) * Mathf.Deg2Rad);
            cos *= cos;
            targetDir.Normalize();

            foreach (AgentUnit agent in _agents)
            {
                if (agent != null && agent.isActiveAndEnabled)
                {
                    Vector3 dirVector = agent.transform.position - unitPos;
                    float distance = dirVector.sqrMagnitude;

                    if (distance <= range)
                    {
                        float dot = Vector3.Dot(targetDir, dirVector);

                        if (dot * dot >= cos * distance)
                        {
                            priorityQueue.Enqueue(agent, distance);

                            if (priorityQueue.Count > maxCount)
                            {
                                priorityQueue.Dequeue();
                            }
                        }
                    }
                }
            }

            List<AgentUnit> agents = new List<AgentUnit>(priorityQueue.Count);

            while (priorityQueue.Count > 0)
            {
                agents.Add(priorityQueue.Dequeue());
            }

            return agents;
        }
        #endregion

        #region 격자 범위 안쪽의 아군 유닛을 반환(2D 전용, X, Z 좌표)
        /// <summary>
        /// 격자 범위 안쪽의 아군 유닛을 반환 (unitPos와 가까운 유닛부터 반환)
        /// </summary>
        internal List<AgentUnit> GetAgentsInGrid(Vector2Int unitCellPos, List<Vector2Int> grid, int maxCount = int.MaxValue)
        {
            if (maxCount == int.MaxValue)
            {
                return GetAllAgentsInGrid(unitCellPos, grid);
            }
            else
            {
                return GetSortedAgentsInGrid(unitCellPos, grid, maxCount);
            }
        }

        private List<AgentUnit> GetAllAgentsInGrid(Vector2Int unitCellPos, List<Vector2Int> grid)
        {
            List<AgentUnit> agents = new List<AgentUnit>();

            foreach (AgentUnit agent in _agents)
            {
                if (agent != null && agent.isActiveAndEnabled)
                {
                    var pos = agent.cellPos - unitCellPos;

                    if (grid.Contains(pos))
                    {
                        agents.Add((agent));
                    }
                }
            }

            return agents;
        }

        private List<AgentUnit> GetSortedAgentsInGrid(Vector2Int unitCellPos, List<Vector2Int> grid)
        {
            PriorityQueue<AgentUnit> priorityQueue = new PriorityQueue<AgentUnit>();

            foreach (AgentUnit agent in _agents)
            {
                if (agent != null && agent.isActiveAndEnabled)
                {
                    var pos = agent.cellPos - unitCellPos;

                    if (grid.Contains(pos))
                    {
                        var distance = (agent.cellPos - unitCellPos).sqrMagnitude;

                        priorityQueue.Enqueue(agent, distance);
                    }
                }
            }

            List<AgentUnit> agents = new List<AgentUnit>(priorityQueue.Count);

            while (priorityQueue.Count > 0)
            {
                agents.Add(priorityQueue.Dequeue());
            }

            return agents;
        }

        private List<AgentUnit> GetSortedAgentsInGrid(Vector2Int unitCellPos, List<Vector2Int> grid, int maxCount)
        {
            PriorityQueue<AgentUnit> priorityQueue = new PriorityQueue<AgentUnit>();

            foreach (AgentUnit agent in _agents)
            {
                if (agent != null && agent.isActiveAndEnabled)
                {
                    var pos = agent.cellPos - unitCellPos;

                    if (grid.Contains(pos))
                    {
                        var distance = (agent.cellPos - unitCellPos).sqrMagnitude;

                        priorityQueue.Enqueue(agent, distance);

                        if (priorityQueue.Count > maxCount)
                        {
                            priorityQueue.Dequeue();
                        }
                    }
                }
            }

            List<AgentUnit> agents = new List<AgentUnit>(priorityQueue.Count);

            while (priorityQueue.Count > 0)
            {
                agents.Add(priorityQueue.Dequeue());
            }

            return agents;
        }
        #endregion

        #region 범위 내 공격 가능한 아군 유닛을 반환
        internal List<AgentUnit> GetAttackableAgents(Vector3 unitPos, float radius, EAttackType attackType, int maxCount = int.MaxValue)
        {
            var agents = GetSortedAgentsInCircle(unitPos, radius);

            return CheckAttackable(agents, attackType, maxCount);
        }

        internal List<AgentUnit> GetAttackableAgents(Vector3 unitPos, Vector3 targetDir, float range, float width, EAttackType attackType, int maxCount = int.MaxValue)
        {
            var agents = GetSortedAgentsInStraight(unitPos, targetDir, range, width);

            return CheckAttackable(agents, attackType, maxCount);
        }

        internal List<AgentUnit> GetAttackableAgents(Vector3 unitPos, Vector3 targetDir, float range, int angle, EAttackType attackType, int maxCount = int.MaxValue)
        {
            var agents = GetSortedAgentsInCone(unitPos, targetDir, range, angle);

            return CheckAttackable(agents, attackType, maxCount);
        }

        internal List<AgentUnit> GetAttackableAgents(Vector2Int unitCellPos, List<Vector2Int> grid, EAttackType attackType, int maxCount = int.MaxValue)
        {
            var agents = GetSortedAgentsInGrid(unitCellPos, grid);

            return CheckAttackable(agents, attackType, maxCount);
        }

        internal List<AgentUnit> GetAllAttackableAgents(EAttackType attackType)
        {
            var agents = new List<AgentUnit>();

            foreach (var agent in _agents)
            {
                if (agent != null && agent.isActiveAndEnabled)
                {
                    // 적이 공중 유닛일 떄, 원거리가 아니라면 공격 불가 (타워 디펜스라면 언덕 유닛일 때, 로 변경)
                    if (agent.template.MoveType == EMoveType.Sky && attackType != EAttackType.Far) continue;

                    // 공격 대상이 아니라면 타겟에 추가하지 않음
                    if (agent.GetAbility<HitAbility>().finalTargetOfAttack == false) continue;

                    agents.Add(agent);
                }
            }

            return agents;
        }

        private List<AgentUnit> CheckAttackable(List<AgentUnit> agents, EAttackType attackType, int maxCount)
        {
            var attackableAgents = new List<AgentUnit>(maxCount);

            foreach (var agent in agents)
            {
                // maxCount만큼 유닛을 찾았다면
                if (attackableAgents.Count >= maxCount) break;

                // 적이 공중 유닛일 떄, 원거리가 아니라면 공격 불가 (타워 디펜스라면 언덕 유닛일 때, 로 변경)
                if (agent.template.MoveType == EMoveType.Sky && attackType != EAttackType.Far) continue;

                // 공격 대상이 아니라면 타겟에 추가하지 않음
                if (agent.GetAbility<HitAbility>().finalTargetOfAttack == false) continue;

                attackableAgents.Add(agent);
            }

            return attackableAgents;
        }
        #endregion

        #region 범위 내 회복 가능한 아군 유닛을 반환
        internal List<AgentUnit> GetHealableAgents(Vector3 unitPos, float radius, int maxCount = int.MaxValue)
        {
            var agents = GetSortedAgentsInCircle(unitPos, radius);

            return CheckHealable(agents, maxCount);
        }

        internal List<AgentUnit> GetHealableAgents(Vector3 unitPos, Vector3 targetDir, float range, float width, int maxCount = int.MaxValue)
        {
            var agents = GetSortedAgentsInStraight(unitPos, targetDir, range, width);

            return CheckHealable(agents, maxCount);
        }

        internal List<AgentUnit> GetHealableAgents(Vector3 unitPos, Vector3 targetDir, float range, int angle, int maxCount = int.MaxValue)
        {
            var agents = GetSortedAgentsInCone(unitPos, targetDir, range, angle);

            return CheckHealable(agents, maxCount);
        }

        internal List<AgentUnit> GetHealableAgents(Vector2Int unitCellPos, List<Vector2Int> grid, int maxCount = int.MaxValue)
        {
            var agents = GetSortedAgentsInGrid(unitCellPos, grid);

            return CheckHealable(agents, maxCount);
        }

        internal List<AgentUnit> GetAllHealableAgents()
        {
            var agents = new List<AgentUnit>();

            foreach (var agent in _agents)
            {
                if (agent != null && agent.isActiveAndEnabled)
                {
                    // 회복 가능 유닛이 아니라면 타겟에 추가하지 않음
                    if (agent.GetAbility<HealthAbility>().finalIsHealAble == false) continue;

                    agents.Add(agent);
                }
            }

            return agents;
        }

        private List<AgentUnit> CheckHealable(List<AgentUnit> agents, int maxCount)
        {
            var healableAgents = new List<AgentUnit>(maxCount);

            foreach (var agent in agents)
            {
                // maxCount만큼 유닛을 찾았다면
                if (healableAgents.Count >= maxCount) break;

                // 회복 가능 유닛이 아니라면 타겟에 추가하지 않음
                if (agent.GetAbility<HealthAbility>().finalIsHealAble == false) continue;

                healableAgents.Add(agent);
            }

            return healableAgents;
        }
        #endregion

        /// <summary>
        /// 범위 내에 가장 가까운 아군 유닛을 반환
        /// </summary>
        internal AgentUnit GetNearestAgent(Vector3 unitPos, float radius)
        {
            AgentUnit agentUnit = null;

            radius *= radius;
            float nearestDistance = Mathf.Infinity;

            foreach (AgentUnit agent in _agents)
            {
                if (agent != null && agent.isActiveAndEnabled)
                {
                    float distance = (agent.transform.position - unitPos).sqrMagnitude;

                    if (distance < nearestDistance && distance <= radius)
                    {
                        agentUnit = agent;
                        nearestDistance = distance;
                    }
                }
            }

            return agentUnit;
        }
        #endregion
    }
}