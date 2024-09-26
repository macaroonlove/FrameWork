using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Temporary.Core
{
    public class AgentUnit : Unit
    {
        private AgentTemplate _template;

        internal AgentTemplate template => _template;

        internal void Initialize(AgentTemplate template)
        {
            base.Initialize(this);

            _id = template.id;
            _template = template;
        }
    }
}

#if UNITY_EDITOR
namespace Temporary.Editor
{
    using Temporary.Core;
    using UnityEditor;

    [CustomEditor(typeof(AgentUnit))]
    public class AgentUnitEditor : UnitEditor
    {
        protected override void AddAbilityMenu()
        {
            GenericMenu menu = new GenericMenu();

            // ���Ǻ� ����
            //menu.AddItem(new GUIContent("�̵� �ɷ�"), false, AddAbility, typeof(MoveAgentAbility));

            menu.ShowAsContext();
        }
    }
}
#endif