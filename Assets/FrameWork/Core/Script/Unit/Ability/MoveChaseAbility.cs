using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Temporary.Core
{
    /// <summary>
    /// ������ �����ϴ� �̵� �����Ƽ
    /// </summary>
    public class MoveChaseAbility : MoveAbility
    {
        private Transform _targetPosition;

        internal override void UpdateAbility()
        {
            if (finalIsMoveAble == false) return;

            // ��ǥ Ÿ���� ���� ���
            if (_targetPosition == null)
            {
                if (unit is AgentUnit)
                {
                    // TODO: AgentTemplate�� ChaseRange �߰��ؼ� 1 �ٲ��ֱ� (���� ��Ÿ�)
                    BattleManager.Instance.GetSubSystem<EnemySystem>().GetNearestEnemy(unit.transform.position, 1);
                }
                else if (unit is EnemyUnit)
                {
                    // TODO: EnemyTemplate�� ChaseRange �߰��ؼ� 1 �ٲ��ֱ� (���� ��Ÿ�)
                    BattleManager.Instance.GetSubSystem<AgentSystem>().GetNearestAgent(unit.transform.position, 1);
                }
            }

            #region �̵��ϱ�
            if (_targetPosition != null)
            {
                // ��ֹ��� ���� ��, ���� �̵�
                transform.position = Vector3.MoveTowards(transform.position, _targetPosition.position, finalMoveSpeed * Time.deltaTime);

                // 3D ȯ�濡�� ��ֹ��� ���� �̵�
                if (_navMeshAgent != null)
                {
                    _navMeshAgent.speed = finalMoveSpeed;
                    _navMeshAgent.SetDestination(_targetPosition.position);
                }

                // 2D ȯ�濡�� ��ֹ��� ���� �̵�
                var astar = BattleManager.Instance.GetSubSystem<AStarSystem>();
                // TODO: ��ΰ� �ٲ�� ���(��ֹ� ���� ��)���� Path�� ��Ž���ϵ��� ����
                var path = astar.SearchPath(transform.position, _targetPosition.position, AStarSystem.NodeTag.Ground);
                astar.Move(transform, path, finalMoveSpeed);
            }
            #endregion

            #region ȸ���ϱ�
            Vector3 direction = (_targetPosition.position - transform.position).normalized;

            // 2D ȸ��
            FlipUnit(direction);

            // 3D ȸ��
            //RotateUnit(direction);
            #endregion
        }
    }
}