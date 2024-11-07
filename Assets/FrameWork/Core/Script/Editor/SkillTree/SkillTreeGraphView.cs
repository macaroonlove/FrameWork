using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

namespace Temporary.Core
{
    public class SkillTreeGraphView : GraphView
    {
        public SkillTreeGraphView()
        {
            // 줌과 드래그 기능 설정
            SetupZoom(ContentZoomer.DefaultMinScale, ContentZoomer.DefaultMaxScale);

            // 그리드 배경 추가
            var grid = new GridBackground();
            Insert(0, grid);
            grid.StretchToParentSize();

            // 기본 조작 추가 (드래그, 선택)
            this.AddManipulator(new ContentDragger());
            this.AddManipulator(new SelectionDragger());
            this.AddManipulator(new RectangleSelector());

            // 예시 노드 생성
            CreateNode("Root Skill", new Vector2(100, 200));
        }

        public void CreateNode(string nodeName, Vector2 position)
        {
            var node = new Node
            {
                title = nodeName,
                style = { left = position.x, top = position.y }
            };

            // 입력 포트 및 출력 포트 추가
            var inputPort = node.InstantiatePort(Orientation.Vertical, Direction.Input, Port.Capacity.Single, typeof(float));
            inputPort.portName = "Input";
            node.inputContainer.Add(inputPort);

            var outputPort = node.InstantiatePort(Orientation.Vertical, Direction.Output, Port.Capacity.Multi, typeof(float));
            outputPort.portName = "Output";
            node.outputContainer.Add(outputPort);

            node.RefreshExpandedState();
            node.RefreshPorts();
            AddElement(node);
        }
    }
}
