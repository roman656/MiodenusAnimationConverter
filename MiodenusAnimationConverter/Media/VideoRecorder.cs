using FFMpegCore;
using FFMpegCore.Extend;
using FFMpegCore.Pipes;
using MiodenusAnimationConverter.Animation;

namespace MiodenusAnimationConverter.Media
{
    public class VideoRecorder
    {
        private readonly MainWindow _window;
        private readonly AnimationInfo _animationInfo;
        
        public VideoRecorder(in MainWindow window, in AnimationInfo animationInfo)
        {
            _window = window;
            _animationInfo = animationInfo;
        }

        public void CreateVideo(in RawVideoPipeSource videoFramesSource)
        {
            FFMpegArguments
                    .FromPipeInput(videoFramesSource)
                    .OutputToFile($"{Config.VideoDirectory}/{_animationInfo.VideoName}.{_animationInfo.VideoFormat}",
                            true, options => options
                                    .UsingMultithreading(true)
                                    .WithVideoCodec(_animationInfo.VideoCodec)
                                    .WithFramerate(_animationInfo.Fps)
                                    .WithVideoBitrate(_animationInfo.VideoBitrate)
                                    .ForceFormat(_animationInfo.VideoFormat))
                    .ProcessSynchronously();
        }

        public BitmapVideoFrameWrapper CreateVideoFrame() => new (new Screenshot(_window).Bitmap);
    }
}