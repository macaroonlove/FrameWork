using UnityEngine;
using UnityEngine.Events;

namespace Temporary.Core
{
    public class Projectile : MonoBehaviour
    {
        [SerializeField] private bool isLookTarget;
        [SerializeField] private float speed;

        private Unit _caster;
        private Unit _target;
        private UnityAction<Unit, Unit> _action;

        // 초기화 여부
        private bool isInit;

        internal void Initialize(Unit caster, Unit target, UnityAction<Unit, Unit> action)
        {
            _caster = caster;
            _target = target;
            _action = action;

            if (target == null || target.isDie)
            {
                DeSpawn();
                return;
            }

            if (isLookTarget)
            {
                transform.GetChild(0).LookAt(_target.projectileHitPoint);
            }

            isInit = true;
        }

        private void Update()
        {
            if (isInit == false) return;

            //날라가는 도중 타겟이 죽었다면
            if (_target.isDie)
            {
                DeSpawn();
                return;
            }

            Move();
        }

        private void Move()
        {
            var projectilePos = this.transform.position;
            var targetPos = _target.projectileHitPoint.position;
            var distance = Vector3.Distance(projectilePos, targetPos);
            var moveDistance = Time.deltaTime * speed;

            // 날라가는 중
            if (distance > moveDistance)
            {
                var dir = (targetPos - projectilePos).normalized;
                var deltaPos = dir * moveDistance;
                this.transform.Translate(deltaPos);
            }
            // 충돌
            else
            {
                OnCollision();
            }
        }

        private void OnCollision()
        {
            _action?.Invoke(_caster, _target);

            DeSpawn();
        }

        private void DeSpawn()
        {
            BattleManager.Instance.GetSubSystem<PoolSystem>().DeSpawn(gameObject);
            isInit = false;
        }
    }
}