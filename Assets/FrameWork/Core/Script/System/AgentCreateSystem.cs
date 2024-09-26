using UnityEngine;

namespace Temporary.Core
{
    public class AgentCreateSystem : MonoBehaviour, ISubSystem
    {
        private AgentSystem _agentSystem;
        private PoolSystem _poolSystem;

        public void Initialize()
        {
            _agentSystem = BattleManager.Instance.GetSubSystem<AgentSystem>();
            _poolSystem = BattleManager.Instance.GetSubSystem<PoolSystem>();
        }

        public void Deinitialize()
        {

        }

        internal bool CreateUnit()
        {
            // 생성할 유닛 불러오기 (선택의 경우 매개변수로 아이디 등을 받음)
            var templates = GameDataManager.Instance.ownedAgentTemplate;
            int index = Random.Range(0, templates.Count);
            var template = templates[index];

            // 생성할 위치 찾기 (선택의 경우 매개변수로 위치를 받음)
            Vector3 pos = new Vector3(-2, 0, -5);

            // 유닛 생성하기
            var obj = _poolSystem.Spawn(template.prefab, transform);

            // 유닛 위치 정해주기 (위치가 타일과 같을 경우 타일에서 위치 정해주기도 가능)
            obj.transform.SetPositionAndRotation(pos, Quaternion.identity);

            if (obj.TryGetComponent(out AgentUnit unit))
            {
                // 유닛 초기화
                unit.Initialize(template);

                // 유닛 등록
                _agentSystem.Regist(unit);
            }
            else
            {
                _poolSystem.DeSpawn(obj);
                return false;
            }

            return true;
        }
    }
}