using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

namespace FrameWork.UIPopup
{
    public class UINotificationPop : MonoBehaviour
    {
        [SerializeField] protected TextMeshProUGUI _title;
        [SerializeField] protected TextMeshProUGUI _desciption;

        public virtual void Initialize(string title, string desciption, ENotificationType notificationType, UnityAction<RectTransform, ENotificationType> action)
        {
            if (_title != null)
            {
                _title.text = title;
            }

            if (_desciption != null)
            {
                _desciption.text = desciption;
            }

            RectTransform rect = transform.GetChild(0) as RectTransform;
            float moveX = rect.sizeDelta.x;

            if (rect != null)
            {
                rect.anchoredPosition = new Vector2(moveX, 0);

                rect.DOLocalMoveX(-moveX, 0.5f).SetRelative(true).SetUpdate(true).OnComplete(() => {
                    DOVirtual.DelayedCall(2f, () =>
                    {
                        rect.DOLocalMoveX(moveX, 0.5f).SetRelative(true).SetUpdate(true).OnComplete(() => {
                            action?.Invoke(transform as RectTransform, notificationType);
                        });
                    }, true);
                });
            }
        }
    }
}