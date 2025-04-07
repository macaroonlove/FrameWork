using FrameWork.Editor;
using UnityEngine;
using UnityEngine.Events;
using VInspector;

namespace Temporary.Core
{
    public class Trap : MonoBehaviour
    {
        [SerializeField, Label("감지 대상")] private LayerMask _layerMask;
        [SerializeField, Label("시전자 사망시 파괴 여부")] private bool _isDespawnCasterDie;
        [Space(10)]
        [SerializeField, Label("범위 적용 여부")] private bool _isSplash;
        [ShowIf("_isSplash")]
        [SerializeField] private GetTargetData _getTargetData;
        [EndIf]
        [Space(10)]
        [SerializeField, Label("감지 시, 호출될 FX")] private FX _collisionFX;

        private UnityAction<Unit, Unit> _action;
        private Unit _caster;

        internal virtual void Initialize(Unit caster, UnityAction<Unit, Unit> action)
        {
            _caster = caster;
            _action = action;

            if (_isDespawnCasterDie)
            {
                _caster.healthAbility.onDeath += DeSpawn;
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            if (((1 << other.gameObject.layer) & _layerMask) != 0)
            {
                if (other.TryGetComponent(out Unit targetUnit))
                {
                    OnCollision(targetUnit);
                }
            }
        }

        private void OnCollision(Unit target)
        {
            if (_isSplash)
            {
                var targets = _getTargetData.GetTarget(target);

                for (int i = 0; i < targets.Count; i++)
                {
                    _action?.Invoke(_caster, targets[i]);
                }
            }
            else
            {
                _action?.Invoke(_caster, target);
            }

            ExecuteTargetFX(target);

            DeSpawn();
        }

        protected void DeSpawn()
        {
            if (_isDespawnCasterDie)
            {
                _caster.healthAbility.onDeath -= DeSpawn;
            }

            CoreManager.Instance.GetSubSystem<PoolSystem>().DeSpawn(gameObject);
        }

        #region FX
        protected void ExecuteTargetFX(Unit target)
        {
            if (_collisionFX != null)
            {
                _collisionFX.Play(target.transform.position);
            }
        }
        #endregion
    }
}
