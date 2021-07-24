using System;
using System.Collections.Generic;

namespace MiodenusAnimationConverter.AnimationFile
{
    public class AnimationInfo
    {
        public string Type {get;set;} = "MAF";
        public float Version {get;set;} = 1.0f;
        public string AnimationName {get;set;} = "Default animation";
        public string VideoName {get;set;} = "default.mp4";
        public float TimeLength {get;set;} = 0;
        public int FPS {get;set;} = 60;
    }
}