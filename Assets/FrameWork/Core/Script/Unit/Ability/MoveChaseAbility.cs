using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Temporary.Core
{
    /// <summary>
    /// 유닛을 추적하는 이동 어빌리티
    /// </summary>
    public class MoveChaseAbility : MoveAbility
    {
        private Transform _targetPosition;

        internal override void UpdateAbility()
        {
            if (finalIsMoveAble == false) return;

            // 목표 타겟이 없을 경우
            if (_targetPosition == null)
            {
                if (unit is AgentUnit)
                {
                    // TODO: AgentTemplate에 ChaseRange 추가해서 1 바꿔주기 (추적 사거리)
                    BattleManager.Instance.GetSubSystem<EnemySystem>().GetNearestEnemy(unit.transform.position, 1);
                }
                else if (unit is EnemyUnit)
                {
                    // TODO: EnemyTemplate에 ChaseRange 추가해서 1 바꿔주기 (추적 사거리)
                    BattleManager.Instance.GetSubSystem<AgentSystem>().GetNearestAgent(unit.transform.position, 1);
                }
            }

            #region 이동하기
            if (_targetPosition != null)
            {
                // 장애물이 없을 때, 직진 이동
                transform.position = Vector3.MoveTowards(transform.position, _targetPosition.position, finalMoveSpeed * Time.deltaTime);

                // 3D 환경에서 장애물을 피해 이동
                if (_navMeshAgent != null)
                {
                    _navMeshAgent.speed = finalMoveSpeed;
                    _navMeshAgent.SetDestination(_targetPosition.position);
                }

                // 2D 환경에서 장애물을 피해 이동
                var astar = BattleManager.Instance.GetSubSystem<AStarSystem>();
                // TODO: 경로가 바뀌는 경우(장애물 생성 등)에만 Path를 재탐색하도록 수정
                var path = astar.SearchPath(transform.position, _targetPosition.position, AStarSystem.NodeTag.Ground);
                astar.Move(transform, path, finalMoveSpeed);
            }
            #endregion

            #region 회전하기
            Vector3 direction = (_targetPosition.position - transform.position).normalized;

            // 2D 회전
            FlipUnit(direction);

            // 3D 회전
            //RotateUnit(direction);
            #endregion
        }
    }
}