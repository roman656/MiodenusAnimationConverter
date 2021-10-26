using System.Collections.Generic;
using MiodenusAnimationConverter.Scene;
using NLog;

namespace MiodenusAnimationConverter.Animation
{
    public class AnimationController
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();
        private Animation _animation;
        private Scene.Scene _scene;
        private int _currentFrameIndex;

        public AnimationController(Animation animation, Scene.Scene scene)
        {
            _animation = animation;
            _scene = scene;
            Initialize();
        }

        private void Initialize()
        {
            var j = 0;
            
            for (var i = 0; i < _scene.ModelGroups.Count; i++)
            {
                while (_scene.ModelGroups[i].Models[0].Name != _animation.ModelsInfo[j].Name)
                {
                    j++;
                }
                
                _animation.ModelsInfo[j].BaseTransformation.Rotation.ToAxisAngle(out var axis, out var angle);
                _scene.ModelGroups[i].Models[0].Move(_animation.ModelsInfo[j].BaseTransformation.Location.X, _animation.ModelsInfo[j].BaseTransformation.Location.Y,
                    _animation.ModelsInfo[j].BaseTransformation.Location.Z);
                _scene.ModelGroups[i].Models[0].Rotate(angle, axis);
                _scene.ModelGroups[i].Models[0].Scale(_animation.ModelsInfo[j].BaseTransformation.Scale.X, _animation.ModelsInfo[j].BaseTransformation.Scale.Y,
                    _animation.ModelsInfo[j].BaseTransformation.Scale.Z);
            }
        }

        public void PrepareSceneToNextFrame()
        {
            _currentFrameIndex++;
        }

        public int CurrentFrameIndex => _currentFrameIndex;
    }
}