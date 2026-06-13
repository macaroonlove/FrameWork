using PrimeTween;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace FrameWork.TweenExtension
{
    [System.Serializable]
    public class ParticleEmissionTweenCommand : TweenCommand
    {
        public ParticleSystem[] targets;

        [Tooltip("체크하면 Rate over Time 값을 덮어쓰거나 트윈합니다.")]
        public bool controlRateOverTime = true;
        public float endRateOverTime = 10f;

        [Tooltip("체크하면 Rate over Distance 값을 덮어쓰거나 트윈합니다.")]
        public bool controlRateOverDistance = false;
        public float endRateOverDistance = 0f;

        [Tooltip("체크하면 아래 지정한 Bursts 배열로 파티클의 Bursts를 완전히 교체합니다.")]
        public bool controlBursts = false;
        public ParticleSystem.Burst[] bursts;

        public float duration = 1f;
        public Ease ease = Ease.Linear;

        [HideInInspector] public float[] _startRateOverTime;
        [HideInInspector] public float[] _startRateOverDistance;

        public override void Execute(ref Sequence sequence, bool isJoinMode, float timeScale = 1f, float startDelay = 0f)
        {
            if (targets == null || targets.Length == 0) return;

            float finalDuration = duration / timeScale;
            Sequence subSeq = Sequence.Create(); // 여러 타겟의 복합 제어를 위해 서브 시퀀스 사용

            for (int i = 0; i < targets.Length; i++)
            {
                if (targets[i] == null) continue;

                var target = targets[i];
                var em = target.emission;
                float startROT = em.rateOverTime.constant;
                float startROD = em.rateOverDistance.constant;

                // Rate 제어 (Duration이 0보다 크면 부드럽게 트윈, 아니면 즉시 적용)
                if (finalDuration > 0f)
                {
                    Tween t = Tween.Custom(target, 0f, 1f, finalDuration, delegate (ParticleSystem ps, float val)
                    {
                        var m = ps.emission;
                        if (controlRateOverTime) m.rateOverTime = Mathf.Lerp(startROT, endRateOverTime, val);
                        if (controlRateOverDistance) m.rateOverDistance = Mathf.Lerp(startROD, endRateOverDistance, val);
                    }, ease, startDelay: startDelay);
                    subSeq.Group(t);
                }
                else
                {
                    Tween t = Tween.Delay(startDelay, delegate ()
                    {
                        var m = target.emission;
                        if (controlRateOverTime) m.rateOverTime = endRateOverTime;
                        if (controlRateOverDistance) m.rateOverDistance = endRateOverDistance;
                    });
                    subSeq.Group(t);
                }

                // 2. Bursts 제어 (Bursts는 트윈의 개념이 없으므로 시작 시점에 즉시 덮어씌움)
                if (controlBursts && bursts != null)
                {
                    Tween burstTween = Tween.Delay(startDelay, delegate ()
                    {
                        var m = target.emission;
                        m.SetBursts(bursts);
                    });
                    subSeq.Group(burstTween);
                }
            }

            if (isJoinMode) sequence.Group(subSeq);
            else sequence.Chain(subSeq);
        }

        public override void RecordStartValues(bool isForce)
        {
            if (targets == null) return;

            if (_startRateOverTime == null || isForce)
            {
                _startRateOverTime = new float[targets.Length];
                _startRateOverDistance = new float[targets.Length];
                for (int i = 0; i < targets.Length; i++)
                {
                    if (targets[i] != null)
                    {
                        _startRateOverTime[i] = targets[i].emission.rateOverTime.constant;
                        _startRateOverDistance[i] = targets[i].emission.rateOverDistance.constant;
                    }
                }
                return;
            }

            // 증분 베이킹 (기존 데이터 유지)
            if (_startRateOverTime.Length != targets.Length)
            {
                float[] newROT = new float[targets.Length];
                float[] newROD = new float[targets.Length];
                for (int i = 0; i < targets.Length; i++)
                {
                    if (i < _startRateOverTime.Length)
                    {
                        newROT[i] = _startRateOverTime[i];
                        newROD[i] = _startRateOverDistance[i];
                    }
                    else if (targets[i] != null)
                    {
                        newROT[i] = targets[i].emission.rateOverTime.constant;
                        newROD[i] = targets[i].emission.rateOverDistance.constant;
                    }
                }
                _startRateOverTime = newROT;
                _startRateOverDistance = newROD;
            }
        }

        public override void RestoreStartValues()
        {
            if (targets == null || _startRateOverTime == null) return;
            for (int i = 0; i < targets.Length; i++)
            {
                if (targets[i] != null && i < _startRateOverTime.Length)
                {
                    var em = targets[i].emission;
                    em.rateOverTime = _startRateOverTime[i];
                    em.rateOverDistance = _startRateOverDistance[i];
                }
            }
        }

#if UNITY_EDITOR
        public override string GetDescription() => "파티클 이미션 제어 (Emission)";

        public override void Draw(Rect rect)
        {
            float h = EditorGUIUtility.singleLineHeight;
            float currentY = rect.y;

            GUIStyle headerStyle = new GUIStyle(EditorStyles.miniBoldLabel);
            headerStyle.normal.textColor = EditorGUIUtility.isProSkin ? new Color(0.8f, 0.8f, 0.8f) : new Color(0.2f, 0.2f, 0.2f);
            SerializedProperty prop = currentDrawingProperty;

            // 1. 타겟 설정
            EditorGUI.LabelField(new Rect(rect.x, currentY, rect.width, h), "타겟 (ParticleSystem) 설정", headerStyle);
            currentY += h + 2;

            if (prop != null)
            {
                SerializedProperty targetsProp = prop.FindPropertyRelative("targets");
                if (targetsProp != null)
                {
                    float height = EditorGUI.GetPropertyHeight(targetsProp, true);
                    EditorGUI.PropertyField(new Rect(rect.x, currentY, rect.width, height), targetsProp, true);
                    currentY += height + 5;
                }
            }

            DrawSeparator(rect.x, currentY, rect.width);
            currentY += 6;

            // 2. 이미션 제어 설정
            EditorGUI.LabelField(new Rect(rect.x, currentY, rect.width, h), "이미션 (Emission) 값 제어", headerStyle);
            currentY += h + 2;

            float toggleWidth = rect.width * 0.4f;
            float fieldWidth = rect.width * 0.6f;

            // Rate over Time
            controlRateOverTime = EditorGUI.ToggleLeft(new Rect(rect.x, currentY, toggleWidth, h), " Rate over Time", controlRateOverTime);
            if (controlRateOverTime)
            {
                endRateOverTime = EditorGUI.FloatField(new Rect(rect.x + toggleWidth, currentY, fieldWidth, h), endRateOverTime);
            }
            currentY += h + 2;

            // Rate over Distance
            controlRateOverDistance = EditorGUI.ToggleLeft(new Rect(rect.x, currentY, toggleWidth, h), " Rate over Distance", controlRateOverDistance);
            if (controlRateOverDistance)
            {
                endRateOverDistance = EditorGUI.FloatField(new Rect(rect.x + toggleWidth, currentY, fieldWidth, h), endRateOverDistance);
            }
            currentY += h + 2;

            DrawSeparator(rect.x, currentY, rect.width);
            currentY += 6;

            // 3. Bursts 배열 제어
            controlBursts = EditorGUI.ToggleLeft(new Rect(rect.x, currentY, rect.width, h), " Bursts 덮어쓰기 (체크 시 하단 배열 적용)", controlBursts);
            currentY += h + 2;

            if (controlBursts && prop != null)
            {
                SerializedProperty burstsProp = prop.FindPropertyRelative("bursts");
                if (burstsProp != null)
                {
                    float height = EditorGUI.GetPropertyHeight(burstsProp, true);
                    EditorGUI.PropertyField(new Rect(rect.x, currentY, rect.width, height), burstsProp, true);
                    currentY += height + 5;
                }
            }

            DrawSeparator(rect.x, currentY, rect.width);
            currentY += 6;

            // 4. 시간 설정
            EditorGUI.LabelField(new Rect(rect.x, currentY, rect.width, h), "재생 설정 (Duration > 0 이면 부드럽게 수치 변화)", headerStyle);
            currentY += h + 2;

            duration = EditorGUI.FloatField(new Rect(rect.x, currentY, rect.width, h), "지속 시간 (Duration)", duration);
            currentY += h + 2;

            if (duration > 0f)
            {
                ease = (Ease)EditorGUI.EnumPopup(new Rect(rect.x, currentY, rect.width, h), "재생 방식 (Ease)", ease);
            }
        }

        public override int GetNumRows()
        {
            if (isFolded) return 0;
            int rows = 2; // 기본 패딩
            SerializedProperty prop = currentDrawingProperty;

            if (prop != null)
            {
                SerializedProperty targetsProp = prop.FindPropertyRelative("targets");
                if (targetsProp != null) rows += Mathf.CeilToInt(EditorGUI.GetPropertyHeight(targetsProp, true) / (EditorGUIUtility.singleLineHeight + 2));
            }
            else rows += 1;

            rows += 1; // 이미션 타이틀
            rows += 1; // Rate over Time 라인
            rows += 1; // Rate over Distance 라인

            rows += 1; // Bursts 토글
            if (controlBursts && prop != null)
            {
                SerializedProperty burstsProp = prop.FindPropertyRelative("bursts");
                if (burstsProp != null) rows += Mathf.CeilToInt(EditorGUI.GetPropertyHeight(burstsProp, true) / (EditorGUIUtility.singleLineHeight + 2));
            }

            rows += 1; // 재생 설정 타이틀
            rows += 1; // duration
            if (duration > 0f) rows += 1; // ease

            return rows;
        }
#endif
    }
}