using UnityEditor;
using UnityEngine;

namespace Temporary.Core
{
    public abstract class InstantPointEffect : PointEffect
    {
        [SerializeField] protected float _skillRange;
        [SerializeField] protected ENonTargetingActiveSkillType _skillType;
        [SerializeField] protected EUnitType _unitType;
        [SerializeField] protected float _skillWidth;
        [SerializeField] protected float _skillAngle;
        
        [SerializeField] protected FX _targetFX;

        public override void Execute(Unit casterUnit, Vector3 targetVector)
        {
            if (casterUnit == null) return;

            switch (_skillType)
            {
                case ENonTargetingActiveSkillType.Straight:
                    GetTargetStraight(casterUnit, targetVector);
                    break;
                case ENonTargetingActiveSkillType.Cone:
                    GetTargetCone(casterUnit, targetVector);
                    break;
            }
        }

        private void GetTargetStraight(Unit casterUnit, Vector3 targetVector)
        {
            var targets = casterUnit.GetAbility<FindTargetAbility>().FindAllTarget(_unitType);

            Vector3 forward = (targetVector - casterUnit.transform.position).normalized;
            Vector3 right = Vector3.Cross(Vector3.up, forward);
            float halfWidth = _skillWidth / 2f;

            foreach (var target in targets)
            {
                Vector3 directionToTarget = target.transform.position - casterUnit.transform.position;

                float forwardDistance = Vector3.Dot(forward, directionToTarget);
                float rightDistance = Vector3.Dot(right, directionToTarget);

                if (forwardDistance >= 0 && forwardDistance <= _skillRange && Mathf.Abs(rightDistance) <= halfWidth)
                {
                    SkillImpact(casterUnit, target);
                }

                ExecuteTargetFX(target);
            }
        }

        private void GetTargetCone(Unit casterUnit, Vector3 targetVector)
        {
            var targets = casterUnit.GetAbility<FindTargetAbility>().FindAttackableTarget(ETarget.AllTargetInRange, _skillRange, EAttackType.Far);

            Vector3 forward = (targetVector - casterUnit.transform.position).normalized;

            float cosThreshold = Mathf.Cos(Mathf.Deg2Rad * _skillAngle);

            foreach (var target in targets)
            {
                Vector3 directionToTarget = (target.transform.position - casterUnit.transform.position).normalized;

                if (Vector3.Dot(forward, directionToTarget) >= cosThreshold)
                {
                    SkillImpact(casterUnit, target);
                }

                ExecuteTargetFX(target);
            }
        }

        protected abstract void SkillImpact(Unit casterUnit, Unit targetUnit);

        #region FX
        private void ExecuteTargetFX(Unit target)
        {
            if (_targetFX != null)
            {
                _targetFX.Play(target);
            }
        }
        #endregion

#if UNITY_EDITOR
        protected float lastRectY { get; private set; }

        public override void Draw(Rect rect)
        {
            var labelRect = new Rect(rect.x, rect.y, 140, rect.height);
            var valueRect = new Rect(rect.x + 140, rect.y, rect.width - 140, rect.height);

            GUI.Label(labelRect, "대상자 FX");
            _targetFX = (FX)EditorGUI.ObjectField(valueRect, _targetFX, typeof(FX), false);

            labelRect.y += 40;
            valueRect.y += 40;
            GUI.Label(labelRect, "범위");
            _skillRange = EditorGUI.FloatField(valueRect, _skillRange);

            labelRect.y += 20;
            valueRect.y += 20;
            GUI.Label(labelRect, "스킬 방식");
            _skillType = (ENonTargetingActiveSkillType)EditorGUI.EnumPopup(valueRect, _skillType);

            if (_skillType == ENonTargetingActiveSkillType.Straight)
            {
                labelRect.y += 20;
                valueRect.y += 20;
                GUI.Label(labelRect, "유닛 타입");
                _unitType = (EUnitType)EditorGUI.EnumPopup(valueRect, _unitType);

                labelRect.y += 20;
                valueRect.y += 20;
                GUI.Label(labelRect, "스킬 너비");
                _skillWidth = EditorGUI.FloatField(valueRect, _skillWidth);
            }
            else
            {
                labelRect.y += 20;
                valueRect.y += 20;
                GUI.Label(labelRect, "콘 각도");
                _skillAngle = EditorGUI.FloatField(valueRect, _skillAngle);
            }

            lastRectY = labelRect.y;
        }

        public override int GetNumRows()
        {
            int rowNum = 4;

            if (_skillType == ENonTargetingActiveSkillType.Straight)
            {
                rowNum++;
            }

            return rowNum;
        }
#endif
    }
}
