using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing.Imaging;
using System.IO;
using FFMpegCore.Extend;
using FFMpegCore.Pipes;
using MiodenusAnimationConverter.Media;
using MiodenusAnimationConverter.Scene.Models.Meshes;
using MiodenusAnimationConverter.Shaders;
using MiodenusAnimationConverter.Shaders.FragmentShaders;
using MiodenusAnimationConverter.Shaders.VertexShaders;
using NLog;
using OpenTK.Graphics.OpenGL;
using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;
using OpenTK.Windowing.GraphicsLibraryFramework;

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

        private double _time;
        private int _vertexArray;
        private int _vertexArray2;
        private int _buffer;
        
        private int _vertexesAmount;
        private long _screenshotId;

        private Matrix4 _model;
        private Matrix4 _view;
        private Matrix4 _projection;
        private float _FOV = 45.0f;

        private float _lastTimestamp = Stopwatch.GetTimestamp();
        private float _freq = Stopwatch.Frequency;

        private List<BitmapVideoFrameWrapper> frames = new ();
        private bool _isCursorVisible = true;
        private float _angle;
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
        
        private float[] _vertexesLocation;
        private float[] _vertexesRotation;
        private float[] _vertexesScale;
        private float _rotationRate = 1.0f;
        private Vector3 _lightPosition = new (0, 10, 10);

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
            
            var shaders = new List<Shader>
            {
                new (TransformShader.Code, TransformShader.Type),
                new (LightingShader.Code, LightingShader.Type)
            };

            _shaderPrograms.Add(new ShaderProgram(shaders));

            foreach (var shader in shaders)
            {
                shader.Delete();
            }
            
            Logger.Trace("Shader pograms initialization finished.");
        }

        protected override void OnLoad()
        {
            _model = Matrix4.Identity;
            
            _scene.ModelGroups[0].Scale(0.02f, 0.02f, 0.02f);

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
            
            CursorVisible = _isCursorVisible;
            _vertexesAmount = _vertexes.Length;

            InitializeShaderPrograms();
            
            GL.Enable(EnableCap.DepthTest);
            GL.PolygonMode(MaterialFace.Front, PolygonMode.Fill);
            GL.PatchParameter(PatchParameterInt.PatchVertices, 3);
        }
        
        private float[] Matrix4ToArray(Matrix4 matrix)
        {
            var data = new float[16];

            for (byte i = 0; i < 4; i++)
            {
                for (byte j = 0; j < 4; j++)
                {
                    data[i * 4 + j] = matrix[i, j];
                }
            }
            
            return data;
        }

        protected override void OnKeyDown(KeyboardKeyEventArgs e)
        {
            switch (e.Key)
            {
                case Keys.W:
                {
                    for (var i = 0; i < _vertexesAmount; i++)
                    {
                        _vertexes[i].Move(0, -25, 0);
                    }
                    
                    _hasTransformed = true;
                    break;
                }
                case Keys.S:
                {
                    for (var i = 0; i < _vertexesAmount; i++)
                    {
                        _vertexes[i].Move(0, 25, 0);
                    }
                    
                    _hasTransformed = true;
                    break;
                }
                case Keys.A:
                {
                    for (var i = 0; i < _vertexesAmount; i++)
                    {
                        _vertexes[i].Move(-25, 0, 0);
                    }
                    
                    _hasTransformed = true;
                    break;
                }
                case Keys.D:
                {
                    for (var i = 0; i < _vertexesAmount; i++)
                    {
                        _vertexes[i].Move(25, 0, 0);
                    }

                    _hasTransformed = true;
                    break;
                }
                case Keys.E:
                {
                    for (var i = 0; i < _vertexesAmount; i++)
                    {
                        _vertexes[i].Rotate((float)Math.PI / 6, new Vector3(1, 0, 0));
                    }

                    _hasTransformed = true;
                    break;
                }
                case Keys.R:
                {
                    for (var i = 0; i < _vertexesAmount; i++)
                    {
                        _vertexes[i].Rotate((float)Math.PI / 6, new Vector3(0, 1, 0));
                    }

                    _hasTransformed = true;
                    break;
                }
                case Keys.T:
                {
                    for (var i = 0; i < _vertexesAmount; i++)
                    {
                        _vertexes[i].Rotate((float)Math.PI / 6, new Vector3(0, 0, 1));
                    }

                    _hasTransformed = true;
                    break;
                }
                case Keys.Q:
                {
                    _lightPosition.X = (float)(1.0 + Math.Sin(Stopwatch.GetTimestamp()) * 2.0);
                    //_lightPosition.Y = (float)(Math.Sin(Stopwatch.GetTimestamp() / 2.0) * 1.0f);
                    
                    break;
                }
            }

            base.OnKeyDown(e);
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

        protected override void OnRenderFrame(FrameEventArgs e)
        {
            GL.ClearColor(_backgroundColor);
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

            UpdateModelsTransformation();

            var timeStamp = Stopwatch.GetTimestamp();
            //_angle += (float)((timeStamp - _lastTimestamp) / (double)_freq) * _rotationRate;
            _lastTimestamp = timeStamp;

            _model = Matrix4.CreateFromAxisAngle(new Vector3(1.0f, 0.0f, 1.0f), _angle);
            _view = Matrix4.LookAt(new Vector3(0.0f, 0.0f, 5.0f), new Vector3(0.0f, 0.0f, 0.0f), Vector3.UnitY);
            _projection = Matrix4.CreatePerspectiveFieldOfView((float)Math.PI * (_FOV / 180f), Size.X / (float)Size.X, 0.2f, 256.0f);
            
            var lightPositionLocation = GL.GetUniformLocation(_shaderPrograms[_currentProgramIndex].ProgramId, "light_position");
            GL.Uniform3(lightPositionLocation, _lightPosition.X, _lightPosition.Y, _lightPosition.Z);

            var location = GL.GetUniformLocation(_shaderPrograms[_currentProgramIndex].ProgramId, "model");
            GL.UniformMatrix4(location, 1, false, Matrix4ToArray(_model));
            location = GL.GetUniformLocation(_shaderPrograms[_currentProgramIndex].ProgramId, "view");
            GL.UniformMatrix4(location, 1, false, Matrix4ToArray(_view));
            location = GL.GetUniformLocation(_shaderPrograms[_currentProgramIndex].ProgramId, "projection");
            GL.UniformMatrix4(location, 1, false, Matrix4ToArray(_projection));
            
            _shaderPrograms[_currentProgramIndex].Use();
            _mainVao.Draw(_vertexesAmount, _drawMode);

            Context.SwapBuffers();
            base.OnRenderFrame(e);

            //frames.Add(video.CreateVideoFrame());
            //TakeScreenshot(_screenshotsPath);
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