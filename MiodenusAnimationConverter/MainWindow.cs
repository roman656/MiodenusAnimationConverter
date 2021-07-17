using System;
using System.Collections.Generic;
using System.Diagnostics;
using MiodenusAnimationConverter.Scene.Models;
using MiodenusAnimationConverter.Scene.Models.Meshes;
using MiodenusAnimationConverter.Shaders;
using MiodenusAnimationConverter.Shaders.FragmentShaders;
using MiodenusAnimationConverter.Shaders.VertexShaders;
using OpenTK.Graphics.OpenGL;
using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;

namespace MiodenusAnimationConverter
{
    public class MainWindow : GameWindow
    {
        private List<ShaderProgram> _shaderPrograms = new ();
        private int _currentProgramIndex = 0;
        private double _time;
        private bool _initialized;
        private int _vertexArray;
        private int _buffer;
        private int _verticeCount;
        private long _screenshotId = 0;

        private Matrix4 _model;
        private Matrix4 _view;
        private Matrix4 _projection;
        private float _FOV = 45.0f;

        private float _lastTimestamp = Stopwatch.GetTimestamp();
        private float _freq = Stopwatch.Frequency;

        private float _angle;
        private Model[] _models;

        public MainWindow(Model[] models, GameWindowSettings gameWindowSettings,
            NativeWindowSettings nativeWindowSettings) : base(gameWindowSettings, nativeWindowSettings)
        {
            _models = models;
        }

        private void InitializeShaderPrograms()
        {
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
        }

        protected override void OnLoad()
        {
            _model = Matrix4.Identity;
            int vertexesAmount = 0;
            
            foreach (var model in _models)
            {
                vertexesAmount += model.Triangles.Length * Triangle.VertexesAmount;
            }

            Vertex[] vertexes = new Vertex[vertexesAmount];

            int i = 0;
            
            foreach (var model in _models)
            {
                foreach (var triangle in model.Triangles)
                {
                    vertexes[i] = triangle.Vertexes[0];
                    vertexes[i + 1] = triangle.Vertexes[1];
                    vertexes[i + 2] = triangle.Vertexes[2];
                    i += Triangle.VertexesAmount;
                }
            }

            _verticeCount = vertexes.Length;
            _vertexArray = GL.GenVertexArray();
            _buffer = GL.GenBuffer();

            GL.BindVertexArray(_vertexArray);
            GL.BindBuffer(BufferTarget.ArrayBuffer, _vertexArray);

            // create first buffer: vertex
            GL.NamedBufferStorage(
                _buffer,
                Vertex.SizeInBytes * vertexes.Length,        // the size needed by this buffer
                vertexes,                           // data to initialize with
                BufferStorageFlags.MapWriteBit);    // at this point we will only write to the buffer


            GL.VertexArrayAttribBinding(_vertexArray, 0, 0);
            GL.EnableVertexArrayAttrib(_vertexArray, 0);
            GL.VertexArrayAttribFormat(
                _vertexArray,
                0,                      // attribute index, from the shader location = 0
                4,                      // size of attribute, vec4
                VertexAttribType.Float, // contains floats
                false,                  // does not need to be normalized as it is already, floats ignore this flag anyway
                0);                     // relative offset, first item


            GL.VertexArrayAttribBinding(_vertexArray, 1, 0);
            GL.EnableVertexArrayAttrib(_vertexArray, 1);
            GL.VertexArrayAttribFormat(
                _vertexArray,
                1,                      // attribute index, from the shader location = 1
                4,                      // size of attribute, vec4
                VertexAttribType.Float, // contains floats
                false,                  // does not need to be normalized as it is already, floats ignore this flag anyway
                16);                     // relative offset after a vec4

            // link the vertex array and buffer and provide the stride as size of Vertex
            GL.VertexArrayVertexBuffer(_vertexArray, 0, _buffer, IntPtr.Zero, Vertex.SizeInBytes);
            _initialized = true;

            CursorVisible = true;

            InitializeShaderPrograms();
            
            GL.Enable(EnableCap.DepthTest);
            GL.PolygonMode(MaterialFace.Front, PolygonMode.Fill);
            GL.PatchParameter(PatchParameterInt.PatchVertices, 3);
        }
        
        private float[] Matrix4ToArray(Matrix4 matrix)
        {
            float[] data = new float[16];
            for (int i = 0; i < 4; i++)
            {
                for (int j = 0; j < 4; j++)
                {
                    data[i * 4 + j] = matrix[i, j];

                }
            }
            return data;
        }

        protected override void OnRenderFrame(FrameEventArgs e)
        {
            var timeStamp = Stopwatch.GetTimestamp();
            _angle += (float)((timeStamp - _lastTimestamp) / (double)_freq);
            _lastTimestamp = timeStamp;

            GL.ClearColor(0.0f, 0.0f, 0.0f, 1.0f);
            // Clear the color buffer.
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

            Context.SwapBuffers();
            base.OnRenderFrame(e);
            /*
            _screenshotId++;
            var tempScreenshot = new Screenshot((ushort)Size.X, (ushort)Size.Y);
            tempScreenshot.SaveToPng($"screenshot_{_screenshotId}");
            */
        }

        protected override void OnClosed()
        {
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