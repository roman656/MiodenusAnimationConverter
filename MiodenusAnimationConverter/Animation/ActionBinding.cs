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

        public ActionBinding(in MafStructure.Binding binding)
        {
            ModelName = string.IsNullOrEmpty(binding.ModelName.Trim())
                    ? DefaultAnimationParameters.ActionBinding.ModelName
                    : binding.ModelName.Trim();
            ActionName = string.IsNullOrEmpty(binding.ActionName.Trim())
                    ? DefaultAnimationParameters.ActionBinding.ActionName
                    : binding.ActionName.Trim();
            StartTime = binding.StartTime < 0
                    ? DefaultAnimationParameters.ActionBinding.StartTime 
                    : binding.StartTime;
            TimeLength = binding.TimeLength < 0
                    ? DefaultAnimationParameters.ActionBinding.TimeLength 
                    : binding.TimeLength;
            UseInterpolation = binding.UseInterpolation;
        }
        
        public override string ToString()
        {
            return string.Format(CultureInfo.InvariantCulture,
                    $"Action binding:\n\tAction name: {ActionName}\n\tModel name: {ModelName}\n\t"
                    + $"Start time: {StartTime}\n\tTime length: {TimeLength}\n\t"
                    + $"Use interpolation: {UseInterpolation}\n");
        }
    }
}