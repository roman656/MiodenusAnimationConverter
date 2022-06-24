namespace MiodenusAnimationConverter;

using System;
using System.Collections.Generic;
using System.IO;
using Animation;
using Loaders.AnimationLoaders;
using Loaders.ModelLoaders;
using NLog;
using Scene.Models;
using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;

public class MainController
{
    private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

    public MainController(in CommandLineOptions options)
    {
        Logger.Trace("<=====Start=====>");

        try
        {
            var animation = LoadAnimation(options.AnimationFilePath);
            var models = LoadModels(animation.ModelsInfo);
            var scene = new Scene.Scene(animation.Info, models);
                
            CheckDirectory(Config.ScreenshotDirectory);
            CheckDirectory(Config.VideoDirectory);
            CreateMainWindow(animation, scene, DetermineWorkMode(options), options.FrameNumberToView,
                    options.FrameNumberToGetImage).Run();
        }
        catch (Exception exception)
        {
            Logger.Error(exception.Message);
            Program.ExitCode = ExitCodeEnum.AnimationLoadingError;
        }

        Logger.Trace("<======End======>");
        LogManager.Shutdown();
    }
        
    private static void CheckDirectory(in string path)
    {
        Logger.Trace($"Checking directory: {path}");
        
        if (!Directory.Exists(path))
        {
            Directory.CreateDirectory(path);
            Logger.Trace("Directory created");
        }
    }
        
    private static WorkModeEnum DetermineWorkMode(in CommandLineOptions options)
    {
        var result = WorkModeEnum.Default;

        if (options.WasFrameNumberToViewOptionGot)
        {
            result = WorkModeEnum.FrameView;
        }
        else if (options.WasFrameNumberToGetImageOptionGot)
        {
            result = WorkModeEnum.GetFrameImage;
        }

        Logger.Trace($"Working mode: {result}");
        
        return result;
    }

    private static Animation.Animation LoadAnimation(in string animationFilePath)
    {
        Logger.Trace("Animation loading started");
            
        IAnimationLoader loader = new LoaderMaf();
        var animation = loader.Load(animationFilePath);
            
        Logger.Trace("Animation loading finished");
        
        return animation;
    }
        
    private static Dictionary<string, Model> LoadModels(in List<ModelInfo> modelsInfo)
    {
        Logger.Trace("Models loading started");
            
        var models = new Dictionary<string, Model>();
        IModelLoader loader = new LoaderStl();

        foreach (var modelInfo in modelsInfo)
        {
            try
            {
                models[modelInfo.Name] = loader.Load(modelInfo);
            }
            catch (Exception exception)
            {
                Logger.Warn(exception.Message);
            }
        }

        Logger.Trace("Models loading finished");
        
        return models;
    }

    private static MainWindow CreateMainWindow(in Animation.Animation animation, in Scene.Scene scene,
            WorkModeEnum workMode, int frameNumberToView, int frameNumberToGetImage)
    {
        GameWindowSettings mainWindowSettings = new()
        {
            IsMultiThreaded = true,
            RenderFrequency = workMode == WorkModeEnum.FrameView ? animation.Info.Fps : 0,
            UpdateFrequency = workMode == WorkModeEnum.FrameView ? animation.Info.Fps : 0
        };
            
        NativeWindowSettings nativeWindowSettings = new()
        {
            Size = new Vector2i(animation.Info.FrameWidth, animation.Info.FrameHeight),
            Title = Config.MainWindowTitle,
            WindowBorder = WindowBorder.Fixed,
            API = ContextAPI.OpenGL,
            StartVisible = workMode == WorkModeEnum.FrameView,
            NumberOfSamples = animation.Info.EnableMultisampling ? Config.DefaultNumberOfSamples : 0,
            Location = CalculateCenteredWindowLocation(animation.Info.FrameWidth, animation.Info.FrameHeight)
        };

        return new MainWindow(animation, scene, workMode, frameNumberToView, frameNumberToGetImage,
                mainWindowSettings, nativeWindowSettings);
    }

    private static Vector2i? CalculateCenteredWindowLocation(int frameWidth, int frameHeight)
    {
        Vector2i? windowLocation;
            
        if (Monitors.TryGetMonitorInfo(0, out var monitorInfo))
        {
            /* Окно будет выровнено по центру экрана. */
            windowLocation = new Vector2i((monitorInfo.HorizontalResolution - frameWidth) / 2,
                    (monitorInfo.VerticalResolution - frameHeight) / 2);
        }
        else
        {
            /* Положение окна определит ОС. */
            windowLocation = null;
        }

        return windowLocation;
    }
}