using System;
using System.Globalization;
using OpenTK.Mathematics;

namespace MiodenusAnimationConverter.Animation
{
    public class AnimationInfo
    {
        public string Type { get; set; }
        public string Version { get; set; }
        public string Name { get; set; }
        public string VideoFormat { get; set; }
        public string VideoCodec { get; set; }
        public int VideoBitrate { get; set; }
        public string VideoName { get; set; }
        public int TimeLength { get; set; }
        public int Fps { get; set; }
        public int FrameWidth { get; set; }
        public int FrameHeight { get; set; }
        public Color4 BackgroundColor { get; set; }
        public string[] Include { get; set; }

        public AnimationInfo(MAFStructure.AnimationInfo animationInfo)
        {
            Type = (animationInfo.Type == string.Empty) ? "maf" : animationInfo.Type.ToLower();
            Version = (animationInfo.Version == string.Empty) ? "1.0" : animationInfo.Version;
            Name = (animationInfo.Name == string.Empty) ? "UnnamedAnimation" : animationInfo.Name;
            VideoFormat = (animationInfo.VideoFormat == string.Empty) ? "mp4" : animationInfo.VideoFormat.ToLower();
            VideoCodec = (animationInfo.VideoCodec == string.Empty) ? "h264" : animationInfo.VideoCodec;
            VideoBitrate = (animationInfo.VideoBitrate == 0) ? 4000 : animationInfo.VideoBitrate;
            VideoName = (animationInfo.VideoName == string.Empty) ? "UnnamedVideo" : animationInfo.VideoName;
            TimeLength = (animationInfo.TimeLength == 0) ? -1 : animationInfo.TimeLength;
            Fps = (animationInfo.Fps <= 0) ? 60 : animationInfo.Fps;
            FrameWidth = (animationInfo.FrameWidth <= 0) ? 600 : animationInfo.FrameWidth;
            FrameHeight = (animationInfo.FrameHeight <= 0) ? 600 : animationInfo.FrameHeight;
            BackgroundColor = ConvertColor(animationInfo.BackgroundColor);
        }
        
        private static Color4 ConvertColor(float[] color)
        {
            return CheckColor(color) ? new Color4(color[0], color[1], color[2], 1.0f) : new Color4(0.3f, 0.3f, 0.4f, 1.0f);
        }
        
        private static bool CheckColor(float[] color)
        {
            var result = true;

            for (var i = 0; i < 3; i++)
            {
                if (color[i] < 0.0f || color[i] > 1.0f)
                {
                    result = false;
                    break;
                }
            }

            return result;
        }

        public override string ToString()
        {
            return string.Format(CultureInfo.InvariantCulture,
                    $"Animation info:\n\tType: {Type}\n\tVersion: {Version}\n\tName: {Name}\n\tVideo type: {VideoFormat}\n\t"
                    + $"Video name: {VideoName}\n\tTime length: {TimeLength}\n\tFPS: {Fps}\n\tFrame width: {FrameWidth}\n\t"
                    + $"Frame height: {FrameHeight}\n");
        }
    }
}