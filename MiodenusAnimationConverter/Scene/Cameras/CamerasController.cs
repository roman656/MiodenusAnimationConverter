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
        private readonly List<Camera> _cameras = new ();
        private readonly int _camerasAmount;
        private readonly int _debugCamerasAmount;
        private int _currentCameraIndex;
        private int _currentDebugCameraIndex;
        private VertexArrayObject _vao;
        private ShaderProgram _shaderProgram;

        public CamerasController(List<Camera> cameras, List<DebugCamera> debugCameras = null)
        {
            _camerasAmount = cameras?.Count ?? 0;
            _debugCamerasAmount = debugCameras?.Count ?? 0;
            _currentCameraIndex = _camerasAmount > 0 ? 0 : -1;
            _currentDebugCameraIndex = _debugCamerasAmount > 0 ? _camerasAmount : -1;
            
            if (cameras != null)
            {
                _cameras.AddRange(cameras);
            }

            if (debugCameras != null)
            {
                _cameras.AddRange(debugCameras);
            }
            
            if (_cameras.Count <= 0)
            {
                Logger.Warn("There are no cameras. Further use of this controller for its intended"
                        + " purpose is not possible.");
            }
        }
        
        public int CamerasAmount => _camerasAmount;
        public int DebugCamerasAmount => _debugCamerasAmount;
        public int AllCamerasAmount => _cameras.Count;
        public Camera CurrentCamera => _camerasAmount > 0 ? _cameras[_currentCameraIndex] : null;
        
        public DebugCamera CurrentDebugCamera
        {
            get => _debugCamerasAmount > 0 ? _cameras[_currentDebugCameraIndex] as DebugCamera : null;
        }
        
        public int CurrentCameraIndex
        {
            get => _currentCameraIndex;
            set
            {
                if (_camerasAmount > 0)
                {
                    if (value >= 0 && value < _camerasAmount)
                    {
                        _currentCameraIndex = value;
                    }
                    else
                    {
                        Logger.Warn("Wrong value for CurrentCameraIndex parameter. Expected: value"
                                + $" equal or greater than 0 and less than {_camerasAmount}. Got: {value}."
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
            get => _debugCamerasAmount > 0 ? _currentDebugCameraIndex - _camerasAmount : -1;
            set
            {
                if (_debugCamerasAmount > 0)
                {
                    if (value >= 0 && value < _debugCamerasAmount)
                    {
                        _currentDebugCameraIndex = value + _camerasAmount;
                    }
                    else
                    {
                        Logger.Warn("Wrong value for CurrentDebugCameraIndex parameter. Expected: value"
                                + $" equal or greater than 0 and less than {_debugCamerasAmount}. Got: {value}."
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
            var fov = new float[AllCamerasAmount];
            var distanceToTheNearClipPlane = new float[AllCamerasAmount];
            var distanceToTheFarClipPlane = new float[AllCamerasAmount];
            var viewDirection = new float[AllCamerasAmount * 3];
            var rightDirection = new float[AllCamerasAmount * 3];
            var upDirection = new float[AllCamerasAmount * 3];
            var position = new float[AllCamerasAmount * 3];
            int i = 0, j = 0;

            foreach (var camera in _cameras)
            {
                fov[i] = camera.Fov;
                distanceToTheNearClipPlane[i] = camera.DistanceToTheNearClipPlane;
                distanceToTheFarClipPlane[i] = camera.DistanceToTheFarClipPlane;
                (viewDirection[j], viewDirection[j + 1], viewDirection[j + 2]) = camera.ViewDirection;
                (rightDirection[j], rightDirection[j + 1], rightDirection[j + 2]) = camera.RightDirection;
                (upDirection[j], upDirection[j + 1], upDirection[j + 2]) = camera.UpDirection;
                (position[j], position[j + 1], position[j + 2]) = camera.Position;
                i++;
                j += 3;
            }

            _vao = new VertexArrayObject();
            _vao.AddVertexBufferObject(fov, 1);
            _vao.AddVertexBufferObject(distanceToTheNearClipPlane, 1);
            _vao.AddVertexBufferObject(distanceToTheFarClipPlane, 1);
            _vao.AddVertexBufferObject(viewDirection, 3);
            _vao.AddVertexBufferObject(rightDirection, 3);
            _vao.AddVertexBufferObject(upDirection, 3);
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

        public void DrawCameras(Camera currentCamera)
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