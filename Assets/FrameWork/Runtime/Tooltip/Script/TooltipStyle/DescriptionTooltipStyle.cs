using TMPro;
using UnityEngine;

namespace FrameWork.Tooltip
{
    public class DescriptionTooltipStyle : TooltipStyle
    {
        #region ¹ÙÀÎµù
        enum Texts
        {
            Description
        }
        #endregion

        private TextMeshProUGUI _descriptionText;

        internal override TooltipData CreateField()
        {
            var data = new TooltipData();

            data.AddString("Description", "");

            return data;
        }

        internal override void ApplyData(TooltipData data)
        {
            _descriptionText.text = data.GetString("Description");
            _descriptionText.ForceMeshUpdate();

            var newHeight = _descriptionText.textInfo.lineCount * 50;
            Vector2 sizeDelta = _descriptionText.rectTransform.sizeDelta;
            sizeDelta.y = newHeight;
            _descriptionText.rectTransform.sizeDelta = sizeDelta;
        }

        protected override void Initialize()
        {
            base.Initialize();

            BindText(typeof(Texts));

            _descriptionText = GetText((int)Texts.Description);
        }
    }
}