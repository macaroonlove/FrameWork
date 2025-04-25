using FrameWork.UIBinding;
using UnityEngine;

namespace Temporary.Core
{
    public class BattleTest : UIBase
    {
        [SerializeField] private AgentTemplate _agentTemplate;
        [SerializeField] private AgentTemplate _agentTemplate2;
        [SerializeField] private EnemyTemplate _enemyTemplate;

        private async void Start()
        {
            var damage = GetComponentInChildren<DamageTestCanvas>();
            var health = GetComponentInChildren<HealthTestCanvas>();
            var buff = GetComponentInChildren<BuffTestCanvas>();
            var abnormalStatus = GetComponentInChildren<AbnormalStatusTestCanvas>();
            var activeSkill = GetComponentInChildren<ActiveSkillTestCanvas>();
            var skillTreeTestCanvas = GetComponentInChildren<SkillTreeTestCanvas>();

            await _agentTemplate.LoadSkinBattleTemplate();
            await _agentTemplate2.LoadSkinBattleTemplate();

            //Debug.Log(_agentTemplate.skins[0].lobbyTemplate);
            //Debug.Log(_agentTemplate.skins[0].battleTemplate);

            BattleManager.Instance.InitializeBattle();
            BattleManager.Instance.GetSubSystem<AgentCreateSystem>().CreateUnit(_agentTemplate, Vector3.zero);
            BattleManager.Instance.GetSubSystem<AgentCreateSystem>().CreateUnit(_agentTemplate2, new Vector3(-3, 0, 0));
            BattleManager.Instance.GetSubSystem<EnemySpawnSystem>().SpawnUnit(_enemyTemplate, new Vector3(3, 0, 0));
            BattleManager.Instance.GetSubSystem<EnemySpawnSystem>().SpawnUnit(_enemyTemplate, new Vector3(3, 0, 1));
            BattleManager.Instance.GetSubSystem<EnemySpawnSystem>().SpawnUnit(_enemyTemplate, new Vector3(3, 0, -1));

            BattleManager.Instance.GetSubSystem<UnitRayCastSystem>().onCast += (Unit unit) =>
            {
                damage?.Initialize(unit);
                health?.Initialize(unit);
                buff?.Initialize(unit);
                abnormalStatus?.Initialize(unit);
                activeSkill?.Initialize(unit);
                skillTreeTestCanvas?.Initialize(unit);
            };
        }

        private void OnDestroy()
        {
            _agentTemplate.ReleaseSkinBattleTemplate();
            _agentTemplate2.ReleaseSkinBattleTemplate();
        }
    }
}