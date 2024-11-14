using FrameWork.UIBinding;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Temporary.Core
{
    public class UISkillExecuteButton : UIBase
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

        private SkillTemplate _template;
        private Unit _unit;

        protected override void Initialize()
        {
            BindImage(typeof(Images));
            BindText(typeof(Texts));
            BindText(typeof(Buttons));

            // 캐쥬얼 게임의 경우 해당 버튼을 클릭하면 스킬 사용
            GetButton((int)Buttons.SkillButton).onClick.AddListener(SkillExecute);
        }

        internal void Show(AgentUnit unit, SkillTemplate template)
        {
            _unit = unit;
            _template = template;
        }

        private void Update()
        {
            // 쿨타임 계산?
        }

        internal void SkillExecute()
        {
            _unit.GetAbility<SkillAbility>().TryExecuteSkill(_template);
        }

        internal void Hide()
        {
            _unit = null;
            _template = null;
        }
    }
}