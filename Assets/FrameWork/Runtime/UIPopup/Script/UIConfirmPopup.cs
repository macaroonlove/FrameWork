using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace FrameWork.UIPopup
{
    public class UIConfirmPopup : UIPopup
    {
        [SerializeField] private TextMeshProUGUI _text;
        [SerializeField] private Button _button;

        public event UnityAction OnResult;

        protected override void Awake()
        {
            base.Awake();

            if (_button != null)
            {
                _button.onClick.AddListener(() => Hide());
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

        public override void Hide(bool isForce = false)
        {
            base.Hide(isForce);

            OnResult?.Invoke();
            OnResult = null;
        }
    }
}