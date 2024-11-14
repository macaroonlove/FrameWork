using FrameWork.UIBinding;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Temporary.Core
{
    public class SkillTreeCanvas : UIBase
    {
        [SerializeField] private RectTransform _content;
        [SerializeField] private GameObject _nodePrefab;
        [SerializeField] private GameObject _pathPrefab;
        [Space]
        [SerializeField] private int _nodeWidthOffset = 300;
        [SerializeField] private int _nodeHeightOffset = 300;

        private PoolSystem _poolSystem;
        private int _contentHeight;

        private List<UISkillNodeController> _nodes = new List<UISkillNodeController>();
        private List<GameObject> _paths = new List<GameObject>();

        protected override void Initialize()
        {
            if (_poolSystem != null) return;

            _poolSystem = BattleManager.Instance.GetSubSystem<PoolSystem>();
        }

        public void Show(SkillTreeGraph skillTree)
        {
            base.Show();

            // 배경 크기 설정
            InitializeContentHeight(skillTree);

            // 노드 배치
            GenerateNode(skillTree);

            // 선 배치
            GeneratePath(skillTree);
        }

        public override void Hide(bool isForce = false)
        {
            base.Hide(isForce);

            foreach (var node in _nodes)
            {
                _poolSystem.DeSpawn(node.gameObject);
            }
            foreach (var path in _paths)
            {
                _poolSystem.DeSpawn(path);
            }

            _nodes.Clear();
            _paths.Clear();
        }

        private void InitializeContentHeight(SkillTreeGraph skillTree)
        {
            int levelCount = skillTree.nodeLevelCount;

            _contentHeight = _nodeHeightOffset * levelCount + 150 * (levelCount - 1);
            _content.sizeDelta = new Vector2(_content.sizeDelta.x, _contentHeight);
        }

        private void GenerateNode(SkillTreeGraph skillTree)
        {
            var nodeControllerMap = new Dictionary<SkillNode, UISkillNodeController>();

            // 노드 생성
            foreach (var node in skillTree.nodes)
            {
                if (node is SkillNode skill)
                {
                    if (skill.skillTemplate == null) continue;

                    Vector2 position = GetSkillNodePosition(skill.level, skill.index, skillTree.nodeLevelCount, skillTree.nodeIndexCount);
                    Transform trans = _poolSystem.Spawn(_nodePrefab, _content).transform;
                    (trans as RectTransform).anchoredPosition = position;

                    var nodeController = trans.GetComponent<UISkillNodeController>();
                    nodeController.Initialize(skill.skillTemplate);

                    nodeControllerMap[skill] = nodeController;
                    _nodes.Add(nodeController);
                }
            }

            // 연결 설정
            foreach (var node in skillTree.nodes)
            {
                if (node is SkillNode from)
                {
                    var nodeController = nodeControllerMap[from];
                    var connections = from.GetPort("level").GetConnections();

                    foreach (var connection in connections)
                    {
                        if (connection.node is SkillNode to)
                        {
                            if (nodeControllerMap.TryGetValue(to, out var targetController))
                            {
                                nodeController.connections.Add(targetController);
                            }
                        }
                    }
                }
            }
        }


        private void GeneratePath(SkillTreeGraph skillTree)
        {
            foreach (var from in _nodes)
            {
                foreach (var to in from.connections)
                {
                    GameObject path = _poolSystem.Spawn(_pathPrefab, _content);
                    UISkillPathController pathController = path.GetComponent<UISkillPathController>();
                    pathController.Initialize(from.output, to.input, _nodeHeightOffset);
                    _paths.Add(path);
                }
            }
        }

        private Vector2 GetSkillNodePosition(int level, int index, int levelCount, int indexCount)
        {
            float x;

            if (indexCount % 2 == 0)
            {
                float halfCount = indexCount * 0.5f;
                float offset = _nodeWidthOffset * 0.5f;
                x = (halfCount - index) * _nodeWidthOffset - offset;
            }
            else
            {
                float halfCount = (indexCount - 1) * 0.5f;
                x = (halfCount - index) * _nodeWidthOffset;
            }

            float y = (levelCount - level - 2) * _nodeHeightOffset;

            return new Vector2(x, y);
        }
    }
}