using UnityEditor;
using UnityEngine;

namespace Temporary.Core
{
    public abstract class TrapUnitEffect : UnitEffect
    {
        [SerializeField] protected GameObject _prefab;

        public override void Execute(Unit casterUnit, Unit targetUnit)
        {
            if (casterUnit == null || targetUnit == null) return;
            if (targetUnit.isDie) return;

            casterUnit.GetAbility<EntitySpawnAbility>().SpawnTrap(_prefab, targetUnit.transform.position, (caster, target) => { SkillImpact(caster, target); });
        }

        protected abstract void SkillImpact(Unit casterUnit, Unit targetUnit);

#if UNITY_EDITOR
        protected float lastRectY { get; private set; }

        public override void Draw(Rect rect)
        {
            var labelRect = new Rect(rect.x, rect.y, 140, rect.height);
            var valueRect = new Rect(rect.x + 140, rect.y, rect.width - 140, rect.height);

            GUI.Label(labelRect, "µ£ ÇÁ¸®ÆÕ");
            _prefab = (GameObject)EditorGUI.ObjectField(valueRect, _prefab, typeof(GameObject), false);

            lastRectY = labelRect.y;
        }

        public override int GetNumRows()
        {
            return 1;
        }
#endif
    }
}