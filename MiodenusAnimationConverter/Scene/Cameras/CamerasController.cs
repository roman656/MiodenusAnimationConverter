using System.Collections.Generic;
using MiodenusAnimationConverter.Shaders;
using MiodenusAnimationConverter.Shaders.FragmentShaders;
using MiodenusAnimationConverter.Shaders.GeometryShaders;
using MiodenusAnimationConverter.Shaders.VertexShaders;
using NLog;
using OpenTK.Graphics.OpenGL;
using OpenTK.Mathematics;

namespace MiodenusAnimationConverter.Scene.Cameras
{
    public class CamerasController
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();
        private readonly Camera[] _cameras;
        private readonly DebugCamera[] _debugCameras;
        private VertexArrayObject _vao;
        private ShaderProgram _shaderProgram;

        public CamerasController(List<Camera> cameras, List<DebugCamera> debugCameras = null)
        {
            if (cameras.Count > 0)
            {
                var i = 0;
                _cameras = new Camera[cameras.Count];

                foreach (var camera in cameras)
                {
                    _cameras[i] = camera;
                    i++;
                }
            }
            
            if (debugCameras != null && debugCameras.Count > 0)
            {
                var j = 0;
                _debugCameras = new DebugCamera[debugCameras.Count];

                foreach (var camera in debugCameras)
                {
                    _debugCameras[j] = camera;
                    j++;
                }
            }
        }

        public Camera[] Cameras => _cameras;
        public DebugCamera[] DebugCameras => _debugCameras;

        public int CamerasAmount => _cameras.Length;
        public int DebugCamerasAmount => _debugCameras.Length;
        public int AllCamerasAmount => _cameras.Length + _debugCameras.Length;

        public void Initialize()
        {
            InitializeShaderProgram();
            InitializeVao();
        }

        private void InitializeVao()
        {
            var allCamerasAmount = AllCamerasAmount;
            var fov = new float[allCamerasAmount];
            var distanceToTheNearClipPlane = new float[allCamerasAmount];
            var distanceToTheFarClipPlane = new float[allCamerasAmount];
            var front = new float[allCamerasAmount * 3];
            var right = new float[allCamerasAmount * 3];
            var up = new float[allCamerasAmount * 3];
            var position = new float[allCamerasAmount * 3];
            var i = 0;
            var j = 0;
            
            foreach (var camera in _cameras)
            {
                fov[i] = camera.Fov;
                distanceToTheNearClipPlane[i] = camera.DistanceToTheNearClipPlane;
                distanceToTheFarClipPlane[i] = camera.DistanceToTheFarClipPlane;
                (front[j], front[j + 1], front[j + 2]) = camera.Front;
                (right[j], right[j + 1], right[j + 2]) = camera.Right;
                (up[j], up[j + 1], up[j + 2]) = camera.Up;
                (position[j], position[j + 1], position[j + 2]) = camera.Position;
                i++;
                j += 3;
            }

            foreach (var camera in _debugCameras)
            {
                fov[i] = camera.Fov;
                distanceToTheNearClipPlane[i] = camera.DistanceToTheNearClipPlane;
                distanceToTheFarClipPlane[i] = camera.DistanceToTheFarClipPlane;
                (front[j], front[j + 1], front[j + 2]) = camera.Front;
                (right[j], right[j + 1], right[j + 2]) = camera.Right;
                (up[j], up[j + 1], up[j + 2]) = camera.Up;
                (position[j], position[j + 1], position[j + 2]) = camera.Position;
                i++;
                j += 3;
            }

            _vao = new VertexArrayObject();
            _vao.AddVertexBufferObject(fov, 1);
            _vao.AddVertexBufferObject(distanceToTheNearClipPlane, 1);
            _vao.AddVertexBufferObject(distanceToTheFarClipPlane, 1);
            _vao.AddVertexBufferObject(front, 3);
            _vao.AddVertexBufferObject(right, 3);
            _vao.AddVertexBufferObject(up, 3);
            _vao.AddVertexBufferObject(position, 3);
        }

        private void InitializeShaderProgram()
        {
            Logger.Trace("Shader pogram initialization started.");
            
            var shaders = new List<Shader>
            {
                new (CamerasVertexShader.Code, CamerasVertexShader.Type),
                new (CamerasGeometryShader.Code, CamerasGeometryShader.Type),
                new (CamerasFragmentShader.Code, CamerasFragmentShader.Type)
            };

            _shaderProgram = new ShaderProgram(shaders);

            foreach (var shader in shaders)
            {
                shader.Delete();
            }
            
            Logger.Trace("Shader pogram initialization finished.");
        }

        public void DrawCameras(Matrix4 view, Matrix4 projection)
        {
            _shaderProgram.SetMatrix4("view", view, false);
            _shaderProgram.SetMatrix4("projection", projection, false);

            _vao.Draw(AllCamerasAmount, PrimitiveType.Lines);
        }
    }
}