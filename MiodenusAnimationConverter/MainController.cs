using System;
using MiodenusAnimationConverter.Loaders;
using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;

namespace MiodenusAnimationConverter
{
    public class MainController
    {
        private readonly MainWindow _mainWindow;
        private readonly string _mainWindowTitle = "Miodenus Animation Converter";
        private readonly ushort _mainWindowWidth = 600;
        private readonly ushort _mainWindowHeight = 600;
        private readonly bool _isMainWindowVisible = true;
        private readonly byte _mainWindowFrequency = 60;
        private readonly string _animationFilename = "";
        private readonly string[] _modelFilenames = { "/home/roman/STL/IS-6.stl" };    // Временное решение.
        private Model[] _models;
        
        public MainController(string[] args)
        {
            Console.WriteLine($"Start time: {DateTime.Now} {DateTime.Now.Millisecond} ms.");
            
            /* TODO: обработка консольных аргументов. */
            
            LoadModels();
            _mainWindow = CreateMainWindow();
            _mainWindow.Run();
            
            Console.WriteLine($"End time: {DateTime.Now} {DateTime.Now.Millisecond} ms.");
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
            uint i = 0;
            _models = new Model[_modelFilenames.Length];
            IModelLoader loader = new LoaderStl();
            
            foreach (var filename in _modelFilenames)
            {
                _models[i] = loader.Load(filename);
                i++;
            }
        }
    }
}