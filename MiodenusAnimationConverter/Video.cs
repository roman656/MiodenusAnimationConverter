using System.Runtime.CompilerServices;

namespace MiodenusAnimationConverter
{
    public struct Video
    {
        public readonly string type;
        public readonly int length;
        public readonly int bitrate;
        public readonly string filename;
        public static Screenshot[] screenshots;
        public readonly int amountOfScreenshots;
        public readonly int fps;

        public Video(string videoType, int videoLength, int videoBitrate, string nameOfVideoFile)
        {
            this.type = videoType;
            this.length = videoLength;
            this.bitrate = videoBitrate;
            this.filename = nameOfVideoFile + "." + type;
            this.amountOfScreenshots = 240;
            this.fps = 60;
            this.initializeArray();
        }

        public void createVideo()
        {
            
        }

        public string toString()
        {
            return
                $"Type = {this.type}, length = {this.length}, bitrate = {this.bitrate}, filename = {this.filename}\n";
        }

        private void initializeArray()
        {
            screenshots = new Screenshot[amountOfScreenshots];
            
            for (int i = 0; i < length; ++i)
            {
                screenshots[i] = new Screenshot();
            }
        }
        
        private void initializeArray(Screenshot[] fillingScreenshots)
        {
            
            if (fillingScreenshots.GetLength(0) == this.amountOfScreenshots)
            {
                screenshots = new Screenshot[amountOfScreenshots];
                for (int i = 0; i < this.amountOfScreenshots; ++i)
                {
                    screenshots[i] = new Screenshot();
                    screenshots[i] = fillingScreenshots[i];
                }
            }
            else
            {
                System.Console.Out.WriteLine("Wrong length of array in function initializeArray in Video struct.");
            }
            
        }
    }
}