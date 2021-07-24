using System;
using System.Collections.Generic;

namespace MiodenusAnimationConverter.AnimationFile
{
    public class ModelAnimationFile
    {
        public AnimationInfo AnimationInfo { get; set; } = new AnimationInfo();
        public List<ModelInfo> Models { get; set; } = new List<ModelInfo>();
        public List<Action> Actions { get; set; } = new List<Action>();
        public List<Bind> Bindings { get; set; } = new List<Bind>();
    }
}