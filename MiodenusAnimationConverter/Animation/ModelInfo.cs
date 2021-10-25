using System;
using System.Collections.Generic;
using System.Globalization;
using FFMpegCore.Enums;
using MiodenusAnimationConverter.Scene.Models.Meshes;
using OpenTK.Mathematics;

namespace MiodenusAnimationConverter.Animation
{
    public class ModelInfo
    {
        public string Name { get; set; }
        public string Type { get; set; }
        public string Filename { get; set; }
        public bool UseCalculatedNormals { get; set; }
        public Color4 Color { get; set; }
        public Transformation BaseTransformation { get; set; }
        public List<ActionBinding> ActionBindings { get; set; }

        public ModelInfo(MAFStructure.ModelInfo modelInfo, List<ActionBinding> actionBindings)
        {
            Name = (modelInfo.Name == string.Empty) ? "UnnamedModel" : modelInfo.Name;
            Type = (modelInfo.Type == string.Empty) ? "stl" : modelInfo.Type;
            Filename = modelInfo.Filename;
            UseCalculatedNormals = modelInfo.UseCalculatedNormals;
            Color = ConvertColor(modelInfo.Color);
            BaseTransformation = ConvertTransformation(modelInfo.BaseTransformation);
            ActionBindings = actionBindings;
        }

        private static Transformation ConvertTransformation(MAFStructure.Transformation transformation)
        {
            var location = new Vector3(transformation.Location[0],
                                       transformation.Location[1], 
                                       transformation.Location[2]);
            var rotation = ConvertRotation(transformation.Rotation);
            var scale = ConvertScale(transformation.Scale);
            
            return new Transformation(location, rotation, scale);
        }
        
        private static Quaternion ConvertRotation(MAFStructure.Rotation rotation)
        {
            var axis = new Vector3(rotation.Vector[0], rotation.Vector[1], rotation.Vector[2]);
            var angle = (rotation.Unit == "deg") ? MathHelper.DegreesToRadians(rotation.Angle) : rotation.Angle;

            return Quaternion.FromAxisAngle(axis, angle);
        }
        
        private static Vector3 ConvertScale(float[] scale)
        {
            return CheckScale(scale) ? new Vector3(scale[0], scale[1], scale[2]) : new Vector3(1.0f);
        }

        private static Color4 ConvertColor(float[] color)
        {
            return CheckColor(color) ? new Color4(color[0], color[1], color[2], 1.0f) : GetRandomColor();
        }
        
        private static bool CheckScale(float[] scale)
        {
            var result = true;

            for (var i = 0; i < 3; i++)
            {
                if (scale[i] <= 0.0f)
                {
                    result = false;
                    break;
                }
            }

            return result;
        }

        private static bool CheckColor(float[] color)
        {
            var result = true;

            for (var i = 0; i < 3; i++)
            {
                if (color[i] < 0.0f || color[i] > 1.0f)
                {
                    result = false;
                    break;
                }
            }

            return result;
        }
        
        private static Color4 GetRandomColor()
        {
            var random = new Random();
            return new Color4((float)random.NextDouble(), (float)random.NextDouble(), (float)random.NextDouble(), 1.0f);
        }

        public override string ToString()
        {
            return string.Format(CultureInfo.InvariantCulture,
                $"Model info:\n\tName: {Name}\n\tType: {Type}\n\tFilename: {Filename}\n\t"
                + $"Use calculated normals: {UseCalculatedNormals}\n\tColor: ({Color.R}; {Color.G}; {Color.B};"
                + $" {Color.A})\n\tBase transformation -> {BaseTransformation}\n\tAction bindings: {ActionBindings}\n");
        }
    }
}