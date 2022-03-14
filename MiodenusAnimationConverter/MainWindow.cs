using System;
using System.Collections.Generic;
using System.Drawing.Imaging;
using System.IO;
using System.Runtime.CompilerServices;
using FFMpegCore.Extend;
using FFMpegCore.Pipes;
using MiodenusAnimationConverter.Animation;
using MiodenusAnimationConverter.Media;
using MiodenusAnimationConverter.Scene;
using MiodenusAnimationConverter.Shaders;
using MiodenusAnimationConverter.Shaders.FragmentShaders;
using MiodenusAnimationConverter.Shaders.GeometryShaders;
using MiodenusAnimationConverter.Shaders.VertexShaders;
using NLog;
using OpenTK.Graphics.OpenGL;
using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;
using OpenTK.Windowing.GraphicsLibraryFramework;
using ErrorCode = OpenTK.Graphics.OpenGL.ErrorCode;

namespace MiodenusAnimationConverter
{
    public class MainWindow : GameWindow
    {
        private const PrimitiveType DefaultDrawMode = PrimitiveType.Triangles;
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();
        private List<ShaderProgram> _shaderPrograms = new ();
        private int _currentProgramIndex = 0;
        private readonly Color4 _backgroundColor;
        private int _screenshotId;
        private float _angle;
        private double _deltaTime;
        private List<BitmapVideoFrameWrapper> frames = new ();
        private bool _isCursorGrabbed = true;
        private Scene.Scene _scene;
        private VideoRecorder _video;
        private readonly PrimitiveType _drawMode = DefaultDrawMode;
        private bool _isCursorModeActive;
        private float _rotationRate = 0.1f;
        private LightPoint _lightPoint1;
        private LightPoint _lightPoint2;
        private bool _isDebugMode;
        private bool _isDrawCamerasModeActive;
        private AnimationController _animationController;

        public MainWindow(Animation.Animation animation, Scene.Scene scene, GameWindowSettings gameWindowSettings,
                NativeWindowSettings nativeWindowSettings) : base(gameWindowSettings, nativeWindowSettings)
        {
            CheckPath(Config.ScreenshotDirectory);
            CheckPath(Config.VideoDirectory);
            _scene = scene;
            _video = new VideoRecorder(this,$"{Config.VideoDirectory}/animation", "mp4", animation.Info.Fps);
            _animationController = new AnimationController(animation, _scene);
            _backgroundColor = animation.Info.BackgroundColor;
        }

        private static void CheckPath(in string path)
        {
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
        }

        private void InitializeShaderPrograms()
        {
            Logger.Trace("Shader pograms initialization started.");
            
            var mainShaders = new List<Shader>
            {
                new (TransformShader.Code, TransformShader.Type),
                new (LightingShader.Code, LightingShader.Type)
            };
            
            var debugShaders = new List<Shader>
            {
                new (TransformShader.Code, TransformShader.Type),
                new (NormalsShader.Code, NormalsShader.Type),
                new (ColorShader.Code, ColorShader.Type)
            };

            _shaderPrograms.Add(new ShaderProgram(mainShaders));
            _shaderPrograms.Add(new ShaderProgram(debugShaders));

            foreach (var shader in mainShaders)
            {
                shader.Delete();
            }

            foreach (var shader in debugShaders)
            {
                shader.Delete();
            }
            
            Logger.Trace("Shader pograms initialization finished.");
        }

        protected override void OnLoad()
        {
            _scene.Initialize();

            _lightPoint1 = _scene.LightPointsController.AddLightPoint(new Vector3(0.0f, 7.0f, -3.0f), Color4.White);

            CursorGrabbed = _isCursorGrabbed;

            _scene.ModelGroups[0].Models["0"].Scale = new Vector3(0.015f);
            _scene.ModelGroups[1].Models["1"].Scale = new Vector3(0.015f);
            _scene.ModelGroups[2].Models["2"].Scale = new Vector3(0.015f);
            _scene.ModelGroups[3].Models["3"].Scale = new Vector3(0.015f);
            _scene.ModelGroups[4].Models["4"].Scale = new Vector3(0.015f);
            _scene.ModelGroups[5].Models["5"].Scale = new Vector3(0.015f);

            InitializeShaderPrograms();

            GL.Enable(EnableCap.DepthTest);
            GL.PolygonMode(MaterialFace.Front, PolygonMode.Fill);
            GL.PatchParameter(PatchParameterInt.PatchVertices, 3);
        }

        protected override void OnUpdateFrame(FrameEventArgs args)
        {
            base.OnUpdateFrame(args);
            
            if (!IsFocused)
            {
                return;
            }
            
            if (KeyboardState.IsKeyDown(Keys.LeftControl))
            {
                CursorVisible = true;
                _isCursorModeActive = true;
            }
            else
            {
                _isCursorModeActive = false;
                CursorGrabbed = true;
                _scene.CamerasController.CurrentDebugCamera.ProcessKeyboard(KeyboardState, _deltaTime);
            }
        }

