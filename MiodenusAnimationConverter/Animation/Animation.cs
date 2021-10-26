using System.Collections.Generic;
using System.Globalization;

namespace MiodenusAnimationConverter.Animation
{
    public class Animation
    {
        public AnimationInfo Info;
        public List<ModelInfo> ModelsInfo;
        public List<Action> Actions;

        public Animation(MAFStructure.Animation animation)
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
                ModelsInfo.Add(new ModelInfo(modelInfo, bindings[modelInfo.Name]));
            }
        }
        
        public override string ToString()
        {
            var result = $"Animation:\n{Info}\nModels info:\n";
            
            foreach (var modelInfo in ModelsInfo)
            {
                result += modelInfo;
            }

            result += "\nActions:\n";
            
            foreach (var action in Actions)
            {
                result += action;
            }
            
            return string.Format(CultureInfo.InvariantCulture, result);
        }
    }
}