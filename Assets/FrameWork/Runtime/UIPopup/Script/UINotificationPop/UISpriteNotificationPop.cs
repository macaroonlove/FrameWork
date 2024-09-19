using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace FrameWork.UIPopup
{
    public class UISpriteNotificationPop : UINotificationPop
    {
        [SerializeField] private Image _image;

        public void Initialize(string title, string desciption, Sprite sprite, ENotificationType notificationType, UnityAction<RectTransform, ENotificationType> action)
        {
            if (_image != null)
            {
                _image.sprite = sprite;
            }

            base.Initialize(title, desciption, notificationType, action);
        }
    }
}