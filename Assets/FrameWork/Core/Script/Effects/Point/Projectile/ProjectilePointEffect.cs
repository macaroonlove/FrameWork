using UnityEditor;
using UnityEngine;

namespace Temporary.Core
{
    public abstract class ProjectilePointEffect : PointEffect
    {
        [SerializeField] protected GameObject _prefab;
        [SerializeField] protected ESpawnPoint _spawnPoint;

        [SerializeField] protected float _skillRange;
        [SerializeField] protected ENonTargetingActiveSkillType _skillType;
        [SerializeField] protected float _projectileAngleStep;
        [SerializeField] protected int _projectileCount;

        public override void Execute(Unit casterUnit, Vector3 targetVector)
        {
            if (casterUnit == null) return;

            switch (_skillType)
            {
                case ENonTargetingActiveSkillType.Straight:
                    SpawnStraightProjectiles(casterUnit, targetVector);
                    break;
                case ENonTargetingActiveSkillType.Cone:
                    SpawnConeProjectiles(casterUnit, targetVector);
                    break;
            }
        }

        private void SpawnStraightProjectiles(Unit casterUnit, Vector3 targetVector)
        {
            Vector3 direction = (targetVector - casterUnit.transform.position).normalized;

            Vector3 finalPosition = casterUnit.transform.position + direction * _skillRange;

            SpawnProjectile(casterUnit, finalPosition);
        }

        private void SpawnConeProjectiles(Unit casterUnit, Vector3 targetVector)
        {
            Vector3 casterPosition = casterUnit.transform.position;
            Vector3 direction = (targetVector - casterPosition).normalized;

            float maxAngle = (_projectileCount - 1) / 2f * _projectileAngleStep;

            for (float angle = -maxAngle; angle <= maxAngle; angle += _projectileAngleStep)
            {
                Quaternion rotation = Quaternion.AngleAxis(angle, Vector3.up);
                Vector3 finalDirection = rotation * direction;

                Vector3 finalPosition = casterPosition + finalDirection * _skillRange;

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
            GUI.Label(labelRect, "범위");
            _skillRange = EditorGUI.FloatField(valueRect, _skillRange);

            labelRect.y += 20;
            valueRect.y += 20;
            GUI.Label(labelRect, "스킬 방식");
            _skillType = (ENonTargetingActiveSkillType)EditorGUI.EnumPopup(valueRect, _skillType);

            if (_skillType == ENonTargetingActiveSkillType.Cone)
            {
                labelRect.y += 20;
                valueRect.y += 20;
                GUI.Label(labelRect, "투사체 사이 간격");
                _projectileAngleStep = EditorGUI.FloatField(valueRect, _projectileAngleStep);

                labelRect.y += 20;
                valueRect.y += 20;
                GUI.Label(labelRect, "투사체 개수");
                _projectileCount = EditorGUI.IntField(valueRect, _projectileCount);
            }

            lastRectY = labelRect.y;
        }

        public override int GetNumRows()
        {
            int rowNum = 5;

            if (_skillType == ENonTargetingActiveSkillType.Cone)
            {
                rowNum += 2;
            }

            return rowNum;
        }
#endif
    }
}
