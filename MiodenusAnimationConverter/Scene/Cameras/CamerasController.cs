using System.Collections.Generic;
using MiodenusAnimationConverter.Shaders;
using MiodenusAnimationConverter.Shaders.FragmentShaders;
using MiodenusAnimationConverter.Shaders.GeometryShaders;
using MiodenusAnimationConverter.Shaders.VertexShaders;
using NLog;

namespace MiodenusAnimationConverter.Scene.Cameras
{
    public class CamerasController
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();
        private readonly Camera[] _cameras = System.Array.Empty<Camera>();
        private readonly DebugCamera[] _debugCameras = System.Array.Empty<DebugCamera>();
        private int _currentCameraIndex;
        private int _currentDebugCameraIndex;
        private VertexArrayObject _vao;
        private ShaderProgram _shaderProgram;

        public CamerasController(in List<Camera> cameras, in List<DebugCamera> debugCameras = null)
        {
            var camerasAmount = cameras?.Count ?? 0;
            var debugCamerasAmount = debugCameras?.Count ?? 0;

            if (camerasAmount <= 0 && debugCamerasAmount <= 0)
            {
                Logger.Warn("There are no cameras. Further use of this controller for its intended"
                        + " purpose is not possible.");
                return;
            }
            
            if (camerasAmount > 0)
            {
                _cameras = new Camera[camerasAmount];

                for (var i = 0; i < camerasAmount; i++)
                {
                    _cameras[i] = cameras[i];
                }
            }

            if (debugCamerasAmount > 0)
            {
                _debugCameras = new DebugCamera[debugCamerasAmount];

                for (var j = 0; j < debugCamerasAmount; j++)
                {
                    _debugCameras[j] = debugCameras[j];
                }
            }
        }
        
        public int CamerasAmount => _cameras.Length;
        public int DebugCamerasAmount => _debugCameras.Length;
        public int AllCamerasAmount => _cameras.Length + _debugCameras.Length;
        
        public Camera CurrentCamera => _cameras[_currentCameraIndex];
        public DebugCamera CurrentDebugCamera => _debugCameras[_currentDebugCameraIndex];

        public int CurrentCameraIndex
        {
            get => _currentCameraIndex;
            set
            {
                if (_cameras.Length > 0)
                {
                    if (value >= 0 && value < _cameras.Length)
                    {
                        _currentCameraIndex = value;
                    }
                    else
                    {
                        Logger.Warn("Wrong value for CurrentCameraIndex parameter. Expected: value"
                                + $" equal or greater than 0 and less than {_cameras.Length}. Got: {value}."
                                + " Current camera index was not changed.");
                    }
                }
                else
                {
                    Logger.Warn("There are no cameras of the type \"Camera\"."
                            + " Current camera index was not changed.");
                }
            }
        }
        
        public int CurrentDebugCameraIndex
        {
            get => _currentDebugCameraIndex;
            set
            {
                if (_debugCameras.Length > 0)
                {
                    if (value >= 0 && value < _debugCameras.Length)
                    {
                        _currentDebugCameraIndex = value;
                    }
                    else
                    {
                        Logger.Warn("Wrong value for CurrentDebugCameraIndex parameter. Expected: value"
                                    + $" equal or greater than 0 and less than {_debugCameras.Length}. Got: {value}."
                                    + " Current debug camera index was not changed.");
                    }
                }
                else
                {
                    Logger.Warn("There are no cameras of the type \"DebugCamera\"."
                                + " Current debug camera index was not changed.");
                }
            }
        }

        public void Initialize()
        {
            InitializeVao();
            InitializeShaderProgram();
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

            for (; i < _cameras.Length; i++, j += 3)
            {
                var camera = _cameras[i];
                fov[i] = camera.Fov;
                distanceToTheNearClipPlane[i] = camera.DistanceToTheNearClipPlane;
                distanceToTheFarClipPlane[i] = camera.DistanceToTheFarClipPlane;
                (front[j], front[j + 1], front[j + 2]) = camera.Front;
                (right[j], right[j + 1], right[j + 2]) = camera.Right;
                (up[j], up[j + 1], up[j + 2]) = camera.Up;
                (position[j], position[j + 1], position[j + 2]) = camera.Position;
            }

            for (; i < _debugCameras.Length; i++, j += 3)
            {
                var camera = _debugCameras[i];
                fov[i] = camera.Fov;
                distanceToTheNearClipPlane[i] = camera.DistanceToTheNearClipPlane;
                distanceToTheFarClipPlane[i] = camera.DistanceToTheFarClipPlane;
                (front[j], front[j + 1], front[j + 2]) = camera.Front;
                (right[j], right[j + 1], right[j + 2]) = camera.Right;
                (up[j], up[j + 1], up[j + 2]) = camera.Up;
                (position[j], position[j + 1], position[j + 2]) = camera.Position;
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

            for (var i = 0; i < shaders.Count; i++)
            {
                shaders[i].Delete();
            }

            Logger.Trace("Shader pogram initialization finished.");
        }

        public void DrawCameras(in Camera currentCamera)
        {
            _shaderProgram.SetMatrix4("view", currentCamera.ViewMatrix, false);
            _shaderProgram.SetMatrix4("projection", currentCamera.ProjectionMatrix, false);

            _vao.Draw(AllCamerasAmount);
        }

        public void Delete()
        {
            _vao.Delete();
            _shaderProgram.Delete();
        }
    }
}