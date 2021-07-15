using System.Collections.Generic;

namespace MiodenusAnimationConverter
{
    public class MainSettings
    {
        public List<string> AnimationFile = new List<string>();
        public string VideoFile = "";
        public string Extension = "avi";
        public int Bitrate = 10000;
        public int Fps = 60;

        public MainSettings(MainSettings mainSettings)
        {
            AnimationFile = mainSettings.AnimationFile;
            VideoFile = mainSettings.VideoFile;
            Extension = mainSettings.Extension;
            Bitrate = mainSettings.Bitrate;
            Fps = mainSettings.Fps;
        }

        public MainSettings(string videoFile, string extension, int bitrate, int fps, List<string> animationFile)
        {
            AnimationFile = animationFile;
            VideoFile = videoFile;
            Extension = extension;
            Bitrate = bitrate;
            Fps = fps;
        }
    }
}