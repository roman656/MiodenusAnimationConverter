namespace MiodenusAnimationConverter.Animation.MAFStructure
{
    public class AnimationInfo
    {
        public string Type { get; set; } = string.Empty;
        public string Version { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string VideoFormat { get; set; } = string.Empty;
        public string VideoCodec { get; set; } = string.Empty;
        public int VideoBitrate { get; set; } = 0;
        public string VideoName { get; set; } = string.Empty;
        public int TimeLength { get; set; } = 0;
        public int Fps { get; set; } = 0;
        public int FrameWidth { get; set; } = 0;
        public int FrameHeight { get; set; } = 0;
        public float[] BackgroundColor { get; set; } = { -1.0f, -1.0f, -1.0f };
        public string[] Include { get; set; } = { string.Empty };
    }
}