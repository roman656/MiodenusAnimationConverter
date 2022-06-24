namespace MiodenusAnimationConverter.Animation.MafStructure;

public class AnimationInfo
{
    public string Type { get; set; } = DefaultMafParameters.AnimationInfo.Type;
    public string Version { get; set; } = DefaultMafParameters.AnimationInfo.Version;
    public string Name { get; set; } = DefaultMafParameters.AnimationInfo.Name;
    public string VideoFormat { get; set; } = DefaultMafParameters.AnimationInfo.VideoFormat;
    public string VideoCodec { get; set; } = DefaultMafParameters.AnimationInfo.VideoCodec;
    public int VideoBitrate { get; set; } = DefaultMafParameters.AnimationInfo.VideoBitrate;
    public string VideoName { get; set; } = DefaultMafParameters.AnimationInfo.VideoName;
    public int TimeLength { get; set; } = DefaultMafParameters.AnimationInfo.TimeLength;
    public int Fps { get; set; } = DefaultMafParameters.AnimationInfo.Fps;
    public bool EnableMultisampling { get; set; } = DefaultMafParameters.AnimationInfo.EnableMultisampling;
    public int FrameWidth { get; set; } = DefaultMafParameters.AnimationInfo.FrameWidth;
    public int FrameHeight { get; set; } = DefaultMafParameters.AnimationInfo.FrameHeight;
    public float[] BackgroundColor { get; set; } = DefaultMafParameters.AnimationInfo.BackgroundColor;
    public string[] Include { get; set; } = DefaultMafParameters.AnimationInfo.Include;
}