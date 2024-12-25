using System.Collections.Generic;
using UnityEngine;

namespace Temporary.Core
{
    public class FXAbility : AlwaysAbility
    {
        private PoolSystem _poolSystem;

        private List<GameObject> _fxObjectList = new List<GameObject>();

        internal override void Initialize(Unit unit)
        {
            base.Initialize(unit);

            _poolSystem = BattleManager.Instance.GetSubSystem<PoolSystem>();

            unit.healthAbility.onDeath += DespawnAll;
        }

        internal override void Deinitialize()
        {
            unit.healthAbility.onDeath -= DespawnAll;
        }

        private void OnDestroy()
        {
            DespawnAll();

            _poolSystem = null;
        }

        internal void AddFX(GameObject fxObj)
        {
            if (!_fxObjectList.Contains(fxObj))
            {
                _fxObjectList.Add(fxObj);
            }
        }

        private void DespawnAll()
        {
            for (int i = _fxObjectList.Count - 1; i >= 0; i--)
            {
                if (_fxObjectList[i] == null || _fxObjectList[i].activeSelf == false)
                {
                    continue;
                }

                _poolSystem.DeSpawn(_fxObjectList[i]);
            }

            _fxObjectList.Clear();
        }
    }
}