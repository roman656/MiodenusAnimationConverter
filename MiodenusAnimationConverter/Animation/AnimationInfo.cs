namespace MiodenusAnimationConverter.Animation
{
    public class AnimationInfo
    {
        public string Type;
        public string Version;
        public string Name;
        public string VideoType;
        public string VideoName;
        public int TimeLength;
        public int Fps;

        public AnimationInfo(string type = "maf",
                             string version = "1.0",
                             string name = "UnnamedAnimation",
                             string videoType = "mp4",
                             string videoName = "UnnamedVideo",
                             int timeLength = -1,
                             int fps = 60)
        {
            Type = type;
            Version = version;
            Name = name;
            VideoType = videoType;
            VideoName = videoName;
            TimeLength = timeLength;
            Fps = fps;
        }
    }
}