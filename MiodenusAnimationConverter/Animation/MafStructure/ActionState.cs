namespace MiodenusAnimationConverter.Animation.MafStructure
{
    public class ActionState
    {
        public int Time { get; set; } = DefaultMafParameters.ActionState.Time;
        public bool IsModelVisible { get; set; } = DefaultMafParameters.ActionState.IsModelVisible;
        public float[] Color { get; set; } = DefaultMafParameters.ActionState.Color;
        public Transformation Transformation { get; set; } = new ();
    }
}