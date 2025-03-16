using CodeStage.AntiCheat.ObscuredTypes;
using FrameWork.UIBinding;
using ScriptableObjectArchitecture;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace FrameWork.UI
{
    public class UIIntVariableTextEffect : UIBase
    {
        #region ¹ÙÀÎµù
        enum Texts
        {
            Value,
        }
        #endregion

        [SerializeField] private ObscuredIntVariable _variable;
        [SerializeField] private int _maxLength;

        private TextMeshProUGUI _valueText;
        private UIIntVariableLogEffect _logEffect;

        protected override void Initialize()
        {
            BindText(typeof(Texts));

            _valueText = GetText((int)Texts.Value);

            _logEffect = GetComponentInChildren<UIIntVariableLogEffect>();
        }

        private void OnEnable()
        {
            if (_variable == null) return;

            _variable.AddListener(OnChangeValue);
            _logEffect?.Initialize(_variable.Value);
            Apply(_variable.Value);
        }

        private void OnDisable()
        {
            if (_variable == null) return;

            _variable.RemoveListener(OnChangeValue);
        }

        private void OnChangeValue(ObscuredInt value)
        {
            _logEffect?.OnChangeValue(value);

            Apply(value);
        }

        private void Apply(int value)
        {
            _valueText.text = value.Format(_maxLength);
        }
    }
}