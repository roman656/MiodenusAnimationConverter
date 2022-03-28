using System.Globalization;
using System.Linq;
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
        public bool EnableMultisampling { get; set; }
        public int FrameWidth { get; set; }
        public int FrameHeight { get; set; }
        public Color4 BackgroundColor { get; set; }
        public string[] Include { get; set; }

        public AnimationInfo(in MafStructure.AnimationInfo animationInfo)
        {
            Type = string.IsNullOrEmpty(animationInfo.Type.Trim())
                    ? DefaultAnimationParameters.AnimationInfo.Type
                    : animationInfo.Type.Trim().ToLower();
            Version = string.IsNullOrEmpty(animationInfo.Version.Trim())
                    ? DefaultAnimationParameters.AnimationInfo.Version
                    : animationInfo.Version.Trim().ToLower();
            Name = string.IsNullOrEmpty(animationInfo.Name.Trim())
                    ? DefaultAnimationParameters.AnimationInfo.Name
                    : animationInfo.Name.Trim();
            VideoFormat = string.IsNullOrEmpty(animationInfo.VideoFormat.Trim())
                    ? DefaultAnimationParameters.AnimationInfo.VideoFormat
                    : animationInfo.VideoFormat.Trim().ToLower();
            VideoCodec = string.IsNullOrEmpty(animationInfo.VideoCodec.Trim())
                    ? DefaultAnimationParameters.AnimationInfo.VideoCodec
                    : animationInfo.VideoCodec.Trim().ToLower();
            VideoBitrate = animationInfo.VideoBitrate <= 0
                    ? DefaultAnimationParameters.AnimationInfo.VideoBitrate
                    : animationInfo.VideoBitrate;
            VideoName = string.IsNullOrEmpty(animationInfo.VideoName.Trim())
                    ? DefaultAnimationParameters.AnimationInfo.VideoName
                    : animationInfo.VideoName.Trim();
            TimeLength = animationInfo.TimeLength < 0
                    ? DefaultAnimationParameters.AnimationInfo.TimeLength
                    : animationInfo.TimeLength;
            Fps = animationInfo.Fps <= 0
                    ? DefaultAnimationParameters.AnimationInfo.Fps
                    : animationInfo.Fps;
            EnableMultisampling = animationInfo.EnableMultisampling;
            FrameWidth = animationInfo.FrameWidth <= 0
                    ? DefaultAnimationParameters.AnimationInfo.FrameWidth
                    : animationInfo.FrameWidth;
            FrameHeight = animationInfo.FrameHeight <= 0
                    ? DefaultAnimationParameters.AnimationInfo.FrameHeight
                    : animationInfo.FrameHeight;
            BackgroundColor = CheckColor(animationInfo.BackgroundColor)
                    ? new Color4(animationInfo.BackgroundColor[0],
                                 animationInfo.BackgroundColor[1],
                                 animationInfo.BackgroundColor[2],
                                 1.0f)
                    : DefaultAnimationParameters.AnimationInfo.BackgroundColor;
            Include = animationInfo.Include;
        }

        private static bool CheckColor(in float[] color)
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
            var result = string.Format(CultureInfo.InvariantCulture, 
                    $"Animation info:\n\tType: {Type}\n\tVersion: {Version}\n\tName: {Name}\n\t"
                    + $"Video format: {VideoFormat}\n\tVideo codec: {VideoCodec}\n\tVideo bitrate: {VideoBitrate}"
                    + $"\n\tVideo name: {VideoName}\n\tTime length: {TimeLength}\n\tFPS: {Fps}\n\tIs multisampling"
                    + $" enabled: {EnableMultisampling}\n\tFrame width: {FrameWidth}\n\tFrame height: {FrameHeight}"
                    + $"\n\tBackground color: ({BackgroundColor.R}; {BackgroundColor.G}; {BackgroundColor.B};"
                    + $" {BackgroundColor.A})\n\tIncludes:");

            result = Include.Aggregate(result, (current, path) => current + $"\n\t{path}");

            return result + "\n";
        }
    }
}