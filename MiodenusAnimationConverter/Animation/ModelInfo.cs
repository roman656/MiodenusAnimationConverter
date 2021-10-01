using System;
using System.Collections.Generic;
using MiodenusAnimationConverter.Scene.Models.Meshes;
using OpenTK.Mathematics;

namespace MiodenusAnimationConverter.Animation
{
    public class ModelInfo
    {
        public string Name;
        public string Type;
        public string Filename;
        public bool UseCalculatedNormals;
        public Color4 Color = GetRandomColor();
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
        
        private static Color4 GetRandomColor()
        {
            var random = new Random();
            return new Color4((float)random.NextDouble(), (float)random.NextDouble(), (float)random.NextDouble(), 1.0f);
        }
    }
}