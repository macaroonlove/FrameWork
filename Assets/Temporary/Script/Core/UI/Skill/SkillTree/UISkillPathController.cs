using FrameWork.UIBinding;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Temporary.Core
{
    public class UISkillPathController : UIBase
    {
        enum Images
        {
            Out,
            Center,
            In,
        }

        internal void Initialize(RectTransform output, RectTransform input, int heightOffset)
        {
            BindImage(typeof(Images));

            Show(output, input, heightOffset);
        }

        internal void Show(RectTransform output, RectTransform input, int heightOffset)
        {
            Vector2 finalOutput = output.anchoredPosition + (output.parent as RectTransform).anchoredPosition;
            Vector2 finalInput = input.anchoredPosition + (input.parent as RectTransform).anchoredPosition;

            var outRect = GetImage((int)Images.Out).rectTransform;
            var centerRect = GetImage((int)Images.Center).rectTransform;
            var inRect = GetImage((int)Images.In).rectTransform;

            float outputHeight = heightOffset * 0.25f;
            float inputHeight = Mathf.Abs(finalOutput.y - finalInput.y) - outputHeight;
            float width = Mathf.Abs(finalOutput.x - finalInput.x) + 4;

            Vector2 finalCenter;
            // 왼쪽으로 이어지는 선
            if (finalOutput.x > finalInput.x)
            {
                finalCenter = new Vector2(finalOutput.x - width + 4, finalOutput.y - outputHeight);
            }
            else
            {
                finalCenter = new Vector2(finalOutput.x, finalOutput.y - outputHeight);
            }

            outRect.sizeDelta = new Vector2(4, outputHeight);
            centerRect.sizeDelta = new Vector2(width, 4);
            inRect.sizeDelta = new Vector2(4, inputHeight);

            outRect.anchoredPosition = finalOutput;
            centerRect.anchoredPosition = finalCenter;
            inRect.anchoredPosition = finalInput;
        }
    }
}
