using System;
using System.Collections.Generic;

namespace MiodenusAnimationConverter.AnimationFile
{
    public class Rotation
    {
        public float Angle { get; set; } = 0.0f;
        public float[] Vector { get; set; } = new float[3] { 0.0f, 0.0f, 0.0f };
    }
}
