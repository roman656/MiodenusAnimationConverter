using System;
using System.Collections.Generic;
using System.Drawing.Imaging;
using System.Linq;
using System.Runtime.CompilerServices;
using FFMpegCore.Extend;
using FFMpegCore.Pipes;
using MiodenusAnimationConverter.Animation;
using MiodenusAnimationConverter.Media;
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
        private const bool IsCheckingGLErrorsEnabled = false;
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();
        private readonly List<ShaderProgram> _shaderPrograms = new ();
        private int _currentProgramIndex = 0;
        private readonly AnimationController _animationController;
        private readonly VideoRecorder _videoRecorder;
        private readonly AnimationInfo _animationInfo;
        private readonly Scene.Scene _scene;
        private readonly WorkModeEnum _workMode;
        private readonly int _frameNumberToView;
        private readonly int _frameNumberToGetImage;
        private readonly List<BitmapVideoFrameWrapper> _frames = new ();
        private PrimitiveType _drawMode = DefaultDrawMode;
        private bool _isCursorModeActive;
        private bool _isDrawNormalsModeActive;
        private bool _isDrawCamerasModeActive;
        private double _deltaTime;
        private bool _isPaused = true;
        private bool _isFirstIterationFinished;

        public MainWindow(in Animation.Animation animation, in Scene.Scene scene, WorkModeEnum workMode,
                int frameNumberToView, int frameNumberToGetImage, GameWindowSettings gameWindowSettings,
                NativeWindowSettings nativeWindowSettings) : base(gameWindowSettings, nativeWindowSettings)
        {
            _scene = scene;
            _workMode = workMode;
            _animationInfo = animation.Info;
            _frameNumberToView = frameNumberToView;
            _frameNumberToGetImage = frameNumberToGetImage;
            _animationController = new AnimationController(animation, scene);

            if (workMode == WorkModeEnum.Default)
            {
                _videoRecorder = new VideoRecorder(this, animation.Info);
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
            
            _shaderPrograms.Add(new ShaderProgram(mainShaders));
            
            foreach (var shader in mainShaders)
            {
                shader.Delete();
            }

            if (_workMode == WorkModeEnum.FrameView)
            {
                var debugShaders = new List<Shader>
                {
                    new(TransformShader.Code, TransformShader.Type),
                    new(NormalsShader.Code, NormalsShader.Type),
                    new(ColorShader.Code, ColorShader.Type)
                };

                _shaderPrograms.Add(new ShaderProgram(debugShaders));

                foreach (var shader in debugShaders)
                {
                    shader.Delete();
                }
            }

            Logger.Trace("Shader pograms initialization finished.");
        }

        protected override void OnLoad()
        {
            _scene.Initialize();
            InitializeShaderPrograms();

            CursorGrabbed = _workMode == WorkModeEnum.FrameView;

            _scene.LightPointsController.AddLightPoint(new Vector3(0.0f, 6.0f, 6.0f), Color4.White);
            _scene.LightPointsController.AddLightPoint(new Vector3(-6.0f, 6.0f, -6.0f), Color4.White);
            _scene.LightPointsController.AddLightPoint(new Vector3(6.0f, 6.0f, -6.0f), Color4.White);
            _scene.LightPointsController.SetLightPointsTo(_shaderPrograms[_currentProgramIndex]);    // На данном этапе параметры освещения динамически обновлять не будем.

            GL.Enable(EnableCap.DepthTest);
            GL.PolygonMode(MaterialFace.Front, PolygonMode.Fill);
            GL.PatchParameter(PatchParameterInt.PatchVertices, 3);
        }

        protected override void OnUpdateFrame(FrameEventArgs args)
        {
            base.OnUpdateFrame(args);
            
            if (_workMode != WorkModeEnum.FrameView || !IsFocused)
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
            
            if (_workMode != WorkModeEnum.FrameView || !IsFocused)
            {
                return;
            }

            if (args.Key == Keys.Escape)
            {
                Close();
            }
            
            if (args.Key == Keys.P)
            {
                _isPaused = !_isPaused;
            }

            if (args.Key == Keys.N)
            {
                _isDrawNormalsModeActive = !_isDrawNormalsModeActive;
            }

            if (args.Key == Keys.V)
            {
                _isDrawCamerasModeActive = !_isDrawCamerasModeActive;
            }

            if (args.Key == Keys.F1)
            {
                _drawMode = PrimitiveType.Triangles;
            }
            else if (args.Key == Keys.F2)
            {
                _drawMode = PrimitiveType.Lines;
            }
            else if (args.Key == Keys.F3)
            {
                _drawMode = PrimitiveType.Points;
            }
            
            if (args.Key == Keys.G)
            {
                _scene.SwitchGridVisibility();
            }
        }

        protected override void OnMouseMove(MouseMoveEventArgs e)
        {
            base.OnMouseMove(e);
            
            if (!_isCursorModeActive && _workMode == WorkModeEnum.FrameView && IsFocused)
            {
                _scene.CamerasController.CurrentDebugCamera.ProcessMouseMovement(MouseState);
            }
        }

        protected override void OnMouseWheel(MouseWheelEventArgs args)
        {
            base.OnMouseWheel(args);

            if (!_isCursorModeActive && _workMode == WorkModeEnum.FrameView && IsFocused)
            {
                _scene.CamerasController.CurrentDebugCamera.ProcessMouseScroll(args, KeyboardState);
            }
        }

        protected override void OnRenderFrame(FrameEventArgs e)
        {
            base.OnRenderFrame(e);

            if (_workMode == WorkModeEnum.GetFrameImage)
            {
                /*
                 * TODO: проверка, что не запросили кадр за пределами видео.
                 * А точнее срез - если просят 1000 когда есть 500 - выдать 500 (или 501),
                 * ведь дальше изменений всеравно не будет.
                 */
                while (_animationController.CurrentFrameIndex < _frameNumberToGetImage)
                {
                    _animationController.PrepareSceneToNextFrame();
                }
            }
            else if (_workMode == WorkModeEnum.Default)
            {
                var totalFramesAmount = 280;    // временно.
                
                if (_animationController.CurrentFrameIndex >= totalFramesAmount)
                {
                    Close();
                }
                
                _animationController.PrepareSceneToNextFrame();
            }
            else
            {
                if (_frameNumberToView <= 0 && (!_isPaused || !_isFirstIterationFinished))
                {
                    _animationController.PrepareSceneToNextFrame();
                }
                else
                {
                    while (_animationController.CurrentFrameIndex < _frameNumberToView)
                    {
                        _animationController.PrepareSceneToNextFrame();
                    }
                    
                    if (!_isPaused)
                    {
                        _animationController.PrepareSceneToNextFrame();
                    }
                }
            }
            
            _deltaTime = e.Time;

            GL.ClearColor(_animationInfo.BackgroundColor);
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

            if (_workMode == WorkModeEnum.FrameView)
            {
                _scene.Grid.Draw(_scene.CamerasController.CurrentDebugCamera);
                _scene.MajorGrid.Draw(_scene.CamerasController.CurrentDebugCamera);

                for (var i = 0; i < _scene.Models.Count; i++)
                {
                    _scene.Models.Values.ElementAt(i).Draw(_shaderPrograms[_currentProgramIndex],
                        _scene.CamerasController.CurrentDebugCamera, _drawMode);
                }
            }
            else
            {
                for (var i = 0; i < _scene.Models.Count; i++)
                {
                    _scene.Models.Values.ElementAt(i).Draw(_shaderPrograms[_currentProgramIndex],
                        _scene.CamerasController.CurrentCamera, _drawMode);
                }
            }

            if (IsCheckingGLErrorsEnabled)
            {
                CheckGLErrors();
            }

            if (_workMode == WorkModeEnum.FrameView && _isDrawNormalsModeActive)
            {
                for (var i = 0; i < _scene.Models.Count; i++)
                {
                    _scene.Models.Values.ElementAt(i).Draw(_shaderPrograms[_currentProgramIndex + 1],
                            _scene.CamerasController.CurrentDebugCamera);
                }

                if (IsCheckingGLErrorsEnabled)
                {
                    CheckGLErrors();
                }
            }

            if (_workMode == WorkModeEnum.FrameView && _isDrawCamerasModeActive)
            {
                _scene.CamerasController.DrawCameras(_scene.CamerasController.CurrentDebugCamera);
                
                if (IsCheckingGLErrorsEnabled)
                {
                    CheckGLErrors();
                }
            }

            Context.SwapBuffers();
            _isFirstIterationFinished = true;

            if (_workMode == WorkModeEnum.Default)
            {
                _frames.Add(_videoRecorder.CreateVideoFrame());
            }
            else if (_workMode == WorkModeEnum.GetFrameImage)
            {
                TakeScreenshot(_frameNumberToGetImage);
                Close();
            }
        }

        private void CheckGLErrors(int sleepTime = 1000, [CallerLineNumber] int lineNumber = -1,
                [CallerMemberName] string caller = null)
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

        private void TakeScreenshot(int screenshotNumber) => new Screenshot(this).Save(
                $"{Config.ScreenshotDirectory}/{screenshotNumber}_{DateTime.Now:yyyy_MM_dd_HH_mm_ss}",
                ImageFormat.Png);

        private IEnumerable<BitmapVideoFrameWrapper> GetFrames()
        {
            for (var i = 0; i < _frames.Count; i++)
            {
                yield return _frames[i];
            }
        }

        protected override void OnClosed()
        {
            if (_workMode == WorkModeEnum.Default)
            {
                _videoRecorder.CreateVideo(new RawVideoPipeSource(GetFrames()) { FrameRate = _animationInfo.Fps });
            }

            _scene.Delete();

            for (var i = 0; i < _shaderPrograms.Count; i++)
            {
                _shaderPrograms[i].Delete();
            }

            base.OnClosed();
        }
    }
}