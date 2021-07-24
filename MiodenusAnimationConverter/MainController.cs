using System.Collections.Generic;
using MiodenusAnimationConverter.Loaders.ModelLoaders;
using MiodenusAnimationConverter.Scene;
using MiodenusAnimationConverter.Scene.Models;
using NLog;
using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;

namespace MiodenusAnimationConverter
{
    public class MainController
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();
        private readonly MainWindow _mainWindow;
        private readonly string _mainWindowTitle = "Miodenus Animation Converter";
        private readonly ushort _mainWindowWidth = 600;
        private readonly ushort _mainWindowHeight = 600;
        private readonly bool _isMainWindowVisible = true;
        private readonly byte _mainWindowFrequency = 60;
        private readonly string _animationFilename = "";
        private readonly string[] _modelFilenames = { "DebugAssets/Rhm_Borsig_12_8.stl",
                                                      "DebugAssets/Jagdtiger.stl",
                                                      "DebugAssets/IS-6.stl",
                                                      "DebugAssets/Sphere.stl",
                                                      "DebugAssets/Bottle.stl" };    // Временное решение.
        private Scene.Scene _scene = new ();
        
        public MainController(string[] args)
        {
            Logger.Trace("<=====Start=====>");
            Logger.Trace("Processing of command line arguments is started.");
            
            //var argumentsHandler = new CommandLineArgumentsHandler(args);
            
            Logger.Trace("Processing of command line arguments is finished.");
            
            LoadModels();
           
            _mainWindow = CreateMainWindow();
            _mainWindow.Run();
            
            Logger.Trace("<======End======>");
            LogManager.Shutdown();
        }

        private MainWindow CreateMainWindow()
        {
            GameWindowSettings mainWindowSettings = new()
            {
                IsMultiThreaded = true,
                RenderFrequency = _mainWindowFrequency,
                UpdateFrequency = _mainWindowFrequency
            };
            
            NativeWindowSettings nativeWindowSettings = new()
            {
                Size = new Vector2i(_mainWindowWidth, _mainWindowHeight),
                Title = _mainWindowTitle,
                WindowBorder = WindowBorder.Fixed,
                API = ContextAPI.OpenGL,
                StartVisible = _isMainWindowVisible
            };

            return new MainWindow(_scene, mainWindowSettings, nativeWindowSettings);
        }

        private void LoadModels()
        {
            Logger.Trace("Loading models started.");
            
            uint i = 0;
            var models = new Model[_modelFilenames.Length];
            IModelLoader loader = new LoaderStl();

            foreach (var filename in _modelFilenames)
            {
                models[i] = loader.Load(filename, Color4.SteelBlue, false);
                i++;
            }

            foreach (var model in models)
            {
                _scene.ModelGroups.Add(new ModelGroup(model));
            }

            Logger.Trace("Loading models finished.");
        }
    }
}