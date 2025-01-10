using TMPro;

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

            data.Add("BaseStat", "");
            data.Add("DetailStat", "");

            return data;
        }

        internal override void ApplyData(TooltipData data)
        {
            _baseStatText.text = data.GetString("BaseStat");
            _detailStatText.text = data.GetString("DetailStat");
        }

        protected override void Initialize()
        {
            BindText(typeof(Texts));

            _baseStatText = GetText((int)Texts.BaseStat);
            _detailStatText = GetText((int)Texts.DetailStat);
        }
    }
}