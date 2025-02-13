using DG.Tweening;
using System;
using UnityEditor;
using UnityEngine;

namespace FrameWork.VisualNovel
{
    [Serializable]
    public class SCGMoveEpisode : ThemeEpisode
    {
        public override CommandType command => CommandType.SCG_Move;

        public Rect position;
        public float anchorX;
        public float anchorY;
        public float duration;
        public int loopCount = 1;
        public Ease ease;
        public bool isReturn;
        public Ease returnEase;

        public void Initialize(string theme, string cell)
        {
            base.Initialize(theme);

            var contents = cell.Split(' ');
            foreach (var content in contents)
            {
                var keyValue = content.Split(':');

                if (keyValue.Length >= 2)
                {
                    var key = keyValue[0];
                    var value = keyValue[1];

                    switch (key)
                    {
                        case "posX":
                            if (int.TryParse(value, out int x))
                            {
                                position.x = x;
                            }
                            break;
                        case "posY":
                            if (int.TryParse(value, out int y))
                            {
                                position.x = y;
                            }
                            break;
                        case "width":
                            if (int.TryParse(value, out int width))
                            {
                                position.width = width;
                            }
                            break;
                        case "height":
                            if (int.TryParse(value, out int height))
                            {
                                position.height = height;
                            }
                            break;
                        case "horizontal":
                            switch (value)
                            {
                                case "left":
                                    anchorX = 0;
                                    break;
                                case "center":
                                    anchorX = 0.5f;
                                    break;
                                case "right":
                                    anchorX = 1;
                                    break;
                            }
                            break;
                        case "vertical":
                            switch (value)
                            {
                                case "bottom":
                                    anchorY = 0;
                                    break;
                                case "middle":
                                    anchorY = 0.5f;
                                    break;
                                case "top":
                                    anchorY = 1;
                                    break;
                            }
                            break;
                    }
                }
            }
        }

#if UNITY_EDITOR
        public override void Draw(Rect rect)
        {
            base.Draw(rect);

            int middle = (int)(rect.width - 330) / 2;
            var labelRect = new Rect(rect.x + 330, rect.y + 4, rect.width - 330, 40);
            var valueRect = new Rect(rect.x + 390, rect.y + 4, middle - 62, 18);

            position = EditorGUI.RectField(labelRect, position);
            labelRect.y += 40;
            valueRect.y += 40;

            labelRect.width = 60;
            labelRect.height = 18;

            EditorGUI.LabelField(labelRect, "앵커 X");
            anchorX = EditorGUI.FloatField(valueRect, anchorX);

            labelRect.x += middle;
            valueRect.x += middle;
            valueRect.width = middle - 60;
            EditorGUI.LabelField(labelRect, "앵커 Y");
            anchorY = EditorGUI.FloatField(valueRect, anchorY);

            labelRect.y += 20;
            valueRect.y += 20;
            EditorGUI.LabelField(labelRect, "반복 횟수");
            loopCount = EditorGUI.IntField(valueRect, loopCount);

            labelRect.x -= middle;
            valueRect.x -= middle;
            valueRect.width = middle - 62;
            EditorGUI.LabelField(labelRect, "지속 시간");
            duration = EditorGUI.FloatField(valueRect, duration);

            labelRect.y += 20;
            valueRect.y += 20;
            valueRect.x += 60;
            labelRect.width = 120;
            valueRect.width = rect.width - 450;
            EditorGUI.LabelField(labelRect, "애니메이션 방식");
            ease = (Ease)EditorGUI.EnumPopup(valueRect, ease);

            labelRect.y += 20;
            valueRect.y += 20;
            EditorGUI.LabelField(labelRect, "본래 위치로 돌아갈지");
            isReturn = EditorGUI.Toggle(valueRect, isReturn);

            if (isReturn)
            {
                labelRect.y += 20;
                valueRect.y += 20;
                EditorGUI.LabelField(labelRect, "리턴 애니메이션 방식");
                returnEase = (Ease)EditorGUI.EnumPopup(valueRect, returnEase);
            }
        }

        public override int GetHeight()
        {
            if (isReturn) return 7;
            
            return 6;
        }
#endif
    }
}