using System;
using FFMpegCore;
using FFMpegCore.Extend;
using FFMpegCore.Pipes;

namespace MiodenusAnimationConverter.Media
{
    public class VideoRecorder
    {
        public readonly string Type;
        public readonly string Filename;
        public readonly byte Fps;
        
        public VideoRecorder(in string videoFilename, in string videoType, byte videoFps)
        {
            CheckArguments(videoFilename, videoType, videoFps);
            
            Type = videoType;
            Filename = videoFilename;
            Fps = videoFps;
        }

        public void CreateVideo(RawVideoPipeSource videoFramesSource)
        { 
            FFMpegArguments
                    .FromPipeInput(videoFramesSource)
                    .OutputToFile(
                            Filename,
                            true,
                            options => options
                                    .WithVideoCodec("h264")
                                    .ForceFormat(Type))
                    .ProcessSynchronously();
        }
/*
        public static IEnumerable<IVideoFrame> GetBitmaps( IEnumerable<IVideoFrame> frames)
        {
           frames.add(CreateVideoFrame());
                {
                    yield return frame;
                }
        }*/

        public static BitmapVideoFrameWrapper CreateVideoFrame() => new (new Screenshot(600, 600).Bitmap);
        
        /*
        public override string ToString()
        {
            return string.Format(CultureInfo.InvariantCulture, 
                    "Video: \n{{\n    Type: {0}\n    Time: {1}\n    Bitrate: {2}\n    Filename: {3}\n    FPS: {4}\n}}", 
                    Type, Time, Bitrate, Filename, Fps);
        }*/

        private static void CheckArguments(in string videoFilename, in string videoType, byte videoFps)
        {
            if (videoFps <= 0)
            {
                throw new ArgumentException("Video FPS must be greater than 0.");
            }

            if (videoFilename.Equals(""))
            {
                throw new ArgumentException("Video filename can not be empty.");
            }
            
            if (videoType.Equals(""))
            {
                throw new ArgumentException("Video type can not be empty.");
            }
        }
    }
}