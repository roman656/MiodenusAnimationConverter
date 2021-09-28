namespace MiodenusAnimationConverter.Animation.MAFStructure
{
    public class Transformation
    {
        public float[] Location { get; set; } = { 0.0f, 0.0f, 0.0f };
        public Rotation Rotation { get; set; } = new ();
        public float[] Scale { get; set; } = { 1.0f, 1.0f, 1.0f };
    }
}