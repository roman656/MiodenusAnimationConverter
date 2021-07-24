using System;
using System.Collections.Generic;

namespace MiodenusAnimationConverter.AnimationFile
{
    public class ModelInfo
    {
        public int ID { get; set; } = 0;
        public string Name { get; set; } = "Model";
        public string FileName { get; set; } = string.Empty;
        public string MD5 { get; set; } = string.Empty;
        public Transform BaseTransform { get; set; } = new Transform();
    }
}