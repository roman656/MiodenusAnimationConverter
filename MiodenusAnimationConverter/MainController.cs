using System;
using System.Collections.Generic;
using MiodenusAnimationConverter.Animation;
using MiodenusAnimationConverter.Loaders.AnimationLoaders;
using MiodenusAnimationConverter.Loaders.ModelLoaders;
using MiodenusAnimationConverter.Scene.Models;
using NLog;
using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;

namespace MiodenusAnimationConverter
{
    public class MainController
    {
        private enum WorkMode
        {
            Default,
            FrameView,
            GetFrameImage
        }
        
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        public MainController(in CommandLineOptions options)
        {
            Logger.Trace("<=====Start=====>");

            try
            {
                var animation = LoadAnimation(options.AnimationFilePath);
                var models = LoadModels(animation.ModelsInfo);
                var scene = new Scene.Scene(animation.Info, models);

                CreateMainWindow(animation, scene, DetermineWorkMode(options)).Run();
            }
            catch (Exception exception)
            {
                Logger.Fatal(exception);
                Program.ExitCode = ExitCodeEnum.AnimationLoadingError;
            }

            Logger.Trace("<======End======>");
            LogManager.Shutdown();
        }
        
        private static WorkMode DetermineWorkMode(in CommandLineOptions options)
        {
            var result = WorkMode.Default;

            if (options.WasFrameNumberToViewOptionGot)
            {
                result = WorkMode.FrameView;
            }
            else if (options.WasFrameNumberToGetImageOptionGot)
            {
                result = WorkMode.GetFrameImage;
            }
            
            Logger.Trace("Working mode: {0}", result);
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
        
        private static List<Model> LoadModels(in List<ModelInfo> modelsInfo)
        {
            Logger.Trace("Loading models started.");
            
            var models = new List<Model>();
            IModelLoader loader = new LoaderStl();

            foreach (var modelInfo in modelsInfo)
            {
                try
                {
                    var model = loader.Load(modelInfo.Filename, modelInfo.Color, modelInfo.UseCalculatedNormals);
                    
                    model.Name = modelInfo.Name;
                    models.Add(model);
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
                WorkMode workMode)
        {
            GameWindowSettings mainWindowSettings = new()
            {
                IsMultiThreaded = true,
                RenderFrequency = animation.Info.Fps,
                UpdateFrequency = animation.Info.Fps
            };
            
            NativeWindowSettings nativeWindowSettings = new()
            {
                Size = new Vector2i(animation.Info.FrameWidth, animation.Info.FrameHeight),
                Title = Config.MainWindowTitle,
                WindowBorder = WindowBorder.Fixed,
                API = ContextAPI.OpenGL,
                StartVisible = workMode == WorkMode.FrameView,
                NumberOfSamples = 4,
                Location = CalculateCenteredWindowLocation(animation.Info.FrameWidth, animation.Info.FrameHeight)
            };

            return new MainWindow(animation, scene, mainWindowSettings, nativeWindowSettings);
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