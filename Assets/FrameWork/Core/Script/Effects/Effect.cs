using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Temporary.Core
{
    /// <summary>
    /// 특정 게임 효과를 나타내는 추상 클래스입니다.
    /// </summary>
    public abstract class Effect : ScriptableObject
    {
        public abstract string GetDescription();

#if UNITY_EDITOR
        public abstract void Draw(Rect rect);

        public virtual float GetHeight()
        {
            var spacing = EditorGUIUtility.singleLineHeight * 3;
            return EditorGUIUtility.singleLineHeight * GetNumRows() + spacing;
        }

        public virtual int GetNumRows()
        {
            return 1;
        }
#endif
    }
}

#if UNITY_EDITOR
namespace Temporary.Editor
{
    using System;
    using UnityEditor;
    using UnityEditorInternal;

    public class EffectEditor : Editor
    {
        public static ReorderableList SetupReorderableList<T>(
            string headerText,
            List<T> elements,
            Action<Rect, T> drawElement,
            Action<T> selectElement,
            Action createElement,
            Action<T> removeElement)
        {
            var list = new ReorderableList(elements, typeof(T), true, true, true, true)
            {
                drawHeaderCallback = (rect) =>
                {
                    EditorGUI.LabelField(rect, headerText);
                },
                drawElementCallback = (rect, index, isActive, isFocus) =>
                {
                    var element = elements[index];
                    drawElement(rect, element);
                }
            };

            list.onSelectCallback = (l) =>
            {
                var selectedElement = elements[list.index];
                selectElement(selectedElement);
            };

            if (createElement != null)
            {
                list.onAddDropdownCallback = (buttonRect, l) =>
                {
                    createElement();
                };
            }

            list.onRemoveCallback = (l) =>
            {
                if (!EditorUtility.DisplayDialog("경고!", "이 항목을 삭제하시겠습니까?", "네", "아니요"))
                {
                    // 삭제하지 않았을 경우
                    return;
                }
                // 삭제했을 경우
                var element = elements[l.index];
                removeElement(element);
                ReorderableList.defaultBehaviours.DoRemoveButton(l);
            };

            return list;
        }
    }
}
#endif