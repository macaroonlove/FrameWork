using FrameWork.UIBinding;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace Temporary.Core
{
    public class StatusTestButton : UIBase
    {
        #region ¹ÙÀÎµù
        enum Buttons
        {
            StatusApplyButton,
            PrevButton,
            NextButton,
        }

        enum Texts
        {
            StatusName,
        }
        #endregion

        [SerializeField] private AgentTemplate _agentTemplate;
        [SerializeField] private List<AbnormalStatusTemplate> _abnormalStatusList = new List<AbnormalStatusTemplate>();

        private Unit _unit;
        private AbnormalStatusTemplate _template;
        private TextMeshProUGUI _statusNameText;
        private int _index = 0;

        private void Start()
        {
            BindButton(typeof(Buttons));
            BindText(typeof(Texts));
            GetButton((int)Buttons.StatusApplyButton).onClick.AddListener(ApplyStatus);
            GetButton((int)Buttons.PrevButton).onClick.AddListener(PrevButton);
            GetButton((int)Buttons.NextButton).onClick.AddListener(NextButton);
            _statusNameText = GetText((int)Texts.StatusName);

            BattleManager.Instance.InitializeBattle();
            BattleManager.Instance.GetSubSystem<AgentSystem>().onRegist += (Unit unit) =>
            {
                _unit = unit;
            };
            BattleManager.Instance.GetSubSystem<AgentCreateSystem>().CreateUnit(_agentTemplate);

            UpdateTemplate();
        }

        private void UpdateTemplate()
        {
            _template = _abnormalStatusList[_index];
            _statusNameText.text = _template.displayName;
        }

        private void PrevButton()
        {
            _index = (_index - 1) % _abnormalStatusList.Count;
            UpdateTemplate();
        }

        private void NextButton()
        {
            _index = (_index + 1) % _abnormalStatusList.Count;

            UpdateTemplate();
        }

        private void ApplyStatus()
        {
            _unit.GetAbility<AbnormalStatusAbility>().ApplyAbnormalStatus(_template, 3f);
        }
    }
}