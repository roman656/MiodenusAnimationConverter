using System.Collections.Generic;

namespace MiodenusAnimationConverter
{
    public class Settings
    {
        public List<string> AnimationFile = new List<string>();
        public string VideoFile = "";
        public string Extension = "avi";
        public int Bitrate = 10000;
        public int Fps = 60;

        public Settings(Settings settings)
        {
            AnimationFile = settings.AnimationFile;
            VideoFile = settings.VideoFile;
            Extension = settings.Extension;
            Bitrate = settings.Bitrate;
            Fps = settings.Fps;
        }

        public Settings(string videoFile, string extension, int bitrate, int fps, List<string> animationFile)
        {
            AnimationFile = animationFile;
            VideoFile = videoFile;
            Extension = extension;
            Bitrate = bitrate;
            Fps = fps;
        }
    }
}