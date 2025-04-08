using UnityEditor;
using UnityEngine;

namespace Temporary.Core
{
    public class TrapUnitEffect : UnitEffect
    {
        [SerializeField] protected GameObject _prefab;

        public override string GetDescription()
        {
            return "µ£";
        }

        public override void Execute(Unit casterUnit, Unit targetUnit)
        {
            if (casterUnit == null || targetUnit == null) return;
            if (targetUnit.isDie) return;

            casterUnit.GetAbility<EntitySpawnAbility>().SpawnTrap(_prefab, targetUnit.transform.position, (caster, target) => { SkillImpact(caster, target); });
        }

#if UNITY_EDITOR
        public override void Draw(Rect rect)
        {
            var labelRect = new Rect(rect.x, rect.y, 140, rect.height);
            var valueRect = new Rect(rect.x + 140, rect.y, rect.width - 140, rect.height);

            GUI.Label(labelRect, "µ£ ÇÁ¸®ÆÕ");
            _prefab = (GameObject)EditorGUI.ObjectField(valueRect, _prefab, typeof(GameObject), false);

            rect.y += 20;
            _effectsList?.DoList(rect);
        }
#endif
    }
}