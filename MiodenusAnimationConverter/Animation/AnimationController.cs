using System;
using System.Collections.Generic;
using MiodenusAnimationConverter.Scene.Models;
using NLog;
using OpenTK.Mathematics;

namespace MiodenusAnimationConverter.Animation
{
    public class AnimationController
    {
        private const float MillisecondsInSecond = 1000.0f;
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();
        private readonly Dictionary<ModelInfo, Model> _modelsInfo = new ();
        private readonly Animation _animation;
        private readonly float _framesPerMillisecond;
        private int _currentFrameIndex;
        public readonly int TotalFramesAmount;

        public bool IsAnimationFinished => _currentFrameIndex >= TotalFramesAmount;

        public AnimationController(in Animation animation, in Scene.Scene scene)
        {
            _animation = animation;
            _framesPerMillisecond = _animation.Info.Fps / MillisecondsInSecond;
            TotalFramesAmount = (int)(animation.Info.TimeLength * _framesPerMillisecond);
            
            foreach (var modelInfo in _animation.ModelsInfo)
            {
                try
                {
                    _modelsInfo[modelInfo] = scene.Models[modelInfo.Name];
                }
                catch (Exception exception)
                {
                    Logger.Warn($"There is no model with name: {modelInfo.Name}");
                }
            }
        }
        
        public int CurrentFrameIndex => _currentFrameIndex;

        private static void TransformModel(in Model model, in Transformation transformation)
        {
            if (transformation.ResetScale)
            {
                model.ResetScale();
            }
            
            if (transformation.Scale != Vector3.One)
            {
                model.Scale(transformation.Scale.X, transformation.Scale.Y, transformation.Scale.Z);
            }

            if (transformation.ResetLocalRotation)
            {
                model.Pivot.ResetLocalRotation();
            }
            
            model.Pivot.LocalRotate(transformation.LocalRotate.Angle, transformation.LocalRotate.Vector);

            if (transformation.ResetPosition)
            {
                model.Pivot.Position = Vector3.Zero;
            }
            
            model.Pivot.GlobalMove(transformation.GlobalMove.X, transformation.GlobalMove.Y,
                    transformation.GlobalMove.Z);
            
            model.Pivot.LocalMove(transformation.LocalMove.X, transformation.LocalMove.Y,
                    transformation.LocalMove.Z);
            
            model.Pivot.Rotate(transformation.Rotate.Angle, transformation.Rotate.RotationVectorStartPoint,
                    transformation.Rotate.RotationVectorEndPoint);
        }

        private static Transformation GetStepTransformation(in ActionState state, int stepsAmount)
        {
            var result = (Transformation)state.Transformation.Clone();

            if (result.GlobalMove != Vector3.Zero)
            {
                result.GlobalMove /= stepsAmount;
            }
            
            if (result.LocalMove != Vector3.Zero)
            {
                result.LocalMove /= stepsAmount;
            }
            
            if (result.Rotate.Angle != 0.0f)
            {
                result.Rotate.Angle /= stepsAmount;
            }

            if (result.LocalRotate.Angle != 0.0f)
            {
                result.LocalRotate.Angle /= stepsAmount;
            }

            return result;
        }

        public void PrepareSceneToNextFrame()
        {
            foreach (var (info, model) in _modelsInfo)
            {
                if (info.ActionBindings == null) { continue; }
                
                foreach (var actionBinding in info.ActionBindings)
                {
                    foreach (var action in _animation.Actions)
                    {
                        if (action.Name == actionBinding.ActionName)
                        {
                            var wasModelTransformed = false;

                            for (var i = 0; i < action.States.Count; i++)
                            {
                                /* TODO: отсортировать состояния по времени. */
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
    }
}