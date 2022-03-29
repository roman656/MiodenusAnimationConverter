using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace MiodenusAnimationConverter.Animation
{
    public class Animation
    {
        public readonly AnimationInfo Info;
        public readonly List<ModelInfo> ModelsInfo;
        public readonly List<Action> Actions;

        public Animation(in MafStructure.Animation animation)
        {
            Info = new AnimationInfo(animation.AnimationInfo);
            ModelsInfo = new List<ModelInfo>();
            Actions = new List<Action>();
            var bindings = new Dictionary<string, List<ActionBinding>>();

            foreach (var action in animation.Actions)
            {
                Actions.Add(new Action(action));
            }

            foreach (var binding in animation.Bindings)
            {
                if (!bindings.ContainsKey(binding.ModelName))
                {
                    bindings[binding.ModelName] = new List<ActionBinding> { new (binding) };
                }
                else
                {
                    bindings[binding.ModelName].Add(new ActionBinding(binding));
                }
            }
            
            foreach (var modelInfo in animation.ModelsInfo)
            {
                ModelsInfo.Add(new ModelInfo(modelInfo, bindings.ContainsKey(modelInfo.Name)
                        ? bindings[modelInfo.Name]
                        : new List<ActionBinding>()));
            }

            CalculateTimeLength();
        }
        
        public Action GetActionByName(string name) => Actions.FirstOrDefault(action => action.Name == name);

        private void CalculateTimeLength()
        {
            var calculateTimeLength = Info.TimeLength == DefaultAnimationParameters.AnimationInfo.TimeLength;
            
            foreach (var modelInfo in ModelsInfo)
            {
                foreach (var binding in modelInfo.ActionBindings)
                {
                    if (binding.TimeLength == DefaultAnimationParameters.ActionBinding.TimeLength)
                    {
                        foreach (var action in Actions)
                        { 
                            if (action.Name == binding.ActionName)
                            {
                                binding.TimeLength = action.TimeLength;
                                break;
                            }
                        }
                    }

                    if (calculateTimeLength && binding.StartTime + binding.TimeLength > Info.TimeLength)
                    {
                        Info.TimeLength = binding.StartTime + binding.TimeLength;
                    }
                }
            }
        }
        
        public override string ToString()
        {
            var result = $"Animation:\n{Info}\nModels info:\n";

            result = ModelsInfo.Aggregate(result, (current, modelInfo) => current + modelInfo);
            result += "Actions:\n";
            result = Actions.Aggregate(result, (current, action) => current + action);

            return string.Format(CultureInfo.InvariantCulture, result);
        }
    }
}