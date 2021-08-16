using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing.Imaging;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using FFMpegCore.Extend;
using FFMpegCore.Pipes;
using MiodenusAnimationConverter.Media;
using MiodenusAnimationConverter.Scene.Models;
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
        private long _screenshotId = 0;

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
            _vertexes = _scene.Vertexes;
            _transformations = new Transformation[_vertexes.Length];
            
            var vertexesPositions = new float[_vertexes.Length * 3];
            var vertexesNormals = new float[_vertexes.Length * 3];
            var vertexesColors = new float[_vertexes.Length * 4];

            for (int i = 0, j = 0, k = 0; i < _vertexes.Length; i += 1, j += 3, k += 4)
            {
                _transformations[i] = new Transformation(_vertexes[i].Transformation.Location,
                                                         _vertexes[i].Transformation.Rotation,
                                                         _vertexes[i].Transformation.Scale);
                
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
            if (e.Key == Keys.W)
            {
                _scene.ModelGroups[0].Models[0].Move(1, 0, 0);
                _scene.ModelGroups[0].Models[0].Scale(0.01f, 0.01f, 0.01f);
            }
            base.OnKeyDown(e);
        }

        protected override void OnRenderFrame(FrameEventArgs e)
        {
            var timeStamp = Stopwatch.GetTimestamp();
            _angle += (float)((timeStamp - _lastTimestamp) / (double)_freq);
            _lastTimestamp = timeStamp;

            GL.ClearColor(_backgroundColor);
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

            _shaderPrograms[_currentProgramIndex].Use();

            _model = Matrix4.CreateFromAxisAngle(new Vector3(1.0f, 0.0f, 1.0f), _angle);
            _view = Matrix4.LookAt(new Vector3(0.0f, 0.0f, 5.0f), new Vector3(0.0f, 0.0f, 0.0f), Vector3.UnitY);
            _projection = Matrix4.CreatePerspectiveFieldOfView((float)Math.PI * (_FOV / 180f), Size.X / (float)Size.X, 0.2f, 256.0f);

            var location = GL.GetUniformLocation(_shaderPrograms[_currentProgramIndex].ProgramId, "model");
            GL.UniformMatrix4(location, 1, false, Matrix4ToArray(_model));
            location = GL.GetUniformLocation(_shaderPrograms[_currentProgramIndex].ProgramId, "view");
            GL.UniformMatrix4(location, 1, false, Matrix4ToArray(_view));
            location = GL.GetUniformLocation(_shaderPrograms[_currentProgramIndex].ProgramId, "projection");
            GL.UniformMatrix4(location, 1, false, Matrix4ToArray(_projection));
            
            _mainVao.Draw(_vertexesAmount);

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