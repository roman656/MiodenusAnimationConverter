using System;
using System.Collections.Generic;
using System.Drawing.Imaging;
using System.IO;
using System.Runtime.CompilerServices;
using FFMpegCore.Extend;
using FFMpegCore.Pipes;
using MiodenusAnimationConverter.Media;
using MiodenusAnimationConverter.Scene;
using MiodenusAnimationConverter.Scene.Models.Meshes;
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
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();
        private readonly string _screenshotsPath;
        private readonly string _videoPath;
        private List<ShaderProgram> _shaderPrograms = new ();
        private int _currentProgramIndex = 0;
        
        private readonly Color4 _backgroundColor = new (0.3f, 0.3f, 0.4f, 1.0f);

        private int _vertexesAmount;
        private long _screenshotId;
        
        private float _angle;
        private double _deltaTime;

        private List<BitmapVideoFrameWrapper> frames = new ();
        private bool _isCursorGrabbed = true;
        private Scene.Scene _scene;
        private VideoRecorder _video;
        private Vertex[] _vertexes;
        private Transformation[] _transformations;
        private VertexArrayObject _mainVao;
        private PrimitiveType _drawMode = PrimitiveType.Triangles;
        private int _locationVboIndex;
        private int _rotationVboIndex;
        private int _scaleVboIndex;
        private bool _hasTransformed;
        private bool _isCursorModeActive;
        
        private float[] _vertexesLocation;
        private float[] _vertexesRotation;
        private float[] _vertexesScale;
        private float _rotationRate = 2.0f;
        private LightPoint _lightPoint1;
        private LightPoint _lightPoint2;
        private bool _isDebugMode;
        private bool _isDrawCamerasModeActive;

        public MainWindow(Scene.Scene scene, GameWindowSettings gameWindowSettings,
            NativeWindowSettings nativeWindowSettings) : base(gameWindowSettings, nativeWindowSettings)
        {
            _screenshotsPath = "screenshots";
            _videoPath = "videos";
            CheckPath(_screenshotsPath);
            CheckPath(_videoPath);
            _scene = scene;
            _video = new VideoRecorder(this,$"{_videoPath}/animation", "mp4", 60);
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
            _scene.CamerasController.Initialize();
            _lightPoint1 = _scene.LightPointsController.AddLightPoint(new Vector3(0.0f, 7.0f, 0.0f), Color4.White);

            _scene.ModelGroups[0].Scale(0.025f, 0.025f, 0.025f);
            _scene.ModelGroups[0].Rotate(-MathHelper.Pi / 2.0f, new Vector3(1.0f, 0.0f, 0.0f));

            _vertexes = _scene.Vertexes;

            var vertexesPositions = new float[_vertexes.Length * 3];
            var vertexesNormals = new float[_vertexes.Length * 3];
            var vertexesColors = new float[_vertexes.Length * 4];
            
            _vertexesLocation = new float[_vertexes.Length * 3];
            _vertexesRotation = new float[_vertexes.Length * 4];
            _vertexesScale = new float[_vertexes.Length * 3];

            for (int i = 0, j = 0, k = 0; i < _vertexes.Length; i += 1, j += 3, k += 4)
            {
                _vertexesLocation[j] = _vertexes[i].Transformation.Location.X;
                _vertexesLocation[j + 1] = _vertexes[i].Transformation.Location.Y;
                _vertexesLocation[j + 2] = _vertexes[i].Transformation.Location.Z;
                
                _vertexesRotation[k] = _vertexes[i].Transformation.Rotation.X;
                _vertexesRotation[k + 1] = _vertexes[i].Transformation.Rotation.Y;
                _vertexesRotation[k + 2] = _vertexes[i].Transformation.Rotation.Z;
                _vertexesRotation[k + 3] = _vertexes[i].Transformation.Rotation.W;
                
                _vertexesScale[j] = _vertexes[i].Transformation.Scale.X;
                _vertexesScale[j + 1] = _vertexes[i].Transformation.Scale.Y;
                _vertexesScale[j + 2] = _vertexes[i].Transformation.Scale.Z;

                vertexesPositions[j] = _vertexes[i].Position.X;
                vertexesPositions[j + 1] = _vertexes[i].Position.Y;
                vertexesPositions[j + 2] = _vertexes[i].Position.Z;
                
                vertexesNormals[j] = _vertexes[i].Normal.X;
                vertexesNormals[j + 1] = _vertexes[i].Normal.Y;
                vertexesNormals[j + 2] = _vertexes[i].Normal.Z;

                vertexesColors[k] = _vertexes[i].Color.R;
                vertexesColors[k + 1] = _vertexes[i].Color.G;
                vertexesColors[k + 2] = _vertexes[i].Color.B;
                vertexesColors[k + 3] = _vertexes[i].Color.A;
            }
            
            _mainVao = new VertexArrayObject();
            
            _mainVao.AddVertexBufferObject(vertexesPositions, 3);
            _mainVao.AddVertexBufferObject(vertexesNormals, 3);
            _mainVao.AddVertexBufferObject(vertexesColors, 4);
            
            _mainVao.AddVertexBufferObject(_vertexesLocation, 3, BufferUsageHint.StreamDraw);
            _locationVboIndex = _mainVao.VertexBufferObjectIndexes[^1];
            _mainVao.AddVertexBufferObject(_vertexesRotation, 4, BufferUsageHint.StreamDraw);
            _rotationVboIndex = _mainVao.VertexBufferObjectIndexes[^1];
            _mainVao.AddVertexBufferObject(_vertexesScale, 3, BufferUsageHint.StreamDraw);
            _scaleVboIndex = _mainVao.VertexBufferObjectIndexes[^1];
            
            CursorGrabbed = _isCursorGrabbed;
            _vertexesAmount = _vertexes.Length;

            InitializeShaderPrograms();
            
            GL.Enable(EnableCap.DepthTest);
            GL.PolygonMode(MaterialFace.Front, PolygonMode.Fill);
            GL.PatchParameter(PatchParameterInt.PatchVertices, 3);
        }

        private void UpdateModelsTransformation()
        {
            if (_hasTransformed)
            {
                for (int i = 0, j = 0, k = 0; i < _vertexes.Length; i += 1, j += 3, k += 4)
                {
                    _vertexesLocation[j] = _vertexes[i].Transformation.Location.X;
                    _vertexesLocation[j + 1] = _vertexes[i].Transformation.Location.Y;
                    _vertexesLocation[j + 2] = _vertexes[i].Transformation.Location.Z;

                    _vertexesRotation[k] = _vertexes[i].Transformation.Rotation.X;
                    _vertexesRotation[k + 1] = _vertexes[i].Transformation.Rotation.Y;
                    _vertexesRotation[k + 2] = _vertexes[i].Transformation.Rotation.Z;
                    _vertexesRotation[k + 3] = _vertexes[i].Transformation.Rotation.W;

                    _vertexesScale[j] = _vertexes[i].Transformation.Scale.X;
                    _vertexesScale[j + 1] = _vertexes[i].Transformation.Scale.Y;
                    _vertexesScale[j + 2] = _vertexes[i].Transformation.Scale.Z;
                }

                _mainVao.UpdateVertexBufferObject(_locationVboIndex, _vertexesLocation);
                _mainVao.UpdateVertexBufferObject(_rotationVboIndex, _vertexesRotation);
                _mainVao.UpdateVertexBufferObject(_scaleVboIndex, _vertexesScale);
            }
            
            _hasTransformed = false;
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
                _scene.CamerasController.DebugCameras[0].ProcessKeyboard(KeyboardState, _deltaTime);
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
                    for (var i = 0; i < _vertexesAmount; i++)
                    {
                        _vertexes[i].Move(0, -25, 0);
                    }
                    
                    _hasTransformed = true;
                    break;
                }
                case Keys.Down:
                {
                    for (var i = 0; i < _vertexesAmount; i++)
                    {
                        _vertexes[i].Move(0, 25, 0);
                    }
                    
                    _hasTransformed = true;
                    break;
                }
                case Keys.Left:
                {
                    for (var i = 0; i < _vertexesAmount; i++)
                    {
                        _vertexes[i].Move(-25, 0, 0);
                    }
                    
                    _hasTransformed = true;
                    break;
                }
                case Keys.Right:
                {
                    for (var i = 0; i < _vertexesAmount; i++)
                    {
                        _vertexes[i].Move(25, 0, 0);
                    }

                    _hasTransformed = true;
                    break;
                }
                case Keys.Y:
                {
                    for (var i = 0; i < _vertexesAmount; i++)
                    {
                        _vertexes[i].Rotate((float)Math.PI / 8, new Vector3(1, 0, 0));
                    }

                    _hasTransformed = true;
                    break;
                }
                case Keys.U:
                {
                    for (var i = 0; i < _vertexesAmount; i++)
                    {
                        _vertexes[i].Rotate((float)Math.PI / 8, new Vector3(0, 1, 0));
                    }

                    _hasTransformed = true;
                    break;
                }
                case Keys.T:
                {
                    for (var i = 0; i < _vertexesAmount; i++)
                    {
                        _vertexes[i].Rotate((float)Math.PI / 8, new Vector3(0, 0, 1));
                    }

                    _hasTransformed = true;
                    break;
                }
                case Keys.M:
                {
                    for (var i = 0; i < _vertexesAmount; i++)
                    {
                        _vertexes[i].Scale(0.99f, 0.99f, 0.99f);
                    }

                    _hasTransformed = true;
                    break;
                }
                case Keys.N:
                {
                    for (var i = 0; i < _vertexesAmount; i++)
                    {
                        _vertexes[i].Scale(1.01f, 1.01f, 1.01f);
                    }

                    _hasTransformed = true;
                    break;
                }
                case Keys.H:
                {
                    _scene.LightPointsController.AddLightPoint(new Vector3(0.0f, 1.0f, 2.0f), Color4.Olive);
                    break;
                }
                case Keys.L:
                {
                    _scene.CamerasController.DebugCameras[0].SwitchCoordinateSystem();
                    break;
                }
                case Keys.I:
                {
                    _scene.CamerasController.DebugCameras[0].LookAt(new Vector3(0.0f));
                    break;
                }
                case Keys.V:
                {
                    _isDrawCamerasModeActive = !_isDrawCamerasModeActive;
                    break;
                }
            }
        }

        protected override void OnMouseMove(MouseMoveEventArgs e)
        {
            base.OnMouseMove(e);
            if (!_isCursorModeActive)
            {
                _scene.CamerasController.DebugCameras[0].ProcessMouseMovement(MouseState);
            }
        }

        protected override void OnMouseWheel(MouseWheelEventArgs args)
        {
            base.OnMouseWheel(args);

            if (!_isCursorModeActive)
            {
                _scene.CamerasController.DebugCameras[0].ProcessMouseScroll(args, KeyboardState);
            }
        }

        protected override void OnRenderFrame(FrameEventArgs e)
        {
            base.OnRenderFrame(e);
            
            _deltaTime = e.Time;
            
            GL.ClearColor(_backgroundColor);
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

            UpdateModelsTransformation();
            
            _angle = (float)(_deltaTime * _rotationRate);
            /*_scene.Cameras[0].Rotate(_angle, new Vector3(0.0f, 1.0f, 0.0f));
            _scene.Cameras[0].LookAt(new Vector3(0.0f));*/
            //_lightPoint1.Rotate(_angle, new Vector3(0, 0, 1));

            _shaderPrograms[_currentProgramIndex].SetMatrix4("view", _scene.CamerasController.DebugCameras[0].ViewMatrix, false);
            _shaderPrograms[_currentProgramIndex].SetMatrix4("projection", _scene.CamerasController.DebugCameras[0].ProjectionMatrix, false);
            _scene.LightPointsController.SetLightPointsTo(_shaderPrograms[_currentProgramIndex]);

            CheckGLErrors();
            
            _mainVao.Draw(_vertexesAmount, _drawMode);
            
            CheckGLErrors();

            if (_isDebugMode)
            {
                _shaderPrograms[_currentProgramIndex + 1].SetMatrix4("view", _scene.CamerasController.DebugCameras[0].ViewMatrix, false);
                _shaderPrograms[_currentProgramIndex + 1].SetMatrix4("projection", _scene.CamerasController.DebugCameras[0].ProjectionMatrix, false);

                CheckGLErrors();
                
                _mainVao.Draw(_vertexesAmount);
                
                CheckGLErrors();
            }

            if (_isDrawCamerasModeActive)
            {
                _scene.CamerasController.DrawCameras(_scene.CamerasController.DebugCameras[0].ViewMatrix, _scene.CamerasController.DebugCameras[0].ProjectionMatrix);
                CheckGLErrors();
            }

            Context.SwapBuffers();

            //frames.Add(video.CreateVideoFrame());
            //TakeScreenshot(_screenshotsPath);
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

        private void TakeScreenshot(in string path)
        {
            _screenshotId++;
            new Screenshot(this).Save($"{path}/screenshot_{_screenshotId}", ImageFormat.Png);
        }
        
        public IEnumerable<IVideoFrame> GetBitmaps()
        {
            for (int i = 0; i < frames.Count; i++)
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
            
            video.CreateVideo(videoFramesSource);
*/
            _mainVao.Delete();
            
            foreach (var shaderProgram in _shaderPrograms)
            {
                shaderProgram.Delete();
            }
            
            base.OnClosed();
        }
    }
}