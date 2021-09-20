namespace MiodenusAnimationConverter.Animation
{
    public class ActionBinding
    {
        public string ActionName;
        public int StartTime;
        public int TimeLength;
        public bool UseInterpolation;

        public ActionBinding(string actionName = "UnnamedAction",
                             int startTime = 0,
                             int timeLength = -1,
                             bool useInterpolation = false)
        {
            ActionName = actionName;
            StartTime = startTime;
            TimeLength = timeLength;
            UseInterpolation = useInterpolation;
        }
    }
}