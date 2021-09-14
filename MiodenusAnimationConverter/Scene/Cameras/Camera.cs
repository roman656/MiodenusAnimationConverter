using NLog;
using OpenTK.Mathematics;

namespace MiodenusAnimationConverter.Scene.Cameras
{
    /// <summary>
    /// Камера общего назначения.
    /// </summary>
    public class Camera : IMovable, IRotatable
    {
        private const float FovMinValue = 1.0f;
        private const float FovMaxValue = 180.0f;
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();
        private int _viewportWidth = 1;
        private int _viewportHeight = 1;
        private float _viewportAspectRatio;
        private float _fov = MathHelper.PiOver3;    // Угол поля зрения в направлении оси OY (в радианах).
        private float _distanceToTheNearClipPlane = 0.01f;
        private float _distanceToTheFarClipPlane = 100.0f;
        private Vector3 _front = -Vector3.UnitZ;
        private Vector3 _up = Vector3.UnitY;
        private Vector3 _right = Vector3.UnitX;
        private Matrix4 _view;
        private Matrix4 _projection;
        public Vector3 Position;

        public Camera(Vector3 position, int viewportWidth, int viewportHeight)
        {
            Position = position;
            ViewportWidth = viewportWidth;
            ViewportHeight = viewportHeight;
            _view = Matrix4.LookAt(Position, Position + _front, _up);
            _projection = Matrix4.CreatePerspectiveFieldOfView(_fov, _viewportAspectRatio,
                    _distanceToTheNearClipPlane, _distanceToTheFarClipPlane);
        }

        /// <summary>Метод, устанавливающий параметры камеры в значения по умолчанию.</summary>
        /// <remarks>положение камеры и размеры окна остаются прежними.</remarks>
        public void Reset()
        {
            _front = -Vector3.UnitZ;
            _up = Vector3.UnitY;
            _right = Vector3.UnitX;
            _fov = MathHelper.PiOver3;
            _distanceToTheNearClipPlane = 0.01f;
            _distanceToTheFarClipPlane = 100.0f;
            _view = Matrix4.LookAt(Position, Position + _front, _up);
            _projection = Matrix4.CreatePerspectiveFieldOfView(_fov, _viewportAspectRatio, 
                    _distanceToTheNearClipPlane, _distanceToTheFarClipPlane);
        }
        
        public int ViewportWidth
        {
            get => _viewportWidth;
            set
            {
                if (value > 0)
                {
                    _viewportWidth = value;
                    _viewportAspectRatio = _viewportWidth / (float)_viewportHeight;
                    _projection = Matrix4.CreatePerspectiveFieldOfView(_fov, _viewportAspectRatio,
                            _distanceToTheNearClipPlane, _distanceToTheFarClipPlane);
                }
                else
                {
                    Logger.Warn("Wrong value for ViewportWidth parameter. Expected: value greater than 0. Got:"
                            + $" {value}. Viewport width was not changed.");
                }
            }
        }
        
        public int ViewportHeight
        {
            get => _viewportHeight;
            set
            {
                if (value > 0)
                {
                    _viewportHeight = value;
                    _viewportAspectRatio = _viewportWidth / (float)_viewportHeight;
                    _projection = Matrix4.CreatePerspectiveFieldOfView(_fov, _viewportAspectRatio,
                            _distanceToTheNearClipPlane, _distanceToTheFarClipPlane);
                }
                else
                {
                    Logger.Warn("Wrong value for ViewportHeight parameter. Expected: value greater than 0. Got:"
                                + $" {value}. Viewport height was not changed.");
                }
            }
        }

        public float Fov
        {
            get => MathHelper.RadiansToDegrees(_fov);
            set
            {
                if ((value >= FovMinValue) && (value <= FovMaxValue))
                {
                    _fov = MathHelper.DegreesToRadians(value);
                    _projection = Matrix4.CreatePerspectiveFieldOfView(_fov, _viewportAspectRatio,
                            _distanceToTheNearClipPlane, _distanceToTheFarClipPlane);
                }
                else
                {
                    Logger.Warn("Wrong value for Fov parameter. Expected: minimum value"
                            + $" is {FovMinValue}; maximum value is {FovMaxValue}. Got: {value}. FOV was not changed.");
                }
            }
        }

