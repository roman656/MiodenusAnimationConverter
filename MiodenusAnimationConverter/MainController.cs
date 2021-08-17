using System;
using System.Collections.Generic;
using MiodenusAnimationConverter.Loaders.ModelLoaders;
using MiodenusAnimationConverter.Scene;
using MiodenusAnimationConverter.Scene.Models;
using Newtonsoft.Json;
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
        private readonly string[] _modelFilenames = {
                //"DebugAssets/Rhm_Borsig_12_8.stl",
                //"DebugAssets/Jagdtiger.stl",
                "DebugAssets/IS-6.stl",
                //"DebugAssets/Sphere.stl",
                //"DebugAssets/Bottle.stl",
        };
        private Scene.Scene _scene = new ();
        
        public MainController(string[] args)
        {
            Logger.Trace("<=====Start=====>");
            Logger.Trace("Processing of command line arguments is started.");
            
            //var argumentsHandler = new CommandLineArgumentsHandler(args);
            
            Logger.Trace("Processing of command line arguments is finished.");

            string maf_filePath = "demo.maf.json";
            var maf = this.GenerateDebugMAF();
            this.WriteAnimationFile(maf_filePath, maf);
            Logger.Trace($"Test MAF file generated to '{maf_filePath}'");
            
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
                models[i] = loader.Load(filename, Color4.ForestGreen, false);
                i++;
            }

            foreach (var model in models)
            {
                _scene.ModelGroups.Add(new ModelGroup(model));
            }

            Logger.Trace("Loading models finished.");
        }

        private AnimationFile.ModelAnimationFile GenerateDebugMAF()
        {
            AnimationFile.ModelAnimationFile maf = new AnimationFile.ModelAnimationFile();
            maf.AnimationInfo.AnimationName = "Generated debug animation";
            maf.AnimationInfo.FPS = 60;
            maf.AnimationInfo.Version = 1;
            maf.AnimationInfo.VideoName = "test.mp4";
            maf.AnimationInfo.TimeLength = 15;
            maf.AnimationInfo.Type = "MAF";

            maf.Models.Add(new AnimationFile.ModelInfo()
            {
                ID = 0,
                FileName = "DebugAssets/Rhm_Borsig_12_8.stl",
                Name = "Борщ",
                BaseTransform = new AnimationFile.Transform()
                {
                    Rotations = new List<AnimationFile.Rotation>()
                    {
                        new AnimationFile.Rotation()
                        {
                            Angle = 90,
                            Vector = new[] { 1.0f, 0.0f, 0.0f }
                        }
                    }
                }
            });
            maf.Models.Add(new AnimationFile.ModelInfo()
            {
                ID = 1,
                FileName = "DebugAssets/Jagdtiger.stl",
                Name = "Тигр",
            });
            maf.Models.Add(new AnimationFile.ModelInfo()
            {
                ID = 2,
                FileName = "DebugAssets/IS-6.stl",
                Name = "Объект 252",
            });
            maf.Models.Add(new AnimationFile.ModelInfo()
            {
                ID = 3,
                FileName = "DebugAssets/Sphere.stl",
                Name = "Сфера",
            });
            maf.Models.Add(new AnimationFile.ModelInfo()
            {
                ID = 4,
                FileName = "DebugAssets/Bottle.stl",
                Name = "Духи",
            });

            maf.Actions.Add(new AnimationFile.Action()
            {
                ID = 0,
                StartTime = 0,
                TimeLength = 10,
                UseInterpolation = false,
                Values = new List<AnimationFile.ActionValue>()
                {
                    new AnimationFile.ActionValue()
                    {
                        Time = 0,
                        Transform = new AnimationFile.Transform()
                    },
                    new AnimationFile.ActionValue()
                    {
                        Time = 10,
                        Transform = new AnimationFile.Transform()
                        {
                            Translation = new[] {0.0f, 10.0f, 0.0f}
                        }
                    }
                }
            });
            maf.Actions.Add(new AnimationFile.Action()
            {
                ID = 1,
                StartTime = 2,
                TimeLength = 10,
                UseInterpolation = false,
                Values = new List<AnimationFile.ActionValue>()
                {
                    new AnimationFile.ActionValue()
                    {
                        Time = 0,
                        Transform = new AnimationFile.Transform()
                    },
                    new AnimationFile.ActionValue()
                    {
                        Time = 10,
                        Transform = new AnimationFile.Transform()
                        {
                            Translation = new[] {0.0f, 0.0f, 5.0f}
                        }
                    }
                }
            });
            maf.Actions.Add(new AnimationFile.Action()
            {
                ID = 2,
                StartTime = 4,
                TimeLength = 10,
                UseInterpolation = false,
                Values = new List<AnimationFile.ActionValue>()
                {
                    new AnimationFile.ActionValue()
                    {
                        Time = 0,
                        Transform = new AnimationFile.Transform()
                    },
                    new AnimationFile.ActionValue()
                    {
                        Time = 10,
                        Transform = new AnimationFile.Transform()
                        {
                            Translation = new[] {15.0f, 0.0f, 0.0f}
                        }
                    }
                }
            });

            maf.Bindings.Add(new AnimationFile.Bind()
            {
                ActionID = 0,
                ModelID = 0
            });
            maf.Bindings.Add(new AnimationFile.Bind()
            {
                ActionID = 1,
                ModelID = 1
            });
            maf.Bindings.Add(new AnimationFile.Bind()
            {
                ActionID = 2,
                ModelID = 2
            });

            return maf;
        }

        private AnimationFile.ModelAnimationFile ReadAnimationFile(string filePath)
        {
            string content = System.IO.File.ReadAllText(filePath);
            AnimationFile.ModelAnimationFile maf = JsonConvert.DeserializeObject<AnimationFile.ModelAnimationFile>(content);
            return maf;
        }

        private void WriteAnimationFile(string filePath, AnimationFile.ModelAnimationFile maf)
        {
            string content = JsonConvert.SerializeObject(maf, Formatting.Indented);
            System.IO.File.WriteAllText(filePath, content);
        }
    }
}