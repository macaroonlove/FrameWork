using FrameWork.UIBinding;

namespace FrameWork.Tooltip
{
    public abstract class TooltipStyle : UIBase
    {
        internal abstract TooltipData CreateField();
        internal abstract void ApplyData(TooltipData data);
    }
}