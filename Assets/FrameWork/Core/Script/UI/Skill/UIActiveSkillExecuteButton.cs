using FrameWork.UIBinding;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Temporary.Core
{
    public class UIActiveSkillExecuteButton : UIBase
    {
        enum Images
        {
            Icon,
        }
        enum Texts
        {
            SkillName,
            Cooldown,
        }
        enum Buttons
        {
            SkillButton,
        }

        private ActiveSkillTemplate _template;
        private Unit _unit;

        protected override void Initialize()
        {
            BindImage(typeof(Images));
            BindText(typeof(Texts));
            BindText(typeof(Buttons));

            // ĳ��� ������ ��� �ش� ��ư�� Ŭ���ϸ� ��ų ���
            GetButton((int)Buttons.SkillButton).onClick.AddListener(SkillExecute);
        }

        internal void Show(AgentUnit unit, ActiveSkillTemplate template)
        {
            _unit = unit;
            _template = template;
        }

        private void Update()
        {
            // ��Ÿ�� ���?
        }

        internal void SkillExecute()
        {
            _unit.GetAbility<ActiveSkillAbility>().TryExecuteSkill(_template);
        }

        internal void Hide()
        {
            _unit = null;
            _template = null;
        }
    }
}