        public float DistanceToTheNearClipPlane
        {
            get => _distanceToTheNearClipPlane;
            set
            {
                if ((value > 0.0f) && (value < _distanceToTheFarClipPlane))
                {
                    _distanceToTheNearClipPlane = value;
                    _projection = Matrix4.CreatePerspectiveFieldOfView(_fov, _viewportAspectRatio,
                            _distanceToTheNearClipPlane, _distanceToTheFarClipPlane);
                }
                else
                {
                    Logger.Warn("Wrong value for DistanceToTheNearClipPlane parameter. Expected: value"
                            + $" greater than 0 and less than {_distanceToTheFarClipPlane} (distance to the far"
                            + $" clip plane). Got: {value}. Distance to the near clip plane was not changed.");
                }
            }
        }
        
        public float DistanceToTheFarClipPlane
        {
            get => _distanceToTheFarClipPlane;
            set
            {
                if (value > _distanceToTheNearClipPlane)
                {
                    _distanceToTheFarClipPlane = value;
                    _projection = Matrix4.CreatePerspectiveFieldOfView(_fov, _viewportAspectRatio,
                            _distanceToTheNearClipPlane, _distanceToTheFarClipPlane);
                }
                else
                {
                    Logger.Warn("Wrong value for DistanceToTheFarClipPlane parameter. Expected: value"
                            + $" greater than {_distanceToTheNearClipPlane} (distance to the near"
                            + $" clip plane). Got: {value}. Distance to the far clip plane was not changed.");
                }
            }
        }
        
        public Matrix4 ViewMatrix => _view;
        public Matrix4 ProjectionMatrix => _projection;
        public Vector3 Front => _front;
        public Vector3 Right => _right;
        public Vector3 Up => _up;
        
        public void LookAt(Vector3 target)
        {
            var rotationAxis = Vector3.Normalize(target - Position) + _front;
            RotateViewDirection(MathHelper.Pi, rotationAxis);
            RotateViewDirection(MathHelper.Pi, _front);
        }

        public void Move(float deltaX, float deltaY, float deltaZ)
        {
            Position.X += deltaX;
            Position.Y += deltaY;
            Position.Z += deltaZ;
            _view = Matrix4.LookAt(Position, Position + _front, _up);
        }

        public void MoveInViewDirection(float delta)
        {
            Position.X += _front.X * delta;
            Position.Y += _front.Y * delta;
            Position.Z += _front.Z * delta;
            _view = Matrix4.LookAt(Position, Position + _front, _up);
        }
        
        public void MoveInRightDirection(float delta)
        {
            Position.X += _right.X * delta;
            Position.Y += _right.Y * delta;
            Position.Z += _right.Z * delta;
            _view = Matrix4.LookAt(Position, Position + _front, _up);
        }
        
        public void MoveInUpDirection(float delta)
        {
            Position.X += _up.X * delta;
            Position.Y += _up.Y * delta;
            Position.Z += _up.Z * delta;
            _view = Matrix4.LookAt(Position, Position + _front, _up);
        }

        public void Rotate(float angle, Vector3 vector)
        {
            Position = Quaternion.FromAxisAngle(vector, angle) * Position;
            _view = Matrix4.LookAt(Position, Position + _front, _up);
        }
        
        public void RotateViewDirection(float angle, Vector3 vector)
        {
            var rotation = Quaternion.FromAxisAngle(vector, angle);

            _front = Vector3.Normalize(rotation * _front);
            _right = Vector3.Normalize(rotation * _right);
            _up = Vector3.Normalize(Vector3.Cross(_right, _front));
            _view = Matrix4.LookAt(Position, Position + _front, _up);
        }
    }
}