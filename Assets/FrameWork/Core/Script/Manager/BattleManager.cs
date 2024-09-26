using FrameWork;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Temporary.Core
{
    /// <summary>
    /// ��Ʋ�� ���õ� �ý��� ����
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

            // TODO: ������ ���� �濡 �����ϸ� RPC�� ���� target�� AllViaServer�� Ready ���θ� ����, �� �÷��̾ ��� �غ� �Ϸ����� ��, ȣ���ϵ��� ����
            //InitializeBattle();
        }

        [ContextMenu("��Ʋ����")]
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