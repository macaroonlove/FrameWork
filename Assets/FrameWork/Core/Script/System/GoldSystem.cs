using ScriptableObjectArchitecture;
using UnityEngine;
using UnityEngine.Events;

namespace Temporary.Core
{
    /// <summary>
    /// 골드를 관리하는 시스템
    /// </summary>
    public class GoldSystem : MonoBehaviour, ISubSystem
    {
        [SerializeField] private ObscuredIntVariable _goldVariable;

        internal int currentGold => _goldVariable.Value;

        internal event UnityAction<int> onChangeGold;

        public void Initialize()
        {
            // TODO: Template에서 시작 골드 받아오도록 수정
            SetGold(100);
        }

        public void Deinitialize()
        {

        }

        internal void AddGold(int gold)
        {
            SetGold(_goldVariable.Value + gold);
        }

        private void SetGold(int gold)
        {
            _goldVariable.SetValue(gold);

            onChangeGold?.Invoke(_goldVariable.Value);
        }
    }
}
