using UnityEditor;
using UnityEngine;

namespace Temporary.Core
{
    public class ProjectileGetTargetUnitEffect : GetTargetUnitEffect
    {
        [SerializeField] protected GameObject _prefab;
        [SerializeField] protected ESpawnPoint _spawnPoint;

        public override string GetDescription()
        {
            return "투사체 (타겟팅)";
        }

        public override void Execute(Unit casterUnit, Unit targetUnit)
        {
            if (casterUnit == null || targetUnit == null) return;
            if (targetUnit.isDie) return;

            casterUnit.GetAbility<EntitySpawnAbility>().SpawnProjectile(_prefab, _spawnPoint, targetUnit, (caster, target) => { SkillImpact(caster, target); });
        }

#if UNITY_EDITOR
        public override void Draw(Rect rect)
        {
            var labelRect = new Rect(rect.x, rect.y, 140, rect.height);
            var valueRect = new Rect(rect.x + 140, rect.y, rect.width - 140, rect.height);

            GUI.Label(labelRect, "투사체 프리팹");
            _prefab = (GameObject)EditorGUI.ObjectField(valueRect, _prefab, typeof(GameObject), false);

            labelRect.y += 20;
            valueRect.y += 20;
            GUI.Label(labelRect, "투사체 생성 위치");
            _spawnPoint = (ESpawnPoint)EditorGUI.EnumPopup(valueRect, _spawnPoint);

            rect.y += 40;
            rect = _getTargetData.Draw(rect);

            _effectsList?.DoList(rect);
        }

        public override int GetNumRows()
        {
            return _getTargetData.GetNumRows(base.GetNumRows());
        }
#endif
    }
}