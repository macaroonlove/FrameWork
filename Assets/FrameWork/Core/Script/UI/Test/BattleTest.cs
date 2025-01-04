using FrameWork.UIBinding;
using UnityEngine;

namespace Temporary.Core
{
    public class BattleTest : UIBase
    {
        [SerializeField] private AgentTemplate _agentTemplate;

        private void Start()
        {
            var damage = GetComponentInChildren<DamageTestCanvas>();
            var health = GetComponentInChildren<HealthTestCanvas>();
            var abnormalStatus = GetComponentInChildren<AbnormalStatusTestCanvas>();

            BattleManager.Instance.InitializeBattle();
            BattleManager.Instance.GetSubSystem<AgentSystem>().onRegist += (Unit unit) =>
            {
                damage?.Initialize(unit);
                health?.Initialize(unit);
                abnormalStatus?.Initialize(unit);
            };
            BattleManager.Instance.GetSubSystem<AgentCreateSystem>().CreateUnit(_agentTemplate);
        }
    }
}