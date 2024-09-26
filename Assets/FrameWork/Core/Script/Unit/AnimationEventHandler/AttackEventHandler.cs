using UnityEngine;
using UnityEngine.Events;

namespace Temporary.Core
{
    public class AttackEventHandler : MonoBehaviour
    {
        internal UnityAction onAttack;

        public void AttackEvent()
        {
            onAttack?.Invoke();
        }
    }
}