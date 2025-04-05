using FrameWork;
using UnityEditor;
using UnityEngine;

namespace Temporary.Core
{
    public abstract class ProjectilePointEffect : PointEffect
    {
        protected enum ENontargetProjectileRangeType
        {
            Straight,
            Cone,
        }

        [SerializeField] protected GameObject _prefab;
        [SerializeField] protected ESpawnPoint _spawnPoint;

        [SerializeField] protected ENontargetProjectileRangeType _rangeType;
        [SerializeField] protected EDirectionType _directionType;
        [SerializeField] protected float _range;
        [SerializeField] protected float _angleStep;
        [SerializeField] protected int _spawnCount;

        public override void Execute(Unit casterUnit, Vector3 targetVector)
        {
            if (casterUnit == null) return;

            Vector3 direction;
            if (targetVector == Vector3.zero)
            {
                FindTargetAbility.directionMap.TryGetValue(_directionType, out direction);
            }
            else
            {
                direction = (targetVector - casterUnit.transform.position).normalized;
            }

            switch (_rangeType)
            {
                case ENontargetProjectileRangeType.Straight:
                    SpawnStraightProjectiles(casterUnit, direction);
                    break;
                case ENontargetProjectileRangeType.Cone:
                    SpawnConeProjectiles(casterUnit, direction);
                    break;
            }
        }

        private void SpawnStraightProjectiles(Unit casterUnit, Vector3 direction)
        {
            Vector3 finalPosition = casterUnit.transform.position + direction * _range;

            SpawnProjectile(casterUnit, finalPosition);
        }

        private void SpawnConeProjectiles(Unit casterUnit, Vector3 direction)
        {
            var casterPos = casterUnit.transform.position;
            
            float maxAngle = (_spawnCount - 1) * 0.5f * _angleStep;

            for (float angle = -maxAngle; angle <= maxAngle; angle += _angleStep)
            {
                Quaternion rotation = Quaternion.AngleAxis(angle, Vector3.up);
                Vector3 finalDirection = rotation * direction;

                Vector3 finalPosition = casterPos + finalDirection * _range;

                SpawnProjectile(casterUnit, finalPosition);
            }
        }

        private void SpawnProjectile(Unit casterUnit, Vector3 finalPosition)
        {
            casterUnit.GetAbility<ProjectileAbility>().SpawnProjectile(_prefab, _spawnPoint, finalPosition, (caster, target) => { SkillImpact(caster, target); });
        }

        protected abstract void SkillImpact(Unit casterUnit, Unit targetUnit);

#if UNITY_EDITOR
        protected float lastRectY { get; private set; }

        public override void Draw(Rect rect)
        {
            var labelRect = new Rect(rect.x, rect.y, 140, rect.height);
            var valueRect = new Rect(rect.x + 140, rect.y, rect.width - 140, rect.height);

            GUI.Label(labelRect, "투사체");
            _prefab = (GameObject)EditorGUI.ObjectField(valueRect, _prefab, typeof(GameObject), false);

            labelRect.y += 20;
            valueRect.y += 20;
            GUI.Label(labelRect, "투사체 생성 위치");
            _spawnPoint = (ESpawnPoint)EditorGUI.EnumPopup(valueRect, _spawnPoint);

            labelRect.y += 40;
            valueRect.y += 40;
            GUI.Label(labelRect, "범위 타입");
            _rangeType = (ENontargetProjectileRangeType)EditorGUI.EnumPopup(valueRect, _rangeType);

            labelRect.y += 20;
            valueRect.y += 20;
            GUI.Label(labelRect, "방향");
            _directionType = (EDirectionType)EditorGUI.EnumPopup(valueRect, _directionType);

            if (_rangeType == ENontargetProjectileRangeType.Cone)
            {
                labelRect.y += 20;
                valueRect.y += 20;
                GUI.Label(labelRect, "투사체 사이 간격");
                _angleStep = EditorGUI.FloatField(valueRect, _angleStep);

                labelRect.y += 20;
                valueRect.y += 20;
                GUI.Label(labelRect, "투사체 개수");
                _spawnCount = EditorGUI.IntField(valueRect, _spawnCount);
            }

            labelRect.y += 20;
            valueRect.y += 20;
            GUI.Label(labelRect, "범위");
            _range = EditorGUI.FloatField(valueRect, _range);

            lastRectY = labelRect.y;
        }

        public override int GetNumRows()
        {
            int rowNum = 6;

            if (_rangeType == ENontargetProjectileRangeType.Cone)
            {
                rowNum += 2;
            }

            return rowNum;
        }
#endif
    }
}