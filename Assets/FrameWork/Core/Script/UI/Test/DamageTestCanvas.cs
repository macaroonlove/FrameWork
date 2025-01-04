using FrameWork.UIBinding;
using UnityEngine;

namespace Temporary.Core
{
    public class DamageTestCanvas : UIBase
    {
        #region ¹ÙÀÎµù
        enum Buttons
        {
            PhysicalDamageButton,
            MagicDamageButton,
            TrueDamageButton,
        }
        #endregion

        private Unit _unit;

        internal void Initialize(Unit unit)
        {
            _unit = unit;

            BindButton(typeof(Buttons));
            GetButton((int)Buttons.PhysicalDamageButton).onClick.AddListener(PhysicalDamage);
            GetButton((int)Buttons.MagicDamageButton).onClick.AddListener(MagicDamage);
            GetButton((int)Buttons.TrueDamageButton).onClick.AddListener(TrueDamage);
        }

        private void PhysicalDamage()
        {
            _unit.GetAbility<HitAbility>().Hit(Random.Range(1, 100), EDamageType.PhysicalDamage);
        }

        private void MagicDamage()
        {
            _unit.GetAbility<HitAbility>().Hit(Random.Range(1, 100), EDamageType.MagicDamage);
        }

        private void TrueDamage()
        {
            _unit.GetAbility<HitAbility>().Hit(Random.Range(1, 100));
        }
    }
}