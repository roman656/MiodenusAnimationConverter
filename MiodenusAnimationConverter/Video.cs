using System;
using System.Globalization;
using System.Runtime.CompilerServices;

namespace MiodenusAnimationConverter
{
    public class Video
    {
        public readonly string Type;
        public readonly uint Time;
        public readonly uint Bitrate;
        public readonly string Filename;
        public readonly byte Fps;

        public Video(string videoType, uint videoTime, uint videoBitrate, string nameOfVideoFile, byte videoFps)
        {
            if (videoFps > 0)
            {
                Fps = videoFps;
            }
            else
            {
                throw new ArgumentException("Invalid Fps value was used in constructor of video");
            }
            
            Type = videoType;
            Time = videoTime;
            Bitrate = videoBitrate;
            Filename = nameOfVideoFile + "." + Type;
        }

        public override string ToString()
        {
            return
                string.Format(CultureInfo.InvariantCulture, "Video: \n{{\n    Type: {0}\n    Time: {1}\n    Bitrate: {2}\n    Filename: {3}\n    FPS: {4}\n}}", Type, Time, Bitrate, Filename, Fps);
        }
    }
}