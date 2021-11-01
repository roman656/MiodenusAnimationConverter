using System;
using System.Globalization;
using MiodenusAnimationConverter.Scene.Models.Meshes;
using OpenTK.Mathematics;

namespace MiodenusAnimationConverter.Animation
{
    public class ActionState
    {
        public int Time { get; set; }
        public bool IsModelVisible { get; set; }
        public Color4 Color { get; set; }
        public Transformation Transformation { get; set; }

        public ActionState(MAFStructure.ActionState actionState)
        {
            Time = (actionState.Time < 0) ? 0 : actionState.Time;
            IsModelVisible = actionState.IsModelVisible;
            Color = ConvertColor(actionState.Color);    // TODO: Если цвет не будет задан явно будет рандомный. Надо исправить.
            Transformation = ConvertTransformation(actionState.Transformation);
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
            return CheckScale(scale) ? new Vector3(scale[0], scale[1], scale[2]) : Vector3.One;
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
        
        private static Color4 ConvertColor(float[] color)
        {
            return CheckColor(color) ? new Color4(color[0], color[1], color[2], 1.0f) : GetRandomColor();
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
                    $"Action state:\n\tTime: {Time}\n\tIs model visible: {IsModelVisible}\n\tColor: ({Color.R};"
                    + $" {Color.G}; {Color.B}; {Color.A})\n\t{Transformation}\n");
        }
    }
}