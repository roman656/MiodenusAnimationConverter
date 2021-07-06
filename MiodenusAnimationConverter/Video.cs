using System;
using System.Globalization;
using System.Drawing; //может, работает
using System.Runtime.CompilerServices;
using AForge.Video.FFMPEG; //не работает

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
            if (videoFps <= 0)
            {
                throw new ArgumentException("Invalid Fps value was used in constructor of video");
            }
            
            Type = videoType;
            Time = videoTime;
            Bitrate = videoBitrate;
            Filename = $"{nameOfVideoFile}.{Type}";
            Fps = videoFps;
        }

        public void CreateVideo(int imagesAmount/*, string path*/) //не работает
        {
            System.Console.Out.WriteLine("Video creation started");
            
            string path = "C:\\Users\\PoorMercymain\\RiderProjects\\Miodenus1\\MiodenusAnimationConverter\\Photo\\";
            
            using (var videoWriter = new VideoFileWriter())
            {
                videoWriter.Open($"{path}video.avi", 600, 600, 15, VideoCodec.MPEG4, 10000);

                for (int imageFrame = 0; imageFrame < imagesAmount; imageFrame++)
                {
                    var imgPath = string.Format("{0}screenshot_{1}.png", path, imageFrame);
                    using (Bitmap image = Bitmap.FromFile(imgPath) as Bitmap)
                    {
                        videoWriter.WriteVideoFrame(image);
                    }
                }
                videoWriter.Close();
            }
        }

        public override string ToString()
        {
            return string.Format(CultureInfo.InvariantCulture, 
                    "Video: \n{{\n    Type: {0}\n    Time: {1}\n    Bitrate: {2}\n    Filename: {3}\n    FPS: {4}\n}}", 
                    Type, Time, Bitrate, Filename, Fps);
        }
    }
}