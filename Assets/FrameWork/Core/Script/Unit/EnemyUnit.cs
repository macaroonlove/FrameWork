using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Temporary.Core
{
    public class EnemyUnit : Unit
    {
        private EnemyTemplate _template;

        internal EnemyTemplate template => _template;

        internal void Initialize(EnemyTemplate template)
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

    [CustomEditor(typeof(EnemyUnit))]
    public class EnemyUnitEditor : UnitEditor
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
