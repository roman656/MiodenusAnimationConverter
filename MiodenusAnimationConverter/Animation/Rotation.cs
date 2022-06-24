namespace MiodenusAnimationConverter.Animation;

using System;
using System.Globalization;
using OpenTK.Mathematics;

public class Rotation : ICloneable
{
    public float Angle { get; set; }
    public Vector3 RotationVectorStartPoint { get; set; }
    public Vector3 RotationVectorEndPoint { get; set; }

    public Rotation(in MafStructure.Rotation rotation)
    {
        /* TODO: если вектора равны. */
        RotationVectorStartPoint = new Vector3(rotation.RotationVectorStartPoint[0],
                rotation.RotationVectorStartPoint[1], rotation.RotationVectorStartPoint[2]);
        RotationVectorEndPoint = new Vector3(rotation.RotationVectorEndPoint[0],
                rotation.RotationVectorEndPoint[1], rotation.RotationVectorEndPoint[2]);
        var unit = string.IsNullOrEmpty(rotation.Unit.Trim())
                ? DefaultAnimationParameters.Rotation.Unit
                : rotation.Unit.Trim().ToLower();
        Angle = unit == DefaultAnimationParameters.Rotation.Unit
                ? MathHelper.DegreesToRadians(rotation.Angle)
                : rotation.Angle;
    }

    private Rotation(in Rotation rotation)
    {
        RotationVectorStartPoint = rotation.RotationVectorStartPoint;
        RotationVectorEndPoint = rotation.RotationVectorEndPoint;
        Angle = rotation.Angle;
    }
        
    public override string ToString()
    {
        return string.Format(CultureInfo.InvariantCulture, 
                $"Rotation:\n\tAngle: {Angle}\n\tRotation vector start point: ({RotationVectorStartPoint.X};"
                + $" {RotationVectorStartPoint.Y}; {RotationVectorStartPoint.Z})\n\tRotation vector end point: "
                + $"({RotationVectorEndPoint.X}; {RotationVectorEndPoint.Y}; {RotationVectorEndPoint.Z})\n");
    }

    public object Clone() => new Rotation(this);
}