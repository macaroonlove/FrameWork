using FrameWork.UIBinding;
using Temporary.Core;
using TMPro;
using UnityEngine.UI;

namespace Temporary.Lobby
{
    public class UIGachaResultItem : UIBase
    {
        #region 바인딩
        enum Images
        {
            GachaResultItem,
            Icon,
        }
        enum Texts
        {
            DisplayName,
        }
        #endregion

        private Image _background;
        private Image _icon;
        private TextMeshProUGUI _displayName;

        protected override void Initialize()
        {
            BindImage(typeof(Images));
            BindText(typeof(Texts));

            _background = GetImage((int)Images.GachaResultItem);
            _icon = GetImage((int)Images.Icon);
            _displayName = GetText((int)Texts.DisplayName);
        }

        internal void Show(AgentTemplate template)
        {
            // TODO: 등급마다 배경이 달라야한다면 주석 해제
            //_background.sprite = template.rarity.sprite;
            _icon.sprite = template.sprite;
            _displayName.text = template.displayName;
        }
    }
}