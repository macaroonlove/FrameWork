using FrameWork;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Temporary.Core
{
    /// <summary>
    /// 배틀에 관련된 시스템 관리
    /// </summary>
    public class BattleManager : Singleton<BattleManager>
    {
        private Dictionary<Type, ISubSystem> _subSystems = new Dictionary<Type, ISubSystem>();

        internal UnityAction onBattleInitialize;
        internal UnityAction onBattleDeinitialize;

        protected override void Awake()
        {
            base.Awake();

            var systems = this.GetComponentsInChildren<ISubSystem>(true);
            foreach (var system in systems)
            {
                _subSystems.Add(system.GetType(), system);
            }

            // TODO: 포톤을 통해 방에 입장하면 RPC를 통해 target을 AllViaServer로 Ready 여부를 보냄, 두 플레이어가 모두 준비 완료했을 때, 호출하도록 구현
            //InitializeBattle();
        }

        [ContextMenu("배틀시작")]
        public void InitializeBattle()
        {
            foreach (var system in this._subSystems.Values)
            {
                system.Initialize();
            }

            onBattleInitialize?.Invoke();
        }

        public void DeinitializeBattle()
        {
            foreach (var item in _subSystems.Values)
            {
                item.Deinitialize();
            }

            onBattleDeinitialize?.Invoke();
        }

        public T GetSubSystem<T>() where T : ISubSystem
        {
            _subSystems.TryGetValue(typeof(T), out var subSystem);
            return (T)subSystem;
        }
    }
}