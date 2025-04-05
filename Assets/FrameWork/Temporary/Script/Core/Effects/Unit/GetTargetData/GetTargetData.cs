using FrameWork;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Temporary.Core
{
    [System.Serializable]
    public class GetTargetData
    {
        [SerializeField] private ETarget _targetType;
        [SerializeField] private EUnitType _unitType;
        [SerializeField] private ERangeType _rangeType;

        [SerializeField] private EDirectionType _directionType;
        [SerializeField] private float _range;
        [SerializeField] private float _assistantRange;
        [SerializeField] private TileRangeTemplate _tileRangeTemplate;

        [SerializeField] private int _numberOfTarget;

        public List<Unit> GetTarget(Unit casterUnit)
        {
            if (_targetType == ETarget.Myself) return new List<Unit> { casterUnit };

            var findTargetAbility = casterUnit.GetAbility<FindTargetAbility>();

            if (_targetType == ETarget.AllTarget)
            {
                return findTargetAbility.FindAllTarget(_unitType);
            }
            else
            {
                int maxCount = _numberOfTarget;

                switch (_targetType)
                {
                    case ETarget.OneTargetInRange:
                        maxCount = 1;
                        break;
                    case ETarget.AllTargetInRange:
                        maxCount = int.MaxValue;
                        break;
                }

                switch (_rangeType)
                {
                    case ERangeType.Circle:
                        return findTargetAbility.FindTargetInCircle(_range, _unitType, maxCount);
                    case ERangeType.Straight:
                        return findTargetAbility.FindTargetInStraight(_directionType, _range, _assistantRange, _unitType, maxCount);
                    case ERangeType.Cone:
                        return findTargetAbility.FindTargetInCone(_directionType, _range, (int)_assistantRange, _unitType, maxCount);
                    case ERangeType.Grid:
                        return findTargetAbility.FindTargetInGrid(_tileRangeTemplate.range, _unitType, maxCount);
                    default:
                        return findTargetAbility.FindAllTarget(_unitType);
                }
            }
        }

        public Rect Draw(Rect rect)
        {
            var labelRect = new Rect(rect.x, rect.y, 140, rect.height);
            var valueRect = new Rect(rect.x + 140, rect.y, rect.width - 140, rect.height);

            GUI.Label(labelRect, "대상");
            _targetType = (ETarget)EditorGUI.EnumPopup(valueRect, _targetType);

            if (_targetType != ETarget.Myself)
            {
                labelRect.y += 20;
                valueRect.y += 20;
                GUI.Label(labelRect, "유닛 타입");
                _unitType = (EUnitType)EditorGUI.EnumPopup(valueRect, _unitType);
            }

            if (_targetType != ETarget.Myself && _targetType != ETarget.AllTarget)
            {
                labelRect.y += 20;
                valueRect.y += 20;
                GUI.Label(labelRect, "범위 타입");
                _rangeType = (ERangeType)EditorGUI.EnumPopup(valueRect, _rangeType);

                if (_rangeType == ERangeType.Circle)
                {
                    labelRect.y += 20;
                    valueRect.y += 20;
                    GUI.Label(labelRect, "범위");
                    _range = EditorGUI.FloatField(valueRect, _range);
                }
                else if (_rangeType == ERangeType.Straight)
                {
                    labelRect.y += 20;
                    valueRect.y += 20;
                    GUI.Label(labelRect, "방향");
                    _directionType = (EDirectionType)EditorGUI.EnumPopup(valueRect, _directionType);

                    labelRect.y += 20;
                    valueRect.y += 20;
                    GUI.Label(labelRect, "범위(세로)");
                    _range = EditorGUI.FloatField(valueRect, _range);

                    labelRect.y += 20;
                    valueRect.y += 20;
                    GUI.Label(labelRect, "너비(가로)");
                    _assistantRange = EditorGUI.FloatField(valueRect, _assistantRange);
                }
                else if (_rangeType == ERangeType.Cone)
                {
                    labelRect.y += 20;
                    valueRect.y += 20;
                    GUI.Label(labelRect, "방향");
                    _directionType = (EDirectionType)EditorGUI.EnumPopup(valueRect, _directionType);

                    labelRect.y += 20;
                    valueRect.y += 20;
                    GUI.Label(labelRect, "범위");
                    _range = EditorGUI.FloatField(valueRect, _range);

                    labelRect.y += 20;
                    valueRect.y += 20;
                    GUI.Label(labelRect, "각도");
                    _assistantRange = EditorGUI.IntField(valueRect, (int)_assistantRange);
                }
                else if (_rangeType == ERangeType.Grid)
                {
                    labelRect.y += 20;
                    valueRect.y += 20;
                    GUI.Label(labelRect, "범위");
                    _tileRangeTemplate = (TileRangeTemplate)EditorGUI.ObjectField(valueRect, _tileRangeTemplate, typeof(TileRangeTemplate), false);
                }
            }

            if (_targetType == ETarget.NumTargetInRange)
            {
                labelRect.y += 20;
                valueRect.y += 20;
                GUI.Label(labelRect, "감지할 유닛의 수");
                _numberOfTarget = EditorGUI.IntField(valueRect, _numberOfTarget);
            }

            rect.y = labelRect.y + 40;
            return rect;
        }

        public int GetNumRows(int rowNum)
        {
            rowNum += 3;

            if (_targetType != ETarget.Myself)
            {
                rowNum++;
            }

            if (_targetType != ETarget.Myself && _targetType != ETarget.AllTarget)
            {
                rowNum++;

                if (_rangeType == ERangeType.Circle)
                {
                    rowNum++;
                }
                else if (_rangeType == ERangeType.Straight)
                {
                    rowNum += 3;
                }
                else if (_rangeType == ERangeType.Cone)
                {
                    rowNum += 3;
                }
                else if (_rangeType == ERangeType.Grid)
                {
                    rowNum++;
                }
            }

            if (_targetType == ETarget.NumTargetInRange)
            {
                rowNum++;
            }

            return rowNum;
        }
    }
}
