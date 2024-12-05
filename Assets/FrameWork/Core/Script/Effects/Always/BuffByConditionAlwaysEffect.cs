using UnityEditor;
using UnityEngine;

namespace Temporary.Core
{
    public class BuffByConditionAlwaysEffect : AlwaysEffect
    {
        [SerializeField] private EUnitType _unitType;
        [SerializeField] private BuffTemplate _buff;

        public override string GetDescription()
        {
            string unitLabel = "모든";
            if (_unitType == EUnitType.Agent)
            {
                unitLabel = "아군";
            }
            else if (_unitType == EUnitType.Enemy)
            {
                unitLabel = "적군";
            }

            return $"{unitLabel} 유닛에게 무한 지속 버프 적용";
        }

        public override void Execute(Unit casterUnit)
        {
            if (casterUnit == null) return;
            if (UnitCondition(casterUnit) == false) return;
            // 조건 추가

            casterUnit.GetAbility<BuffAbility>().ApplyBuff(_buff, int.MaxValue);
        }

        private bool UnitCondition(Unit unit)
        {
            if (_unitType == EUnitType.All)
            {
                return true;
            }
            if (_unitType == EUnitType.Agent && unit is AgentUnit)
            {
                return true;
            }
            if (_unitType == EUnitType.Enemy && unit is EnemyUnit)
            {
                return true;
            }

            return false;
        }

#if UNITY_EDITOR
        public override void Draw(Rect rect)
        {
            var labelRect = new Rect(rect.x, rect.y, 140, rect.height);
            var valueRect = new Rect(rect.x + 140, rect.y, rect.width - 140, rect.height);

            GUI.Label(labelRect, "유닛 타입");
            _unitType = (EUnitType)EditorGUI.EnumPopup(valueRect, _unitType);

            labelRect.y += 20;
            valueRect.y += 20;
            GUI.Label(labelRect, "버프");
            _buff = (BuffTemplate)EditorGUI.ObjectField(valueRect, _buff, typeof(BuffTemplate), false);
        }

        public override int GetNumRows()
        {
            return 2;
        }
#endif
    }
}