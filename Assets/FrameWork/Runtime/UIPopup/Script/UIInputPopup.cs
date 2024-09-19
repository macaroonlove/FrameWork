using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace FrameWork.UIPopup
{
    public class UIInputPopup : UIPopup
    {
        [SerializeField] private TextMeshProUGUI _text;
        [SerializeField] private TMP_InputField _inputField;
        [SerializeField] private Button _buttonConfirm;
        [SerializeField] private Button _buttonCancel;

        public event UnityAction<string> OnResult;

        protected override void Awake()
        {
            base.Awake();

            if (_buttonConfirm != null)
            {
                _buttonConfirm.onClick.AddListener(OnConfirm);
            }

            if (_buttonCancel != null)
            {
                _buttonCancel.onClick.AddListener(OnCancel);
            }
        }

        public void Show(string context)
        {
            if (_text != null)
            {
                _text.text = context;
            }

            base.Show();
        }

        private void OnConfirm()
        {
            Hide();

            OnResult?.Invoke(_inputField.text);
            OnResult = null;
        }

        private void OnCancel()
        {
            Hide();

            OnResult = null;
        }
    }
}