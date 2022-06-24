namespace MiodenusAnimationConverter.Animation.MafStructure;

public class Rotation
{
    public float Angle { get; set; } = DefaultMafParameters.Rotation.Angle;
    public string Unit { get; set; } = DefaultMafParameters.Rotation.Unit;
    public float[] RotationVectorStartPoint { get; set; } = DefaultMafParameters.Rotation.RotationVectorStartPoint;
    public float[] RotationVectorEndPoint { get; set; } = DefaultMafParameters.Rotation.RotationVectorEndPoint;
}