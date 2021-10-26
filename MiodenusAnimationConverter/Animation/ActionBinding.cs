using System.Globalization;

namespace MiodenusAnimationConverter.Animation
{
    public class ActionBinding
    {
        public string ModelName { get; set; }
        public string ActionName { get; set; }
        public int StartTime { get; set; }
        public int TimeLength { get; set; }
        public bool UseInterpolation { get; set; }

        public ActionBinding(MAFStructure.Binding binding)
        {
            ModelName = (binding.ModelName == string.Empty) ? "UnnamedModel" : binding.ModelName;
            ActionName = (binding.ActionName == string.Empty) ? "UnnamedAction" : binding.ActionName;
            StartTime = (binding.StartTime < 0) ? 0 : binding.StartTime;
            TimeLength = (binding.TimeLength == 0) ? -1 : binding.TimeLength;
            UseInterpolation = binding.UseInterpolation;
        }
        
        public override string ToString()
        {
            return string.Format(CultureInfo.InvariantCulture,
                    $"Action binding:\n\tModel name: {ModelName}\n\tAction name: {ActionName}\n\tStart time: {StartTime}\n\t"
                    + $"Time length: {TimeLength}\n\tUse interpolation: {UseInterpolation}\n");
        }
    }
}