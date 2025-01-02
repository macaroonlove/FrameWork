using FrameWork.UIBinding;
using UnityEngine;
using UnityEngine.UI;

namespace Temporary.Core
{
    public class HPBar : UIBase
    {
        #region ¹ÙÀÎµù
        enum Images
        {
            HP_Fill,
            Shield_Fill,
        }
        #endregion

        private HealthAbility _healthAbility;

        private Image _hp;
        private Image _shield;
        private Vector2 _shieldPos = new Vector2(0, -1.6f);

        protected override void Awake()
        {
            base.Awake();

            BindImage(typeof(Images));
            _hp = GetImage((int)Images.HP_Fill);
            _shield = GetImage((int)Images.Shield_Fill);

            _healthAbility = GetComponentInParent<HealthAbility>();
            _healthAbility.onChangedHealth += OnChangedHealth;
            _healthAbility.onChangedShield += OnChangedShield;
            _healthAbility.onDeath += OnDeath;
        }

        private void OnEnable()
        {
            Show();
        }

        private void OnDestroy()
        {
            _healthAbility.onChangedHealth -= OnChangedHealth;
            _healthAbility.onChangedShield -= OnChangedShield;
            _healthAbility.onDeath -= OnDeath;
        }

        private void OnChangedHealth(int hp)
        {
            var maxHp = _healthAbility.finalMaxHP;
            var per = hp / (float)maxHp;
            _hp.fillAmount = per;
        }

        private void OnChangedShield(int shield)
        {
            var maxHp = _healthAbility.finalMaxHP;
            var hp = _healthAbility.currentHP;
            var per = shield / (float)maxHp;
            var per2 = hp / (float)maxHp;
            _shield.fillAmount = per;

            if ((maxHp - hp) >= shield)
            {
                _shieldPos.x = per2 * 70.4f;
                _shield.rectTransform.anchoredPosition = _shieldPos;
                _shield.fillOrigin = 0;
            }
            else
            {
                _shieldPos.x = 0;
                _shield.rectTransform.anchoredPosition = _shieldPos;
                _shield.fillOrigin = 1;
            }
        }

        private void OnDeath()
        {
            Hide();
        }
    }
}