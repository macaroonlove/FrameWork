using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

namespace Temporary.Core
{
    public class SkillTreeGraphView : GraphView
    {
        public SkillTreeGraphView()
        {
            // �ܰ� �巡�� ��� ����
            SetupZoom(ContentZoomer.DefaultMinScale, ContentZoomer.DefaultMaxScale);

            // �׸��� ��� �߰�
            var grid = new GridBackground();
            Insert(0, grid);
            grid.StretchToParentSize();

            // �⺻ ���� �߰� (�巡��, ����)
            this.AddManipulator(new ContentDragger());
            this.AddManipulator(new SelectionDragger());
            this.AddManipulator(new RectangleSelector());

            // ���� ��� ����
            CreateNode("Root Skill", new Vector2(100, 200));
        }

        public void CreateNode(string nodeName, Vector2 position)
        {
            var node = new Node
            {
                title = nodeName,
                style = { left = position.x, top = position.y }
            };

            // �Է� ��Ʈ �� ��� ��Ʈ �߰�
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
