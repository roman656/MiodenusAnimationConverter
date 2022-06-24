namespace MiodenusAnimationConverter.Animation;

using System.Collections.Generic;
using System.Linq;
using Scene.Models;
using NLog;
using OpenTK.Mathematics;

public class AnimationController
{
    private const float MillisecondsInSecond = 1000.0f;
    private static readonly Logger Logger = LogManager.GetCurrentClassLogger();
    private readonly Dictionary<ModelInfo, Model> _modelsInfo = new ();
    private readonly Animation _animation;
    private readonly float _framesPerMillisecond;
    private readonly int _totalFramesAmount;
    private int _currentFrameIndex;

    public bool IsAnimationFinished => _currentFrameIndex >= _totalFramesAmount;
    public int CurrentFrameIndex => _currentFrameIndex;
    public int TotalFramesAmount => _totalFramesAmount;

    private static int CalculateTotalFramesAmount(in AnimationInfo info)
    {
        var result = (int)(info.Fps / MillisecondsInSecond * info.TimeLength);
            
        return result > 0 ? result : 1;
    }

    public AnimationController(in Animation animation, in Scene.Scene scene)
    {
        _animation = animation;
        _framesPerMillisecond = _animation.Info.Fps / MillisecondsInSecond;
        _totalFramesAmount = CalculateTotalFramesAmount(_animation.Info);

        foreach (var modelInfo in _animation.ModelsInfo)
        {
            if (scene.Models.ContainsKey(modelInfo.Name))
            {
                _modelsInfo[modelInfo] = scene.Models[modelInfo.Name];
            }
            else
            {
                Logger.Warn($"There is no model with name: {modelInfo.Name}");
            }
        }
    }

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
            
        model.Pivot.GlobalMove(transformation.GlobalMove.X, transformation.GlobalMove.Y, transformation.GlobalMove.Z);
        model.Pivot.LocalMove(transformation.LocalMove.X, transformation.LocalMove.Y, transformation.LocalMove.Z);
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

    public void PrepareSceneToFrame(int frameIndex)
    {
        while (_currentFrameIndex < frameIndex && !IsAnimationFinished)
        {
            PrepareSceneToNextFrame();
        }
    }

    public void PrepareSceneToNextFrame()
    {
        if (!IsAnimationFinished)
        {
            foreach (var (info, model) in _modelsInfo)
            {
                foreach (var actionBinding in info.ActionBindings)
                {
                    var action = _animation.GetActionByName(actionBinding.ActionName);

                    if (action == null)
                    {
                        if (_currentFrameIndex == 0)
                        {
                            Logger.Warn($"There is no action with name: {actionBinding.ActionName}");
                        }
                            
                        continue;
                    }
                        
                    var actionBindingFinalFrameIndex = (int)(_framesPerMillisecond * (actionBinding.TimeLength
                            + actionBinding.StartTime));

                    if (actionBindingFinalFrameIndex >= _currentFrameIndex)
                    {
                        //continue;
                    }

                    for (var i = 0; i < action.States.Count; i++)
                    {
                        var nextState = action.States[i];
                        var nextStateFrameIndex = (int)(_framesPerMillisecond * (nextState.Time
                                + actionBinding.StartTime));
                        nextStateFrameIndex = nextStateFrameIndex > 0 ? nextStateFrameIndex - 1 : 0;
                            
                        if (!actionBinding.UseInterpolation && nextStateFrameIndex == _currentFrameIndex)
                        {
                            TransformModel(model, nextState.Transformation);
                            model.IsVisible = nextState.IsModelVisible;

                            if (nextState.WasColorChanged)
                            {
                                for (var j = 0; j < model.Meshes.Count; j++)
                                {
                                    model.Meshes.Values.ElementAt(j).Color = nextState.Color;
                                }
                            }
                        }
                        else if (actionBinding.UseInterpolation && nextStateFrameIndex > _currentFrameIndex)
                        {
                            var prevState = (i != 0) ? action.States[i - 1] : action.States[i];
                            var prevStateFrameIndex = (int)(_framesPerMillisecond * (prevState.Time
                                    + actionBinding.StartTime));
                            prevStateFrameIndex = prevStateFrameIndex > 0 ? prevStateFrameIndex - 1 : 0;
                            var stepsAmount = nextStateFrameIndex - prevStateFrameIndex;

                            if (stepsAmount > 0)
                            {
                                TransformModel(model, GetStepTransformation(nextState, stepsAmount));
                                    
                                if (nextState.WasColorChanged)
                                {
                                    for (var j = 0; j < model.Meshes.Count; j++)
                                    {
                                        model.Meshes.Values.ElementAt(j).Color = new Color4(
                                                model.Meshes.Values.ElementAt(j).Triangles[0].Vertexes[0].Color.R
                                                        + nextState.Color.R / stepsAmount,
                                                model.Meshes.Values.ElementAt(j).Triangles[0].Vertexes[0].Color.G
                                                        + nextState.Color.G / stepsAmount,
                                                model.Meshes.Values.ElementAt(j).Triangles[0].Vertexes[0].Color.B
                                                        + nextState.Color.B / stepsAmount,
                                                nextState.Color.A);
                                    }
                                }
                            }

                            break;
                        }
                    }
                }
            }

            _currentFrameIndex++;
        }
    }
}