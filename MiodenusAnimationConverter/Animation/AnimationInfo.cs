using System.Globalization;

namespace MiodenusAnimationConverter.Animation
{
    public class AnimationInfo
    {
        public string Type { get; set; }
        public string Version { get; set; }
        public string Name { get; set; }
        public string VideoType { get; set; }
        public string VideoName { get; set; }
        public int TimeLength { get; set; }
        public int Fps { get; set; }
        public int FrameWidth { get; set; }
        public int FrameHeight { get; set; }

        public AnimationInfo(MAFStructure.AnimationInfo animationInfo)
        {
            Type = (animationInfo.Type == string.Empty) ? "maf" : animationInfo.Type;
            Version = (animationInfo.Version == string.Empty) ? "1.0" : animationInfo.Version;
            Name = (animationInfo.Name == string.Empty) ? "UnnamedAnimation" : animationInfo.Name;
            VideoType = (animationInfo.VideoType == string.Empty) ? "mp4" : animationInfo.VideoType;
            VideoName = (animationInfo.VideoName == string.Empty) ? "UnnamedVideo" : animationInfo.VideoName;
            TimeLength = (animationInfo.TimeLength == 0) ? -1 : animationInfo.TimeLength;
            Fps = (animationInfo.Fps <= 0) ? 60 : animationInfo.Fps;
            FrameWidth = (animationInfo.FrameWidth <= 0) ? 600 : animationInfo.FrameWidth;
            FrameHeight = (animationInfo.FrameHeight <= 0) ? 600 : animationInfo.FrameHeight;
        }

        public override string ToString()
        {
            return string.Format(CultureInfo.InvariantCulture,
                    $"Animation info:\n\tType: {Type}\n\tVersion: {Version}\n\tName: {Name}\n\tVideo type: {VideoType}\n\t"
                    + $"Video name: {VideoName}\n\tTime length: {TimeLength}\n\tFPS: {Fps}\n\tFrame width: {FrameWidth}\n\t"
                    + $"Frame height: {FrameHeight}\n");
        }
    }
}