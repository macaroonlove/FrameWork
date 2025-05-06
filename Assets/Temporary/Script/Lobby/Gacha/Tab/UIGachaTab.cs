using DG.Tweening;
using FrameWork.Editor;
using FrameWork.UIBinding;
using ScriptableObjectArchitecture;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Temporary.Lobby
{
    [System.Serializable]
    public class GachaButtonInfomation
    {
        [Label("필요 재화 종류")] public ObscuredIntVariable variable;
        [Label("필요 재화 수")] public int needCount;
        [Label("뽑을 횟수")] public int gachaCount;
        [Label("버튼 색")] public Color color = new Color(1, 1, 1, 1);
    }

    public abstract class UIGachaTab : UIBase
    {
        [SerializeField, Label("배경 이미지")] private Sprite _background;
        [SerializeField] private List<GachaButtonInfomation> _gachaButtonInfos = new List<GachaButtonInfomation>();
        [SerializeField, Label("천장 변수")] protected ObscuredIntVariable _confirmedPickUpVariable;

        protected UIGachaResultCanvas _gachaResultCanvas;
        private UnityAction<UIGachaTab> _onSelect;

        internal Sprite background => _background;
        internal IReadOnlyList<GachaButtonInfomation> gachaButtonInfos => _gachaButtonInfos;

        protected override void Initialize()
        {
            if (TryGetComponent(out Button button))
            {
                button.onClick.AddListener(OnClick);
            }
        }

        internal void Initialize(UIGachaResultCanvas gachaResultCanvas, UnityAction<UIGachaTab> onSelect)
        {
            _gachaResultCanvas = gachaResultCanvas;
            _onSelect = onSelect;
        }

        private void OnClick()
        {
            _onSelect?.Invoke(this);
        }

        internal virtual void Select()
        {
            transform.DOLocalMoveX(0, 0.5f);
        }

        internal virtual void UnSelect()
        {
            transform.DOLocalMoveX(-20, 0.5f);
        }

        internal abstract void PickUp(int gachaCount);
    }
}