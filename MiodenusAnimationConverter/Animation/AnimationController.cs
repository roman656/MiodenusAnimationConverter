using System.Numerics;
using Vector3 = OpenTK.Mathematics.Vector3;

namespace MiodenusAnimationConverter.Animation
{
    public class AnimationController
    {
        private Animation _animation;
        private Scene.Scene _scene;
        private int _currentFrameIndex;

        public AnimationController(Animation animation, Scene.Scene scene)
        {
            _animation = animation;
            _scene = scene;
            PrepareScene();
        }

        private void PrepareScene()
        {
            int i = 0;
            {
                _scene.ModelGroups[i].Move(_animation.ModelsInfo[0].BaseTransformation.Location.X,
                    _animation.ModelsInfo[0].BaseTransformation.Location.Y,
                    _animation.ModelsInfo[0].BaseTransformation.Location.Z);
                _animation.ModelsInfo[0].BaseTransformation.Rotation.ToAxisAngle(out Vector3 axis, out float angle);
                _scene.ModelGroups[i].Rotate(angle, axis);
                _scene.ModelGroups[i].Scale(_animation.ModelsInfo[0].BaseTransformation.Scale.X,
                    _animation.ModelsInfo[0].BaseTransformation.Scale.Y,
                    _animation.ModelsInfo[0].BaseTransformation.Scale.Z);
            }
            i++;
            {
                _scene.ModelGroups[i].Move(_animation.ModelsInfo[1].BaseTransformation.Location.X,
                    _animation.ModelsInfo[1].BaseTransformation.Location.Y,
                    _animation.ModelsInfo[1].BaseTransformation.Location.Z);
                _animation.ModelsInfo[1].BaseTransformation.Rotation.ToAxisAngle(out Vector3 axis, out float angle);
                _scene.ModelGroups[i].Rotate(angle, axis);
                _scene.ModelGroups[i].Scale(_animation.ModelsInfo[1].BaseTransformation.Scale.X,
                    _animation.ModelsInfo[1].BaseTransformation.Scale.Y,
                    _animation.ModelsInfo[1].BaseTransformation.Scale.Z);
            }
        }

        public void PrepareSceneToNextFrame()
        {
            _currentFrameIndex++;
        }

        public int CurrentFrameIndex => _currentFrameIndex;
    }
}