namespace MiodenusAnimationConverter.Animation.MAFStructure
{
    public class ActionState
    {
        public int Time { get; set; } = -1;
        public bool IsModelVisible { get; set; } = true;
        public float[] Color { get; set; } = { -1.0f, -1.0f, -1.0f };
        public Transformation Transformation { get; set; } = new ();
    }
}