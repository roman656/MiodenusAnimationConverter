using System.Collections.Generic;
using MiodenusAnimationConverter.Scene.Models.Meshes;

namespace MiodenusAnimationConverter.Animation
{
    public class ModelInfo
    {
        public string Name;
        public string Type;
        public string Filename;
        public Transformation BaseTransformation;
        public List<ActionBinding> ActionBindings;

        public ModelInfo(List<ActionBinding> actionBindings,
                         Transformation baseTransformation,
                         string filename = "",
                         string name = "UnnamedModel",
                         string type = "stl")
        {
            ActionBindings = actionBindings;
            Name = name;
            Type = type;
            Filename = filename;
            BaseTransformation = baseTransformation;
        }
    }
}