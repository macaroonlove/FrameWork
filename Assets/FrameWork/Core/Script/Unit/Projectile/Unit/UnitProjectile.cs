using UnityEngine.Events;

namespace Temporary.Core
{
    public abstract class UnitProjectile : Projectile
    {
        protected Unit _caster;
        protected Unit _target;

        internal virtual void Initialize(Unit caster, Unit target, UnityAction<Unit, Unit> action)
        {
            _caster = caster;
            _target = target;
            _action = action;

            if (target == null || target.isDie)
            {
                DeSpawn();
                return;
            }

            ExecuteCasterFX(caster);

            if (_isLookTarget)
            {
                transform.GetChild(0).LookAt(_target.projectileHitPoint);
            }

            _isInit = true;
        }

        private void Update()
        {
            if (_isInit == false) return;

            //날라가는 도중 타겟이 죽었다면
            if (_target == null || _target.isDie)
            {
                DeSpawn();
                return;
            }

            Move();
        }

        protected abstract void Move();

        protected void OnCollision()
        {
            _action?.Invoke(_caster, _target);

            ExecuteTargetFX(_target);

            DeSpawn();
        }
    }
}