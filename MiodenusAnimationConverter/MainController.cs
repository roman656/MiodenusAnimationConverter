using System;
using System.Collections.Generic;
using System.IO;
using MiodenusAnimationConverter.Animation;
using MiodenusAnimationConverter.Loaders.AnimationLoaders;
using MiodenusAnimationConverter.Loaders.ModelLoaders;
using NLog;
using MiodenusAnimationConverter.Scene.Models;
using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;

namespace MiodenusAnimationConverter
{
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

                CheckPath(Config.ScreenshotDirectory);
                CheckPath(Config.VideoDirectory);
                CreateMainWindow(animation, scene, DetermineWorkMode(options), options.FrameNumberToView,
                        options.FrameNumberToGetImage).Run();
            }
            catch (Exception exception)
            {
                Logger.Fatal(exception);
                Program.ExitCode = ExitCodeEnum.AnimationLoadingError;
            }

            Logger.Trace("<======End======>");
            LogManager.Shutdown();
        }
        
        private static void CheckPath(in string path)
        {
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
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
            Logger.Trace("Loading animation started.");
            
            IAnimationLoader loader = new LoaderMaf();
            var animation = loader.Load(animationFilePath);
            
            Logger.Trace("Loading animation finished.");
            return animation;
        }
        
        private static Dictionary<string, Model> LoadModels(in List<ModelInfo> modelsInfo)
        {
            Logger.Trace("Loading models started.");
            
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
                    Logger.Warn(exception);
                }
            }

            Logger.Trace("Loading models finished.");
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
                NumberOfSamples = animation.Info.EnableMultisampling ? 4 : 0,
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
}