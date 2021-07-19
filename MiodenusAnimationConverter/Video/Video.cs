using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using FFMpegCore;
using FFMpegCore.Extend;
using FFMpegCore.Pipes;

namespace MiodenusAnimationConverter.Video
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

        public void CreateVideo(int imagesAmount, string path)
        {
            Console.WriteLine("Video creation started");

            string filename = Filename;
            
            var videoFramesSource = new RawVideoPipeSource(GetBitmaps())
            {
                FrameRate = Fps
            };

            FFMpegArguments
                .FromPipeInput(videoFramesSource)
                .OutputToFile(filename, true, 
                    options => options
                        .WithVideoCodec("h264")
                        .ForceFormat(Type))
                .ProcessSynchronously();
                
                System.Console.Out.WriteLine("Video creation finished");
        }
        
        private static ImageInfo[] GetFiles()
        {
            var di = new System.IO.DirectoryInfo("screenshots");
            var files = di.GetFiles();

            ImageInfo[] ret = new ImageInfo[files.Length];

            for (int i = 0; i < files.Length; i++)
            {
                ImageInfo iinfo = new ImageInfo(files[i].FullName);
                ret[i] = iinfo;
            }

            return ret;
        }

        public static IEnumerable<IVideoFrame> GetBitmaps()
        {
            var di = new System.IO.DirectoryInfo("screenshots");
            var files = di.GetFiles();

            for (int i = 0; i < files.Length; i++)
            {
                using (var frame = CreateVideoFrame(files[i].FullName))
                {
                    yield return frame;
                }
            }
        }

        public static BitmapVideoFrameWrapper CreateVideoFrame(string filePath)
        {
            var bitmap = new Bitmap(filePath);
            return new BitmapVideoFrameWrapper(bitmap);
        }

        public override string ToString()
        {
            return string.Format(CultureInfo.InvariantCulture, 
                    "Video: \n{{\n    Type: {0}\n    Time: {1}\n    Bitrate: {2}\n    Filename: {3}\n    FPS: {4}\n}}", 
                    Type, Time, Bitrate, Filename, Fps);
        }

        private static void CheckArguments(uint videoTimeArgument, uint videoBitrateArgument, string nameOfVideoFileArgument, byte videoFpsArgument)
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
                throw new ArgumentException("Empty name of video file was used in constructor of video");
            }
        }
    }
}