namespace MiodenusAnimationConverter.Animation.MAFStructure
{
    public class Binding
    {
        public string ModelName { get; set; } = string.Empty;
        public string ActionName { get; set; } = string.Empty;
        public int StartTime { get; set; } = -1;
        public int TimeLength { get; set; } = 0;
        public bool UseInterpolation { get; set; } = true;
    }
}