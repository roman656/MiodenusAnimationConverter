using System;
using System.Collections.Generic;

namespace MiodenusAnimationConverter.AnimationFile
{
    public class Transform
    {
        public float[] Translation { get; set; } = new float[3] { 0.0f, 0.0f, 0.0f };
        public List<Rotation> Rotations { get; set; } = new List<Rotation>();
        public float Scale { get; set; } = 1.0f;

    }
}