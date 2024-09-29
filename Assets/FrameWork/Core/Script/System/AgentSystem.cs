using FrameWork.Editor;
using System.Collections.Generic;
using UnityEngine;

namespace Temporary.Core
{
    /// <summary>
    /// 아군 유닛을 관리하는 클래스
    /// (유틸리티 메서드)
    /// </summary>
    public class AgentSystem : MonoBehaviour, ISubSystem
    {
        [SerializeField, ReadOnly] private List<AgentUnit> _agents = new List<AgentUnit>();

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

        /// <summary>
        /// 범위 내에 아군 유닛을 반환 (unitPos와 가까운 유닛부터 반환)
        /// </summary>
        internal List<AgentUnit> GetAgentsInRadius(Vector3 unitPos, float radius, int maxCount = int.MaxValue)
        {
            List<AgentUnit> agents = new List<AgentUnit>();
            List<(AgentUnit agent, float distance)> agentsWithDistance = new List<(AgentUnit, float)>();

            radius *= radius;

            foreach (AgentUnit agent in _agents)
            {
                if (agent != null && agent.isActiveAndEnabled)
                {
                    var distance = (agent.transform.position - unitPos).sqrMagnitude;

                    if (distance <= radius)
                    {
                        agentsWithDistance.Add((agent, distance));
                    }
                }
            }

            if (agentsWithDistance.Count > maxCount)
            {
                agentsWithDistance.Sort((a, b) => a.distance.CompareTo(b.distance));
            }

            foreach (var (agent, _) in agentsWithDistance)
            {
                if (agents.Count >= maxCount) break;
                agents.Add(agent);
            }

            return agents;
        }

        /// <summary>
        /// 공격 가능한 아군 유닛을 반환
        /// </summary>
        internal List<AgentUnit> GetAttackableAgentsInRadius(Vector3 unitPos, float radius, EAttackType attackType, int maxCount = int.MaxValue)
        {
            List<AgentUnit> agents = new List<AgentUnit>();
            List<(AgentUnit agent, float distance)> agentsWithDistance = new List<(AgentUnit, float)>();

            radius *= radius;

            foreach (AgentUnit agent in _agents)
            {
                if (agent != null && agent.isActiveAndEnabled)
                {
                    // 적이 공중 유닛일 떄, 원거리가 아니라면 공격 불가 (타워 디펜스라면 언덕 유닛일 때, 로 변경)
                    if (agent.template.MoveType == EMoveType.Sky && attackType != EAttackType.Far) continue;
                    // 공격 대상이 아니라면 타겟에 추가하지 않음
                    if (agent.GetAbility<HitAbility>().finalTargetOfAttack == false) continue;

                    var distance = (agent.transform.position - unitPos).sqrMagnitude;

                    if (distance <= radius)
                    {
                        agentsWithDistance.Add((agent, distance));
                    }
                }
            }

            if (agentsWithDistance.Count > maxCount)
            {
                agentsWithDistance.Sort((a, b) => a.distance.CompareTo(b.distance));
            }

            foreach (var (agent, _) in agentsWithDistance)
            {
                if (agents.Count >= maxCount) break;
                agents.Add(agent);
            }

            return agents;
        }

        /// <summary>
        /// 회복 가능한 아군 유닛을 반환
        /// </summary>
        internal List<AgentUnit> GetHealableAgentsInRadius(Vector3 unitPos, float radius, int maxCount = int.MaxValue)
        {
            List<AgentUnit> agents = new List<AgentUnit>();
            List<(AgentUnit agent, float distance)> agentsWithDistance = new List<(AgentUnit, float)>();

            radius *= radius;

            foreach (AgentUnit agent in _agents)
            {
                if (agent != null && agent.isActiveAndEnabled)
                {
                    // 회복 가능 유닛이 아니라면 타겟에 추가하지 않음
                    if (agent.GetAbility<HealthAbility>().finalIsHealAble == false) continue;

                    var distance = (agent.transform.position - unitPos).sqrMagnitude;

                    if (distance <= radius)
                    {
                        agentsWithDistance.Add((agent, distance));
                    }
                }
            }

            if (agentsWithDistance.Count > maxCount)
            {
                agentsWithDistance.Sort((a, b) => a.distance.CompareTo(b.distance));
            }

            foreach (var (agent, _) in agentsWithDistance)
            {
                if (agents.Count >= maxCount) break;
                agents.Add(agent);
            }

            return agents;
        }

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