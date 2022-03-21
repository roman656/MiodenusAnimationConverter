namespace MiodenusAnimationConverter.Animation.MafStructure
{
    public class Binding
    {
        public string ModelName { get; set; } = DefaultMafParameters.Binding.ModelName;
        public string ActionName { get; set; } = DefaultMafParameters.Binding.ActionName;
        public int StartTime { get; set; } = DefaultMafParameters.Binding.StartTime;
        public int TimeLength { get; set; } = DefaultMafParameters.Binding.TimeLength;
        public bool UseInterpolation { get; set; } = DefaultMafParameters.Binding.UseInterpolation;
    }
}