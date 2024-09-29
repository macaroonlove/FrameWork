using DG.Tweening;
using FrameWork.Editor;
using UnityEngine;
using UnityEngine.AI;

namespace Temporary.Core
{
    public class MoveAbility : AlwaysAbility
    {
        [SerializeField, ReadOnly] private float _baseMoveSpeed;

        private BuffAbility _buffAbility;
        private AbnormalStatusAbility _abnormalStatusAbility;

        #region 3D의 경우 (2D의 경우 삭제)
        protected NavMeshAgent _navMeshAgent;
        #endregion

        #region 계산 스탯
        protected float finalMoveSpeed
        {
            get
            {
                float result = _baseMoveSpeed;

                #region 증가·감소
                float increase = 1;

                foreach (var effect in _buffAbility.MoveIncreaseDataEffects)
                {
                    increase += effect.value;
                }

                foreach (var effect in _abnormalStatusAbility.MoveIncreaseDataEffects)
                {
                    increase += effect.value;
                }

                result *= increase;
                #endregion

                #region 상승·하락
                foreach (var effect in _buffAbility.MoveMultiplierDataEffects)
                {
                    result *= effect.value;
                }
                #endregion

                return result;
            }
        }

        protected bool finalIsMoveAble
        {
            get
            {
                // 이동 불가 상태이상에 걸렸다면
                if (_abnormalStatusAbility.UnableToMoveEffects.Count > 0) return false;

                return true;
            }
        }
        #endregion

        internal override void Initialize(Unit unit)
        {
            base.Initialize(unit);

            TryGetComponent(out _navMeshAgent);

            _buffAbility = unit.GetAbility<BuffAbility>();
            _abnormalStatusAbility = unit.GetAbility<AbnormalStatusAbility>();

            if (unit is AgentUnit agentUnit)
            {
                _baseMoveSpeed = agentUnit.template.MoveSpeed;
            }
            else if (unit is EnemyUnit enemyUnit)
            {
                _baseMoveSpeed = enemyUnit.template.MoveSpeed;
            }
        }

        #region 회전
        #region 2D 회전
        private bool IsUnitLeft(Vector3 direction)
        {
            Vector3 unitRight = unit.transform.forward;
            float angle = Vector3.SignedAngle(direction, unitRight, Vector3.up);

            return angle > 0f;
        }

        protected void FlipUnit(Vector3 direction)
        {
            bool isLeft = IsUnitLeft(direction);

            float scaleX = isLeft ? 1f : -1f;
            transform.GetChild(1).DOScaleX(scaleX, 0.1f);
        }
        #endregion

        #region 3D 회전
        protected void RotateUnit(Vector3 direction)
        {
            if (direction != Vector3.zero)
            {
                Quaternion targetRotation = Quaternion.LookRotation(direction);
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * 2);
            }
        }
        #endregion
        #endregion
    }
}