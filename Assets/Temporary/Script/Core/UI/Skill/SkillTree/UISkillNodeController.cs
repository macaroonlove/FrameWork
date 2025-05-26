using FrameWork.UIBinding;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Temporary.Core
{
    public class UISkillNodeController : UIBase
    {
        #region ¹ÙÀÎµù
        enum Images
        {
            Icon,
        }
        enum Objects
        {
            Out,
            In,
        }
        #endregion

        internal RectTransform output;
        internal RectTransform input;

        internal List<UISkillNodeController> connections = new List<UISkillNodeController>();

        public void Initialize(SkillTemplate template)
        {
            BindImage(typeof(Images));
            BindObject(typeof(Objects));

            output = GetObject((int)Objects.Out).transform as RectTransform;
            input = GetObject((int)Objects.In).transform as RectTransform;

            Show(template);
        }

        public void Show(SkillTemplate template)
        {
            GetImage((int)Images.Icon).sprite = template.sprite;
        }
    }
}