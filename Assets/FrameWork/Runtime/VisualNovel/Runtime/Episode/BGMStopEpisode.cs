using System;

namespace FrameWork.VisualNovel
{
    [Serializable]
    public class BGMStopEpisode : ThemeEpisode
    {
        public override CommandType command => CommandType.BGM_Stop;
    }
}