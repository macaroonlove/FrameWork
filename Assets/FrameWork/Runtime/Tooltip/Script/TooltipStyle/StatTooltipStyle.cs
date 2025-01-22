using TMPro;
using UnityEngine;

namespace FrameWork.Tooltip
{
    public class StatTooltipStyle : TooltipStyle
    {
        #region ¹ÙÀÎµù
        enum Texts
        {
            BaseStat,
            DetailStat,
        }
        #endregion

        private TextMeshProUGUI _baseStatText;
        private TextMeshProUGUI _detailStatText;

        internal override TooltipData CreateField()
        {
            var data = new TooltipData();

            data.AddString("BaseStat", "");
            data.AddString("DetailStat", "");

            return data;
        }

        internal override void ApplyData(TooltipData data)
        {
            _baseStatText.text = data.GetString("BaseStat");
            _detailStatText.text = data.GetString("DetailStat");
            _detailStatText.ForceMeshUpdate();
            
            var newHeight = _detailStatText.textInfo.lineCount * 50;
            Vector2 sizeDelta = _detailStatText.rectTransform.sizeDelta;
            sizeDelta.y = newHeight;
            _detailStatText.rectTransform.sizeDelta = sizeDelta;
        }

        protected override void Initialize()
        {
            base.Initialize();

            BindText(typeof(Texts));

            _baseStatText = GetText((int)Texts.BaseStat);
            _detailStatText = GetText((int)Texts.DetailStat);
        }
    }
}