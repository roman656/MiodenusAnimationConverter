using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing.Imaging;
using System.IO;
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
        
        private int _verticeCount;
        private long _screenshotId = 0;

        private Matrix4 _model;
        private Matrix4 _view;
        private Matrix4 _projection;
        private float _FOV = 45.0f;

        private float _lastTimestamp = Stopwatch.GetTimestamp();
        private float _freq = Stopwatch.Frequency;

        private List<BitmapVideoFrameWrapper> frames = new ();

        private float _angle;
        private Scene.Scene _scene;
        private VideoRecorder video;
        private Vertex[] _vertexes;
        private Transformation[] _transformations;


        public MainWindow(Scene.Scene scene, GameWindowSettings gameWindowSettings,
            NativeWindowSettings nativeWindowSettings) : base(gameWindowSettings, nativeWindowSettings)
        {
            _screenshotsPath = "screenshots";
            _videoPath = "videos";
            CheckPath(_screenshotsPath);
            CheckPath(_videoPath);
            _scene = scene;
            video = new VideoRecorder(this,$"{_videoPath}/animation", "mp4", 60);
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

        private static Vertex[] GetAllVertexes(Scene.Scene scene)
        {
            var vertexes = new List<Vertex>();
            
            foreach (var modelGroup in scene.ModelGroups)
            {
                foreach (var model in modelGroup.Models)
                {
                    foreach (var triangle in model.Mesh.Triangles)
                    {
                        foreach (var vertex in triangle.Vertexes)
                        {
                            vertexes.Add(vertex);
                        }
                    }
                }
            }

            return vertexes.ToArray();
        }

        protected override void OnLoad()
        {
            _model = Matrix4.Identity;

            _vertexes = GetAllVertexes(_scene);

            _transformations = new Transformation[_vertexes.Length];

            for (var i = 0; i < _vertexes.Length; i++)
            {
                _transformations[i] = new Transformation(_vertexes[i].Transformation.Location, _vertexes[i].Transformation.Rotation, _vertexes[i].Transformation.Scale);
            }

            var vertexesData = new byte[(Vertex.SizeInBytes - Transformation.SizeInBytes) * _vertexes.Length];

            _verticeCount = _vertexes.Length;
            _vertexArray = GL.GenVertexArray();
            _buffer = GL.GenBuffer();

            GL.BindVertexArray(_vertexArray);
            GL.BindBuffer(BufferTarget.ArrayBuffer, _vertexArray);

            // create first buffer: vertex
            GL.NamedBufferStorage(
                _buffer,
                _vertexes.Length * Vertex.SizeInBytes,        // the size needed by this buffer
                _vertexes,                           // data to initialize with
                BufferStorageFlags.MapWriteBit);    // at this point we will only write to the buffer


            GL.VertexArrayAttribBinding(_vertexArray, 0, 0);
            GL.EnableVertexArrayAttrib(_vertexArray, 0);
            GL.VertexArrayAttribFormat(
                _vertexArray,
                0,                      // attribute index, from the shader location = 0
                3,                      // size of attribute, vec3
                VertexAttribType.Float, // contains floats
                false,                  // does not need to be normalized as it is already, floats ignore this flag anyway
                0);                     // relative offset, first item
            
            GL.VertexArrayAttribBinding(_vertexArray, 1, 0);
            GL.EnableVertexArrayAttrib(_vertexArray, 1);
            GL.VertexArrayAttribFormat(
                _vertexArray,
                1,
                3,
                VertexAttribType.Float,
                false,
                12);

            GL.VertexArrayAttribBinding(_vertexArray, 2, 0);
            GL.EnableVertexArrayAttrib(_vertexArray, 2);
            GL.VertexArrayAttribFormat(
                _vertexArray,
                2,                      // attribute index, from the shader location = 2
                4,                      // size of attribute, vec4
                VertexAttribType.Float, // contains floats
                false,                  // does not need to be normalized as it is already, floats ignore this flag anyway
                24);                     // relative offset after a vec4
            
            

            // link the vertex array and buffer and provide the stride as size of Vertex
            GL.VertexArrayVertexBuffer(_vertexArray, 0, _buffer, IntPtr.Zero, Vertex.SizeInBytes);

            CursorVisible = true;

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
                Logger.Trace("Scaled.");
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

            // Bind the VBO
            GL.BindBuffer(BufferTarget.ArrayBuffer, _buffer);
            // Bind the VAO
            GL.BindVertexArray(_vertexArray);
            
            _shaderPrograms[_currentProgramIndex].Use();

            _model = Matrix4.CreateFromAxisAngle(new Vector3(1.0f, 0.0f, 1.0f), _angle);
            _view = Matrix4.LookAt(new Vector3(0.0f, 0.0f, 5.0f), new Vector3(0.0f, 0.0f, 0.0f), Vector3.UnitY);
            _projection = Matrix4.CreatePerspectiveFieldOfView((float)Math.PI * (_FOV / 180f), Size.X / (float)Size.X, 0.2f, 256.0f);

            int location = GL.GetUniformLocation(_shaderPrograms[_currentProgramIndex].ProgramId, "model");
            GL.UniformMatrix4(location, 1, false, Matrix4ToArray(_model));
            location = GL.GetUniformLocation(_shaderPrograms[_currentProgramIndex].ProgramId, "view");
            GL.UniformMatrix4(location, 1, false, Matrix4ToArray(_view));
            location = GL.GetUniformLocation(_shaderPrograms[_currentProgramIndex].ProgramId, "projection");
            GL.UniformMatrix4(location, 1, false, Matrix4ToArray(_projection));

            // This draws the triangle.
            GL.DrawArrays(PrimitiveType.Triangles, 0, _verticeCount);
            
            GL.BindVertexArray(0);

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
            GL.DeleteVertexArray(_vertexArray);
            GL.DeleteBuffer(_buffer);
            
            foreach (var shaderProgram in _shaderPrograms)
            {
                shaderProgram.Delete();
            }
            
            base.OnClosed();
        }
    }
}