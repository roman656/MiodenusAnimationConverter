using System.Collections.Generic;
using MiodenusAnimationConverter.Scene.Models;
using MiodenusAnimationConverter.Scene.Models.Meshes;
using NLog;
using OpenTK.Mathematics;

namespace MiodenusAnimationConverter.Animation
{
    public class AnimationController
    {
        private const float MillisecondsInSecond = 1000.0f;
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();
        private readonly Dictionary<Model, ModelInfo> _modelsInfo = new ();
        private readonly Animation _animation;
        private readonly Scene.Scene _scene;
        private readonly float _framesPerMillisecond;
        private int _currentFrameIndex;

        public AnimationController(Animation animation, Scene.Scene scene)
        {
            _animation = animation;
            Logger.Trace(_animation);
            _scene = scene;
            _framesPerMillisecond = _animation.Info.Fps / MillisecondsInSecond;
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

                _modelsInfo[_scene.ModelGroups[i].Models[0]] = _animation.ModelsInfo[j];
                TransformModel(_scene.ModelGroups[i].Models[0], _animation.ModelsInfo[j].BaseTransformation);
            }
        }

        private static void TransformModel(Model model, Transformation transformation)
        {
            transformation.Rotation.ToAxisAngle(out var axis, out var angle);
            model.Move(transformation.Location.X, transformation.Location.Y, transformation.Location.Z);
            model.Rotate(angle, axis);
            model.Scale(transformation.Scale.X, transformation.Scale.Y, transformation.Scale.Z);
        }

        private static void CheckWasTransformationParametersChanged(ActionState state,
                out bool wasLocationChanged, out bool wasRotationChanged, out bool wasScaleChanged)
        {
            wasLocationChanged = state.Transformation.Location != Vector3.Zero;
            wasRotationChanged = state.Transformation.Rotation != Quaternion.Identity;
            wasScaleChanged = state.Transformation.Scale != Vector3.One;
        }
        
        private static Transformation GetStepTransformation(ActionState state, int stepsAmount)
        {
            var result = new Transformation(Vector3.Zero, Quaternion.Identity, Vector3.One);

            CheckWasTransformationParametersChanged(state, out var wasLocationChanged, out var wasRotationChanged,
                    out var wasScaleChanged);
            state.Transformation.Rotation.ToAxisAngle(out var axis, out var angle);

            if (wasLocationChanged)
            {
                result.Location = state.Transformation.Location / stepsAmount;
            }
            
            if (wasRotationChanged)
            {
                result.Rotation = Quaternion.FromAxisAngle(axis, angle / stepsAmount);
            }
            
            if (wasScaleChanged)
            {
                result.Scale = state.Transformation.Scale / stepsAmount;
            }
            
            return result;
        }

        public void PrepareSceneToNextFrame()
        {
            foreach (var (model, info) in _modelsInfo)
            {
                foreach (var actionBinding in info.ActionBindings)
                {
                    foreach (var action in _animation.Actions)
                    {
                        if (action.Name == actionBinding.ActionName)
                        {
                            var wasModelTransformed = false;

                            for (var i = 0; i < action.States.Count; i++)
                            {
                                var prevState = (i != 0) ? action.States[i - 1] : action.States[i];
                                var nextState = action.States[i];
                                var nextStateFrameIndex = (int)(_framesPerMillisecond * nextState.Time);
                                var prevStateFrameIndex = (int)(_framesPerMillisecond * prevState.Time);

                                if (!actionBinding.UseInterpolation && nextStateFrameIndex == _currentFrameIndex)
                                {
                                    TransformModel(model, nextState.Transformation);
                                }
                                else if (actionBinding.UseInterpolation && nextStateFrameIndex > _currentFrameIndex 
                                        && !wasModelTransformed)
                                {   
                                        var stepsAmount = nextStateFrameIndex - prevStateFrameIndex;

                                        if (stepsAmount > 0)
                                        {
                                            TransformModel(model, GetStepTransformation(nextState, stepsAmount));
                                        }

                                        wasModelTransformed = true;
                                }
                            }
                        }
                    }
                }
            }
            
            _currentFrameIndex++;
        }

        public int CurrentFrameIndex => _currentFrameIndex;
    }
}