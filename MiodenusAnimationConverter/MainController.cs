using System;
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
        private static readonly Logger _logger = LogManager.GetCurrentClassLogger();
        private readonly MainWindow _mainWindow;
        private readonly string _mainWindowTitle = "Miodenus Animation Converter";
        private readonly ushort _mainWindowWidth = 600;
        private readonly ushort _mainWindowHeight = 600;
        private readonly bool _isMainWindowVisible = true;
        private readonly byte _mainWindowFrequency = 60;
        private readonly string _animationFilename = "";
        private readonly string[] _modelFilenames = { "/home/roman/STL/Rhm._Borsig_12.8.stl",
                                                      "/home/roman/STL/IS-6.stl" };    // Временное решение.
        private Model[] _models;
        
        public MainController(string[] args)
        {
            _logger.Trace("<=====Start=====>");

            //new CommandLineArgumentsHandler(args);
            
            LoadModels();
           
            _mainWindow = CreateMainWindow();
            _mainWindow.Run();
            
            _logger.Trace("<======End======>");
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

            return new MainWindow(_models, mainWindowSettings, nativeWindowSettings);
        }

        private void LoadModels()
        {
            _logger.Trace("Loading models started.");
            
            uint i = 0;
            _models = new Model[_modelFilenames.Length];
            IModelLoader loader = new LoaderStl();

            foreach (var filename in _modelFilenames)
            {
                _models[i] = loader.Load(filename, Color4.SteelBlue, false);
                i++;
            }
            
            _logger.Trace("Loading models finished.");
        }
    }
}