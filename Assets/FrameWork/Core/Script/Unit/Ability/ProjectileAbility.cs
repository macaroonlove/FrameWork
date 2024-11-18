using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Temporary.Core
{
    public class ProjectileAbility : AlwaysAbility
    {
        [SerializeField] private Transform _projectileSpawnPoint;

        private PoolSystem _poolSystem;

        internal override void Initialize(Unit unit)
        {
            base.Initialize(unit);

            _poolSystem = BattleManager.Instance.GetSubSystem<PoolSystem>();
        }

        internal override void Deinitialize()
        {
            _poolSystem = null;
        }

        internal void SpawnProjectile(GameObject prefab, Unit targetUnit, UnityAction<Unit, Unit> action)
        {
            var projectile = _poolSystem.Spawn(prefab).GetComponent<Projectile>();
            projectile.transform.SetPositionAndRotation(_projectileSpawnPoint.position, Quaternion.identity);
            projectile.Initialize(unit, targetUnit, action);
        }
    }
}
