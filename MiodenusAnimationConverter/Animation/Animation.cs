using System.Collections.Generic;

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
            
            foreach (var modelInfo in animation.ModelsInfo)
            {
                ModelsInfo.Add(new ModelInfo(modelInfo, null));
            }

            foreach (var action in animation.Actions)
            {
                Actions.Add(new Action(null, action.Name));
            }
        }
    }
}