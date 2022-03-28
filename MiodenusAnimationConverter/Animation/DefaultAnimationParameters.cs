using OpenTK.Mathematics;

namespace MiodenusAnimationConverter.Animation
{
    public static class DefaultAnimationParameters
    {
        public static class AnimationInfo
        {
            public const string Type = "maf";
            public const string Version = "1.0";
            public const string Name = "DefaultName";
            public const string VideoFormat = "mp4";
            public const string VideoCodec = "mpeg4";
            public const int VideoBitrate = 4000;
            public const string VideoName = "ResultVideo";
            public const int TimeLength = 0;
            public const int Fps = 60;
            public const int FrameWidth = 600;
            public const int FrameHeight = 600;
            public static readonly Color4 BackgroundColor = new (0.3f, 0.3f, 0.4f, 1.0f);
        }
        
        public static class ModelInfo
        {
            public const string Name = "UnnamedModel_";
            public const string Type = "stl";
            public const string Filename = "";
            public static readonly Color4 Color = new (0.45f, 0.52f, 0.58f, 1.0f);
        }
        
        public static class Action
        {
            public const string Name = "UnnamedAction_";
        }
        
        public static class ActionBinding
        {
            public const string ModelName = "UnnamedModel";
            public const string ActionName = "UnnamedAction";
            public const int StartTime = 0;
            public const int TimeLength = 0;
        }
        
        public static class ActionState
        {
            public const int Time = 0;
            public static readonly Color4 Color = new (0.45f, 0.52f, 0.58f, 1.0f);
        }

        public static class LocalRotation
        {
            public const string Unit = "deg";
        }

        public static class Rotation
        {
            public const string Unit = "deg";
        }
    }
}