namespace MiodenusAnimationConverter.Animation.MafStructure
{
    public class Transformation
    {
        public bool ResetScale { get; set; } = DefaultMafParameters.Transformation.ResetScale;
        public float[] Scale { get; set; } = DefaultMafParameters.Transformation.Scale;
        public bool ResetLocalRotation { get; set; } = DefaultMafParameters.Transformation.ResetLocalRotation;
        public LocalRotation LocalRotate { get; set; } = new ();
        public bool ResetPosition { get; set; } = DefaultMafParameters.Transformation.ResetPosition;
        public float[] GlobalMove { get; set; } = DefaultMafParameters.Transformation.GlobalMove;
        public float[] LocalMove { get; set; } = DefaultMafParameters.Transformation.LocalMove;
        public Rotation Rotate { get; set; } = new ();
    }
}