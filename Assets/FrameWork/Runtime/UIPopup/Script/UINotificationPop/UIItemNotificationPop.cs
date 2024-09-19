using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace FrameWork.UIPopup
{
    public class UIItemNotificationPop : UINotificationPop
    {
        [SerializeField] private Image _image;

        [SerializeField] private List<Sprite> sprites = new List<Sprite>();

        public override void Initialize(string title, string desciption, ENotificationType notificationType, UnityAction<RectTransform, ENotificationType> action)
        {
            int index = GetIndex(title);

            if (index == -1)
            {
                action?.Invoke(transform as RectTransform, notificationType);
            }

            if (_image != null)
            {
                _image.sprite = sprites[index];
            }

            base.Initialize(title, desciption, notificationType, action);
        }

        private int GetIndex(string title)
        {
            switch (title)
            {
                case "���":
                    return 0;
                default:
                    Debug.LogError($"�����ۿ� {title} �̹����� ��ϵǾ� ���� �ʽ��ϴ�.");
                    return -1;
            }
        }
    }
}