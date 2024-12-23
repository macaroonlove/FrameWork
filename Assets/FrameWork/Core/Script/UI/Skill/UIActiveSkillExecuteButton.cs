using FrameWork.UIBinding;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Temporary.Core
{
    /// <summary>
    /// 배치해두고 사용하는 스킬 버튼
    /// </summary>
    public class UIActiveSkillExecuteButton : UIBase
    {
        #region 바인딩
        enum Images
        {
            Icon,
            CooldownTimeImage,
        }
        enum Texts
        {
            CooldownTimeText,
        }
        enum Buttons
        {
            ActiveSkillButton,
        }
        #endregion

        private enum EActionType
        {
            Skill_1,
            Skill_2,
            Skill_3,
            Skill_4,
        }

        [SerializeField] private EActionType _actionType;

        private Image _cooldownTimeImage;

        private TextMeshProUGUI _coolDownTimeText;

        private InputSystem _inputSystem;
        private ManaAbility _manaAbility;

        private Unit _unit;
        private ActiveSkillTemplate _template;

        private float _inverseMaxCoolDownTime;
        private float _currentCoolDownTime;

        private bool _isInteractable;

        #region 프로퍼티
        private bool IsInteractable
        {
            get
            {
                if (_isInteractable == false) return false;

                return true;
            }
        }

        private float finalCoolDownTime
        {
            get
            {
                float result = _template.cooldownTime;

                return result;
            }
        }
        #endregion

        protected override void Initialize()
        {
            BindImage(typeof(Images));
            BindText(typeof(Texts));
            BindButton(typeof(Buttons));

            _cooldownTimeImage = GetImage((int)Images.CooldownTimeImage);
            _coolDownTimeText = GetText((int)Texts.CooldownTimeText);

            _cooldownTimeImage.gameObject.SetActive(false);

            GetButton((int)Buttons.ActiveSkillButton).onClick.AddListener(ExecuteSkill);
        }

        internal void Show(AgentUnit unit, ActiveSkillTemplate template)
        {
            _unit = unit;
            _template = template;

            _inputSystem = BattleManager.Instance.GetSubSystem<InputSystem>();
            _manaAbility = _unit.GetAbility<ManaAbility>();
            _manaAbility.onChangedMana += OnChangeMana;

            CalcMaxCoolDownTime();
            _currentCoolDownTime = 0;

            CheckInteractable();

            InputBinding();
        }

        internal void Hide()
        {
            _unit = null;
            _template = null;

            _manaAbility.onChangedMana -= OnChangeMana;
            _manaAbility = null;

            InputCancelBinding();
            _inputSystem = null;
        }

        #region Input Binding
        private void InputBinding()
        {
            switch (_actionType)
            {
                case EActionType.Skill_1:
                    _inputSystem.onSkill_1 += ExecuteSkill;
                    break;
                case EActionType.Skill_2:
                    _inputSystem.onSkill_2 += ExecuteSkill;
                    break;
                case EActionType.Skill_3:
                    _inputSystem.onSkill_3 += ExecuteSkill;
                    break;
                case EActionType.Skill_4:
                    _inputSystem.onSkill_4 += ExecuteSkill;
                    break;
            }
        }

        private void InputCancelBinding()
        {
            switch (_actionType)
            {
                case EActionType.Skill_1:
                    _inputSystem.onSkill_1 -= ExecuteSkill;
                    break;
                case EActionType.Skill_2:
                    _inputSystem.onSkill_2 -= ExecuteSkill;
                    break;
                case EActionType.Skill_3:
                    _inputSystem.onSkill_3 -= ExecuteSkill;
                    break;
                case EActionType.Skill_4:
                    _inputSystem.onSkill_4 -= ExecuteSkill;
                    break;
            }
        }
        #endregion

        private void OnChangeMana(int mana)
        {
            CheckInteractable();
        }

        #region 쿨타임
        private void CalcMaxCoolDownTime()
        {
            if (finalCoolDownTime == 0)
            {
                _inverseMaxCoolDownTime = 0;
            }
            else
            {
                _inverseMaxCoolDownTime = 1 / finalCoolDownTime;
            }
        }

        private void UpdateCoolDownTime()
        {
            if (_currentCoolDownTime == 0) return;

            _currentCoolDownTime -= Time.deltaTime;

            if (_currentCoolDownTime < 0)
            {
                _currentCoolDownTime = 0;
                _cooldownTimeImage.gameObject.SetActive(false);
                CheckInteractable();
            }
            else
            {
                _coolDownTimeText.text = _currentCoolDownTime.ToString("F1");
                _cooldownTimeImage.fillAmount = _currentCoolDownTime * _inverseMaxCoolDownTime;
            }
        }
        #endregion

        private void CheckInteractable()
        {
            bool isInteractable = true;
            if (_currentCoolDownTime > 0)
            {
                isInteractable = false;
                _isInteractable = false;
                _cooldownTimeImage.gameObject.SetActive(true);
            }
            if (_manaAbility.CheckMana(_template.needMana) == false)
            {
                isInteractable = false;
                _isInteractable = false;
            }

            if (isInteractable == true)
            {
                _isInteractable = true;
                _cooldownTimeImage.gameObject.SetActive(false);
            }
        }

        private void Update()
        {
            UpdateCoolDownTime();
        }

        internal void ExecuteSkill()
        {
            if (IsInteractable == false) return;

            if (_unit.GetAbility<ActiveSkillAbility>().TryExecuteSkill(_template))
            {
                // 쿨타임 적용
                _currentCoolDownTime = finalCoolDownTime;
                CalcMaxCoolDownTime();

                CheckInteractable();
            }
        }
    }
}