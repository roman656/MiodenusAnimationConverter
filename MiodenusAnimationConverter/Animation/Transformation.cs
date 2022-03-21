using System;
using System.Globalization;
using OpenTK.Mathematics;

namespace MiodenusAnimationConverter.Animation
{
    public class Transformation : ICloneable
    {
        public bool ResetScale { get; set; }
        public bool ResetLocalRotation { get; set; }
        public bool ResetPosition { get; set; }
        public Vector3 Scale { get; set; }
        public Vector3 GlobalMove { get; set; }
        public Vector3 LocalMove { get; set; }
        public Rotation Rotate { get; set; }
        public LocalRotation LocalRotate { get; set; }
        
        public Transformation(in MafStructure.Transformation transformation)
        {
            ResetScale = transformation.ResetScale;
            ResetLocalRotation = transformation.ResetLocalRotation;
            ResetPosition = transformation.ResetPosition;
            Scale = new Vector3(transformation.Scale[0], transformation.Scale[1], transformation.Scale[2]);    // Пропустит -1, но в модели не применится.
            GlobalMove = new Vector3(transformation.GlobalMove[0], transformation.GlobalMove[1],
                    transformation.GlobalMove[2]);
            LocalMove = new Vector3(transformation.LocalMove[0], transformation.LocalMove[1],
                    transformation.LocalMove[2]);
            Rotate = new Rotation(transformation.Rotate);
            LocalRotate = new LocalRotation(transformation.LocalRotate);
        }

        private Transformation(in Transformation transformation)
        {
            ResetScale = transformation.ResetScale;
            ResetLocalRotation = transformation.ResetLocalRotation;
            ResetPosition = transformation.ResetPosition;
            Scale = transformation.Scale;
            GlobalMove = transformation.GlobalMove;
            LocalMove = transformation.LocalMove;
            Rotate = (Rotation)transformation.Rotate.Clone();
            LocalRotate = (LocalRotation)transformation.LocalRotate.Clone();
        }
        
        public override string ToString()
        {
            return string.Format(CultureInfo.InvariantCulture, 
                    $"Transformation:\n\tReset scale: {ResetScale}\n\tReset local rotation: {ResetLocalRotation}\n\t"
                    + $"Reset position: {ResetPosition}\n\tScale: ({Scale.X}; {Scale.Y}; {Scale.Z})\n\t"
                    + $"Global move: ({GlobalMove.X}; {GlobalMove.Y}; {GlobalMove.Z})\n\t"
                    + $"Local move: ({LocalMove.X}; {LocalMove.Y}; {LocalMove.Z})\n\t{Rotate}\t{LocalRotate}");
        }

        public object Clone()
        {
            return new Transformation(this);
        }
    }
}