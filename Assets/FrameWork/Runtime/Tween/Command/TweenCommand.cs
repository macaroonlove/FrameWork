using System;
using UnityEngine;
using PrimeTween;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace FrameWork.TweenExtension
{
    [Serializable]
    public abstract class TweenCommand
    {
        [HideInInspector] public bool isFolded = false;

        public virtual bool IsDelayCommand => false;

        /// <summary>
        /// 트윈 실행
        /// </summary>
        public abstract void Execute(ref Sequence sequence, bool isJoinMode, float timeScale = 1f, float startDelay = 0f);

        /// <summary>
        /// 시작 값 기록
        /// </summary>
        public virtual void RecordStartValues(bool isForce) { }

        /// <summary>
        /// 시작 값으로 복원
        /// </summary>
        public virtual void RestoreStartValues() { }

#if UNITY_EDITOR
        public static SerializedProperty currentDrawingProperty;

        /// <summary>
        /// 커맨드 제목
        /// </summary>
        public abstract string GetDescription();

        /// <summary>
        /// 에디터에서 차지할 높이 계산
        /// </summary>
        public virtual float GetHeight()
        {
            if (isFolded) return 0;
            return GetNumRows() * (EditorGUIUtility.singleLineHeight + 2);
        }

        /// <summary>
        /// 기본적으로 몇 줄을 차지할 것인가
        /// </summary>
        public virtual int GetNumRows()
        {
            return 1;
        }

        /// <summary>
        /// 인스펙터 내에서 자신을 그리는 메서드
        /// </summary>
        public abstract void Draw(Rect rect);

        protected void DrawSeparator(float x, float y, float width)
        {
            Rect lineRect = new Rect(x, y + 2, width, 1);
            Color lineColor = EditorGUIUtility.isProSkin ? new Color(0.35f, 0.35f, 0.35f) : new Color(0.65f, 0.65f, 0.65f);
            EditorGUI.DrawRect(lineRect, lineColor);
        }

        protected SerializedProperty GetCurrentProperty()
        {
            if (Selection.activeGameObject == null) return null;
            var executor = Selection.activeGameObject.GetComponent<TweenExecutor>();
            if (executor == null) return null;

            SerializedObject so = new SerializedObject(executor);
            SerializedProperty listProp = so.FindProperty("commands");
            if (listProp == null) return null;

            for (int i = 0; i < listProp.arraySize; i++)
            {
                var element = listProp.GetArrayElementAtIndex(i);
                if (element.managedReferenceValue == this)
                {
                    return element;
                }
            }
            return null;
        }
#endif
    }
}