        protected override void OnKeyDown(KeyboardKeyEventArgs args)
        {
            base.OnKeyDown(args);

            switch (args.Key)
            {
                case Keys.B:
                {
                    _isDebugMode = !_isDebugMode;
                    break;
                }
                case Keys.Up:
                {
                    _scene.ModelGroups[0].Move(0.0f, -20.0f, 0.0f);
                    break;
                }
                case Keys.Down:
                {
                    _scene.ModelGroups[0].Move(0.0f, 20.0f, 0.0f);
                    break;
                }
                case Keys.Left:
                {
                    _scene.ModelGroups[0].Move(-20.0f, 0.0f, 0.0f);
                    break;
                }
                case Keys.Right:
                {
                    _scene.ModelGroups[0].Move(20.0f, 0.0f, 0.0f);
                    break;
                }
                case Keys.Y:
                {
                    _scene.ModelGroups[0].Rotate((float)Math.PI / 8, new Vector3(1, 0, 0));
                    break;
                }
                case Keys.U:
                {
                    _scene.ModelGroups[0].Rotate((float)Math.PI / 8, new Vector3(0, 1, 0));
                    break;
                }
                case Keys.M:
                {
                    _scene.ModelGroups[0].Scale(0.99f, 0.99f, 0.99f);
                    break;
                }
                case Keys.N:
                {
                    _scene.ModelGroups[0].Scale(1.01f, 1.01f, 1.01f);
                    break;
                }
                case Keys.H:
                {
                    _scene.LightPointsController.AddLightPoint(new Vector3(0.0f, 1.0f, 2.0f), Color4.Olive);
                    break;
                }
                case Keys.I:
                {
                    _scene.CamerasController.CurrentDebugCamera.LookAt(new Vector3(0.0f));
                    break;
                }
                case Keys.V:
                {
                    _isDrawCamerasModeActive = !_isDrawCamerasModeActive;
                    break;
                }
                case Keys.G:
                {
                    _scene.Grid.IsXzPlaneVisible = !_scene.Grid.IsXzPlaneVisible;
                    _scene.MajorGrid.IsXzPlaneVisible = !_scene.MajorGrid.IsXzPlaneVisible;
                    _scene.MajorGrid.Pivot.IsVisible = !_scene.MajorGrid.Pivot.IsVisible;
                    break;
                }
            }
        }

        protected override void OnMouseMove(MouseMoveEventArgs e)
        {
            base.OnMouseMove(e);
            if (!_isCursorModeActive)
            {
                _scene.CamerasController.CurrentDebugCamera.ProcessMouseMovement(MouseState);
            }
        }

        protected override void OnMouseWheel(MouseWheelEventArgs args)
        {
            base.OnMouseWheel(args);

            if (!_isCursorModeActive)
            {
                _scene.CamerasController.CurrentDebugCamera.ProcessMouseScroll(args, KeyboardState);
            }
        }

        protected override void OnRenderFrame(FrameEventArgs e)
        {
            /*if (_animationController.CurrentFrameIndex > 181)
            {
                Close();
            }*/

            base.OnRenderFrame(e);

            //_animationController.PrepareSceneToNextFrame();
            _deltaTime = e.Time;

            GL.ClearColor(_backgroundColor);
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

            _angle = (float) (_deltaTime * _rotationRate);
            _scene.CamerasController.CurrentCamera.GlobalRotate(_angle, new Vector3(0.0f, 1.0f, 0.0f));
            _scene.CamerasController.CurrentCamera.LookAt(new Vector3(0.0f, 0.5f, 0.0f));
            //_lightPoint1.Rotate(_angle, new Vector3(0, 0, 1));

            _scene.LightPointsController.SetLightPointsTo(_shaderPrograms[_currentProgramIndex]);

            CheckGLErrors();
            
            _scene.Grid.Draw(_scene.CamerasController.CurrentDebugCamera);
            _scene.MajorGrid.Draw(_scene.CamerasController.CurrentDebugCamera);

            for (var i = 0; i < _scene.ModelGroups.Count; i++)
            {
                _scene.ModelGroups[i].Draw(_shaderPrograms[_currentProgramIndex],
                        _scene.CamerasController.CurrentDebugCamera, _drawMode);
            }

            CheckGLErrors();

            if (_isDebugMode)
            {
                CheckGLErrors();
                
                for (var i = 0; i < _scene.ModelGroups.Count; i++)
                {
                    _scene.ModelGroups[i].Draw(_shaderPrograms[_currentProgramIndex + 1],
                            _scene.CamerasController.CurrentDebugCamera);
                }

                CheckGLErrors();
            }

            if (_isDrawCamerasModeActive)
            {
                _scene.CamerasController.DrawCameras(_scene.CamerasController.CurrentDebugCamera);
                CheckGLErrors();
            }

            Context.SwapBuffers();
            
            //frames.Add(_video.CreateVideoFrame());
            //TakeScreenshot(_screenshotId++);
        }

        private void CheckGLErrors(int sleepTime = 1000, [CallerLineNumber] int lineNumber = -1, [CallerMemberName] string caller = null)
        {
            var hasError = false;
            ErrorCode errorCode;
            
            while ((errorCode = GL.GetError()) != ErrorCode.NoError)
            {
                hasError = true;
                Logger.Error($"{caller}: detected OpenGL error on line {lineNumber}. Type: {errorCode}.");
            }

            if (hasError)
            {
                System.Threading.Thread.Sleep(sleepTime);
            }
        }

        private void TakeScreenshot(int screenshotNumber)
        {
            new Screenshot(this).Save($"{Config.ScreenshotDirectory}/{screenshotNumber}_{DateTime.Now:yyyy_MM_dd_HH_mm_ss}",
                    ImageFormat.Png);
        }
        
        public IEnumerable<IVideoFrame> GetBitmaps()
        {
            for (var i = 0; i < frames.Count; i++)
            {
                yield return frames[i];
            }
        }

        protected override void OnClosed()
        {
/*
            var videoFramesSource = new RawVideoPipeSource(GetBitmaps())
            {
                FrameRate = 60
            };
            
            _video.CreateVideo(videoFramesSource);
*/
            _scene.Delete();

            for (var i = 0; i < _shaderPrograms.Count; i++)
            {
                _shaderPrograms[i].Delete();
            }

            base.OnClosed();
        }
    }
}