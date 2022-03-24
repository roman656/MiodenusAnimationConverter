using System;

namespace MiodenusAnimationConverter.Animation.MafStructure
{
    public static class DefaultMafParameters
    {
        public static class AnimationInfo
        {
            public const string Type = "maf";
            public const string Version = "1.0";
            public const string Name = "DefaultName";
            public const string VideoFormat = "mp4";
            public const string VideoCodec = "h264";
            public const int VideoBitrate = 4000;
            public const string VideoName = "ResultVideo";
            public const int TimeLength = 0;
            public const int Fps = 60;
            public const bool EnableMultisampling = true;
            public const int FrameWidth = 800;
            public const int FrameHeight = 800;
            public static readonly float[] BackgroundColor = { 0.3f, 0.3f, 0.4f };
            public static readonly string[] Include = Array.Empty<string>();
        }
        
        public static class ModelInfo
        {
            public const string Name = "";
            public const string Type = "stl";
            public const string Filename = "";
            public static readonly float[] Color = { 0.45f, 0.52f, 0.58f };
            public const bool UseCalculatedNormals = false;
        }
        
        public static class Action
        {
            public const string Name = "";
        }
        
        public static class ActionState
        {
            public const int Time = 0;
            public const bool IsModelVisible = true;
            public static readonly float[] Color = { -1.0f, -1.0f, -1.0f };
        }
        
        public static class Binding
        {
            public const string ModelName = "";
            public const string ActionName = "";
            public const int StartTime = 0;
            public const int TimeLength = 0;
            public const bool UseInterpolation = true;
        }
        
        public static class Transformation
        {
            public const bool ResetScale = false;
            public static readonly float[] Scale = { 1.0f, 1.0f, 1.0f };
            public const bool ResetLocalRotation = false;
            public const bool ResetPosition = false;
            public static readonly float[] GlobalMove = { 0.0f, 0.0f, 0.0f };
            public static readonly float[] LocalMove = { 0.0f, 0.0f, 0.0f };
        }
        
        public static class LocalRotation
        {
            public const float Angle = 0.0f;
            public const string Unit = "deg";
            public static readonly float[] Vector = { 0.0f, 0.0f, 0.0f };
        }
        
        public static class Rotation
        {
            public const float Angle = 0.0f;
            public const string Unit = "deg";
            public static readonly float[] RotationVectorStartPoint = { 0.0f, 0.0f, 0.0f };
            public static readonly float[] RotationVectorEndPoint = { 0.0f, 0.0f, 0.0f };
        }
    }
}