using MiodenusAnimationConverter.Scene.Models.Meshes;

namespace MiodenusAnimationConverter.Animation
{
    public class ActionState
    {
        public int Time;
        public Transformation Transformation;
        public bool IsModelVisible;

        public ActionState(int time, Transformation transformation, bool isModelVisible = true)
        {
            Time = time;
            Transformation = transformation;
            IsModelVisible = isModelVisible;
        }
    }
}