using FrameWork.UIBinding;
using FrameWork.UIPopup;
using ScriptableObjectArchitecture;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Temporary.Lobby
{
    public class UIGachaCanvas : UIBase
    {
        #region 바인딩
        enum Images
        {
            Background,
        }
        #endregion

        [SerializeField] private GameObject _prefab;
        [SerializeField] private Transform _parent;

        private UIGachaTab[] _gachaTabs;
        private List<UIGachaButton> _gachaButtons = new List<UIGachaButton>();
        private Image _background;
        private UIGachaResultCanvas _gachaResultCanvas;

        private UIGachaTab _currentTab;

        protected override void Initialize()
        {
            BindImage(typeof(Images));

            _background = GetImage((int)Images.Background);

            _gachaTabs = GetComponentsInChildren<UIGachaTab>();
            _gachaResultCanvas = GetComponentInChildren<UIGachaResultCanvas>();

            foreach (var tab in _gachaTabs)
            {
                tab.Initialize(_gachaResultCanvas, Select);
            }

            Select(_gachaTabs[0]);
        }

        private void SetButtons(UIGachaTab tab)
        {
            var infos = tab.gachaButtonInfos;
            int count = infos.Count;

            // 필요한 수만큼 생성
            while (_gachaButtons.Count < count)
            {
                var instance = Instantiate(_prefab, _parent);
                var toggle = instance.GetComponent<UIGachaButton>();
                _gachaButtons.Add(toggle);
            }

            // 초기화
            for (int i = 0; i < _gachaButtons.Count; i++)
            {
                if (i < count)
                {
                    var info = infos[i];
                    _gachaButtons[i].Show(info.gachaCount, info.needCount, info.color, info.variable, PickUp);
                }
                else
                {
                    _gachaButtons[i].Hide();
                }
            }
        }

        private void Select(UIGachaTab tab)
        {
            _currentTab?.UnSelect();
            _currentTab = tab;
            _currentTab?.Select();
            
            _background.sprite = tab.background;
            SetButtons(tab);
        }

        private void PickUp(int gachaCount, int needCount, ObscuredIntVariable variable)
        {
            UIPopupManager.Instance.ShowConfirmCancelPopup($"{variable.DisplayName} {needCount}개를 사용하여 유닛을 획득하시겠습니까?", (result) => 
            { 
                if (result == true)
                {
                    // 해당 종류의 변수(Variable)의 개수가 충분하다면
                    if (variable.Value >= needCount)
                    {
                        _currentTab.PickUp(gachaCount);

                        variable.AddValue(-needCount);
                    }
                    else
                    {
                        UIPopupManager.Instance.ShowConfirmPopup($"{variable.DisplayName}가 부족합니다.");
                    }
                }
            });
        }
    }
}