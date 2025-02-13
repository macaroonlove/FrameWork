using System;
using UnityEditor;
using UnityEngine;

namespace FrameWork.VisualNovel
{
    [Serializable]
    public class SCGShowEpisode : ThemeEpisode
    {
        public override CommandType command => CommandType.SCG_Show;
        
        public Rect position;
        public float anchorX;
        public float anchorY;

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

            EditorGUI.LabelField(labelRect, "¾ÞÄ¿ X");
            anchorX = EditorGUI.FloatField(valueRect, anchorX);
            
            labelRect.x += middle;
            valueRect.x += middle;
            valueRect.width = middle - 60;
            EditorGUI.LabelField(labelRect, "¾ÞÄ¿ Y");
            anchorY = EditorGUI.FloatField(valueRect, anchorY);
        }

        public override int GetHeight()
        {
            return 3;
        }
#endif
    }
}