using System.Collections.Generic;
using MiodenusAnimationConverter.Scene.Models.Meshes;

namespace MiodenusAnimationConverter.Animation
{
    public class Animation
    {
        public AnimationInfo Info;
        public List<ModelInfo> ModelsInfo;
        public List<Action> Actions;

        public Animation(MAFStructure.Animation animation)
        {
            Info = new AnimationInfo(animation.AnimationInfo.Type, animation.AnimationInfo.Version,
                    animation.AnimationInfo.Name, animation.AnimationInfo.VideoType, animation.AnimationInfo.VideoName,
                    animation.AnimationInfo.TimeLength, animation.AnimationInfo.Fps, animation.AnimationInfo.FrameWidth,
                    animation.AnimationInfo.FrameHeight);
            ModelsInfo = new List<ModelInfo>();
            foreach (var modelInfo in animation.ModelsInfo)
            {
                ModelsInfo.Add(new ModelInfo(null, new Transformation(),
                        modelInfo.Filename, modelInfo.Name, modelInfo.Type));
            }
            Actions = new List<Action>();
            foreach (var action in animation.Actions)
            {
                Actions.Add(new Action(null, action.Name));
            }
        }
    }
}