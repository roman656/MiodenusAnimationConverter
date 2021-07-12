using System;
using System.Globalization;
using System.Drawing;
using System.IO; //может, работает
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
            CheckArguments(videoTime, videoBitrate, nameOfVideoFile, videoFps);
            Type = videoType;
            Time = videoTime;
            Bitrate = videoBitrate;
            Filename = $"{nameOfVideoFile}.{Type}";
            Fps = videoFps;
        }

        public void CreateVideo(int imagesAmount, string path) //не работает
        {
            System.Console.Out.WriteLine("Video creation started");

            using (var videoWriter = new VideoFileWriter())
            {
                var img = Bitmap.FromFile($"{path}screenshot_0.png");
                
                var width = img.Width;
                var height = img.Height;
                
                videoWriter.Open($"{path}{Filename}", width, height, Fps, VideoCodec.MPEG4, (int)Bitrate);

                for (int imageFrame = 0; imageFrame < imagesAmount; imageFrame++)
                {
                    var file = string.Format("{0}screenshot_{1}.png", path, imageFrame);
                    
                    if (System.IO.File.Exists(file))
                    {
                        using (Bitmap image = Bitmap.FromFile(file) as Bitmap)
                        {
                            videoWriter.WriteVideoFrame(image);
                        }
                    }
                    else
                    {
                        throw new FileNotFoundException($"File {file} was not found.");
                    }
                    
                }
                videoWriter.Close();
                
                System.Console.Out.WriteLine("Video creation finished");
            }
        }

        public override string ToString()
        {
            return string.Format(CultureInfo.InvariantCulture, 
                    "Video: \n{{\n    Type: {0}\n    Time: {1}\n    Bitrate: {2}\n    Filename: {3}\n    FPS: {4}\n}}", 
                    Type, Time, Bitrate, Filename, Fps);
        }

        private void CheckArguments(uint videoTimeArgument, uint videoBitrateArgument, string nameOfVideoFileArgument, byte videoFpsArgument)
        {
            if (videoFpsArgument <= 0)
            {
                throw new ArgumentException("Invalid Fps value was used in constructor of video");
            }

            if (videoBitrateArgument <= 0)
            {
                throw new ArgumentException("Invalid bitrate value was used in constructor of video");
            }
            
            if (videoTimeArgument <= 0)
            {
                throw new ArgumentException("Invalid time value was used in constructor of video");
            }
            
            if (nameOfVideoFileArgument == "")
            {
                throw new ArgumentException("Epty name of video file was used in constructor of video");
            }
        }
    }
}