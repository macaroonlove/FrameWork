using FrameWork.Editor;
using UnityEngine;
using UnityEngine.Events;

namespace Temporary.Core
{
    public class Projectile : MonoBehaviour
    {
        [SerializeField, Label("적을 바라볼지 여부")] protected bool _isLookTarget;

        protected bool _isInit;
        protected UnityAction<Unit, Unit> _action;
        
        protected void DeSpawn()
        {
            BattleManager.Instance.GetSubSystem<PoolSystem>().DeSpawn(gameObject);
            _isInit = false;
        }
    }
}