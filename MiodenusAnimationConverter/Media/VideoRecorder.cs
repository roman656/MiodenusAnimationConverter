using FFMpegCore;
using FFMpegCore.Extend;
using FFMpegCore.Pipes;

namespace MiodenusAnimationConverter.Media
{
    public class VideoRecorder
    {
        public readonly string Type;
        public readonly string Filename;
        public readonly int Fps;
        private readonly MainWindow _window;
        
        public VideoRecorder(in MainWindow window, in string videoFilename, in string videoType, int videoFps)
        {
            _window = window;
            Type = videoType;
            Filename = videoFilename;
            Fps = videoFps;
        }

        public void CreateVideo(RawVideoPipeSource videoFramesSource)
        { 
            FFMpegArguments
                    .FromPipeInput(videoFramesSource)
                    .OutputToFile(
                            $"{Filename}.{Type}",
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

        public BitmapVideoFrameWrapper CreateVideoFrame() => new (new Screenshot(_window).Bitmap);
        
        /*
        public override string ToString()
        {
            return string.Format(CultureInfo.InvariantCulture, 
                    "Video: \n{{\n    Type: {0}\n    Time: {1}\n    Bitrate: {2}\n    Filename: {3}\n    FPS: {4}\n}}", 
                    Type, Time, Bitrate, Filename, Fps);
        }*/
    }
}