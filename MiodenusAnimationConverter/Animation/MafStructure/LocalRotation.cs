namespace MiodenusAnimationConverter.Animation.MafStructure;

public class LocalRotation
{
    public float Angle { get; set; } = DefaultMafParameters.LocalRotation.Angle;
    public string Unit { get; set; } = DefaultMafParameters.LocalRotation.Unit;
    public float[] Vector { get; set; } = DefaultMafParameters.LocalRotation.Vector;
}