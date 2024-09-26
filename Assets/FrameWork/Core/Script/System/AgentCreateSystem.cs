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
            // ������ ���� �ҷ����� (������ ��� �Ű������� ���̵� ���� ����)
            var templates = GameDataManager.Instance.ownedAgentTemplate;
            int index = Random.Range(0, templates.Count);
            var template = templates[index];

            // ������ ��ġ ã�� (������ ��� �Ű������� ��ġ�� ����)
            Vector3 pos = new Vector3(-2, 0, -5);

            // ���� �����ϱ�
            var obj = _poolSystem.Spawn(template.prefab, transform);

            // ���� ��ġ �����ֱ� (��ġ�� Ÿ�ϰ� ���� ��� Ÿ�Ͽ��� ��ġ �����ֱ⵵ ����)
            obj.transform.SetPositionAndRotation(pos, Quaternion.identity);

            if (obj.TryGetComponent(out AgentUnit unit))
            {
                // ���� �ʱ�ȭ
                unit.Initialize(template);

                // ���� ���
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