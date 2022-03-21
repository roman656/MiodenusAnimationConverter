using System.Globalization;
using OpenTK.Mathematics;

namespace MiodenusAnimationConverter.Animation
{
    public class LocalRotation
    {
        public float Angle { get; set; }
        public Vector3 Vector { get; set; }

        public LocalRotation(in MafStructure.LocalRotation localRotation)
        {
            Vector = new Vector3(localRotation.Vector[0], localRotation.Vector[1], localRotation.Vector[2]);    // TODO: что делать если вектор 0, 0, 0.
            var unit = string.IsNullOrEmpty(localRotation.Unit.Trim())
                    ? DefaultAnimationParameters.LocalRotation.Unit
                    : localRotation.Unit.Trim().ToLower();
            Angle = unit == DefaultAnimationParameters.LocalRotation.Unit
                    ? MathHelper.DegreesToRadians(localRotation.Angle)
                    : localRotation.Angle;
        }
        
        public override string ToString()
        {
            return string.Format(CultureInfo.InvariantCulture, 
                    $"Local rotation:\n\tAngle: {Angle}\n\tVector: ({Vector.X}; {Vector.Y}; {Vector.Z})\n");
        }
    }
}