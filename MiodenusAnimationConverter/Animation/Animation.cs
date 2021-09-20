using System.Collections.Generic;
using MiodenusAnimationConverter.Scene.Models.Meshes;

namespace MiodenusAnimationConverter.Animation
{
    public class Animation
    {
        public AnimationInfo Info;
        public List<ModelInfo> ModelsInfo;
        public List<Action> Actions;

        public Animation()
        {
            var temp = new Transformation();
            temp.Reset();
            
            Info = new AnimationInfo();
            ModelsInfo = new List<ModelInfo> { new (new List<ActionBinding> {}, temp) };
            Actions = new List<Action> { new (new [] { new ActionState(0, temp) } ) };
        }
    }
}