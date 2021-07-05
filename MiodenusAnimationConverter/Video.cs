using System.Runtime.CompilerServices;

namespace MiodenusAnimationConverter
{
    public struct Video
    {
        public readonly string Type;
        public readonly int Length;
        public readonly int Bitrate;
        public readonly string Filename;
        public readonly int FPS;

        public Video(string videoType, int videoLength, int videoBitrate, string nameOfVideoFile)
        {
            this.Type = videoType;
            this.Length = videoLength;
            this.Bitrate = videoBitrate;
            this.Filename = nameOfVideoFile + "." + Type;
            this.FPS = 60;
        }

        public void CreateVideo()
        {
            
        }

        public override string ToString()
        {
            return
                $"Type = {this.Type}, length = {this.Length}, bitrate = {this.Bitrate}, filename = {this.Filename}\n";
        }
    }
}