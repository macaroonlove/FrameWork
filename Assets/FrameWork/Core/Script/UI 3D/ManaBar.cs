using FrameWork.UIBinding;
using UnityEngine.UI;

namespace Temporary.Core
{
    public class ManaBar : UIBase
    {
        #region ¹ÙÀÎµù
        enum Images
        {
            Mana_Fill,
        }
        #endregion

        private ManaAbility _manaAbility;

        private Image _mana;

        protected override void Awake()
        {
            base.Awake();

            BindImage(typeof(Images));
            _mana = GetImage((int)Images.Mana_Fill);

            _manaAbility = GetComponentInParent<ManaAbility>();
            if (_manaAbility.finalMaxMana <= 0)
            {
                Hide();
            }
            else
            {
                _manaAbility.onChangedMana += OnChangedMana;
                Show();
            }
        }

        private void OnDestroy()
        {
            _manaAbility.onChangedMana -= OnChangedMana;
        }

        private void OnChangedMana(int mana)
        {
            var maxHp = _manaAbility.finalMaxMana;
            var per = mana / (float)maxHp;
            _mana.fillAmount = per;
        }
    }
}