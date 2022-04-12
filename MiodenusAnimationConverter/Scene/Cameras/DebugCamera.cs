using System.Linq;
using NLog;
using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.GraphicsLibraryFramework;

namespace MiodenusAnimationConverter.Scene.Cameras
{
    public class DebugCamera : Camera
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();
        private float _movementSpeed = 2.0f;
        private float _movementSpeedDelta = 0.1f;
        private bool _isFirstMouseMove = true;
        private Vector2 _previousMousePosition;
        private float _maxRotationSpeed = 0.1f;
        private float _scrollSensitivity = 2.0f;
        private float _mouseSensitivity = 0.004f;
        public bool UseLocalCoordinateSystemForMovement = true;

        public DebugCamera(Vector3 position, int viewportWidth = 1, int viewportHeight = 1)
                : base(position, viewportWidth, viewportHeight) {}
        
        public DebugCamera(int viewportWidth = 1, int viewportHeight = 1)
                : base(viewportWidth, viewportHeight) {}

        public void SwitchCoordinateSystemForMovement()
        {
            UseLocalCoordinateSystemForMovement = !UseLocalCoordinateSystemForMovement;
        }
        
        public float MouseSensitivity
        {
            get => _mouseSensitivity;
            set
            {
                if (value > 0.0f)
                {
                    _mouseSensitivity = value;
                }
                else
                {
                    Logger.Warn("Wrong value for MouseSensitivity parameter. Expected: value greater than"
                                + $" 0. Got: {value}. Mouse sensitivity was not changed.");
                }
            }
        }
        
        public float ScrollSensitivity
        {
            get => _scrollSensitivity;
            set
            {
                if (value > 0.0f)
                {
                    _scrollSensitivity = value;
                }
                else
                {
                    Logger.Warn("Wrong value for ScrollSensitivity parameter. Expected: value greater than"
                                + $" 0. Got: {value}. Scroll sensitivity was not changed.");
                }
            }
        }
        
        public float MaxRotationSpeed
        {
            get => _maxRotationSpeed;
            set
            {
                if (value > 0.0f)
                {
                    _maxRotationSpeed = value;
                }
                else
                {
                    Logger.Warn("Wrong value for MaxRotationSpeed parameter. Expected: value greater than"
                            + $" 0. Got: {value}. Max rotation speed was not changed.");
                }
            }
        }

        public float MovementSpeed
        {
            get => _movementSpeed;
            set
            {
                if (value > _movementSpeedDelta)
                {
                    _movementSpeed = value;
                }
                else
                {
                    Logger.Warn("Wrong value for MovementSpeed parameter. Expected: value greater than"
                            + $" {_movementSpeedDelta} (movement speed delta). Got:"
                            + $" {value}. Movement speed was not changed.");
                }
            }
        }
        
        public float MovementSpeedDelta
        {
            get => _movementSpeedDelta;
            set
            {
                if (value > 0.0f)
                {
                    _movementSpeedDelta = value;
                }
                else
                {
                    Logger.Warn("Wrong value for MovementSpeedDelta parameter. Expected: value greater than"
                            + $" 0. Got: {value}. Movement speed delta was not changed.");
                }
            }
        }
        
        public void ProcessKeyboard(KeyboardState keyboardState, double deltaTime)
        {
            var velocity = (float)(_movementSpeed * deltaTime);
            
            if (keyboardState.IsKeyDown(DebugCameraKeyBindings.ForwardMoveKey)
                && !keyboardState.IsKeyDown(DebugCameraKeyBindings.BackMoveKey))
            {
                if (UseLocalCoordinateSystemForMovement)
                {
                    MoveInViewDirection(velocity);
                }
                else
                {
                    GlobalMove(deltaZ: -velocity);
                }
            }
            
            if (keyboardState.IsKeyDown(DebugCameraKeyBindings.LeftMoveKey)
                    && !keyboardState.IsKeyDown(DebugCameraKeyBindings.RightMoveKey))
            {
                if (UseLocalCoordinateSystemForMovement)
                {
                    MoveInRightDirection(-velocity);
                }
                else
                {
                    GlobalMove(-velocity);
                }
            }
            
            if (keyboardState.IsKeyDown(DebugCameraKeyBindings.BackMoveKey)
                    && !keyboardState.IsKeyDown(DebugCameraKeyBindings.ForwardMoveKey))
            {
                if (UseLocalCoordinateSystemForMovement)
                {
                    MoveInViewDirection(-velocity);
                }
                else
                {
                    GlobalMove(deltaZ: velocity);
                }
            }
            
            if (keyboardState.IsKeyDown(DebugCameraKeyBindings.RightMoveKey)
                    && !keyboardState.IsKeyDown(DebugCameraKeyBindings.LeftMoveKey))
            {
                if (UseLocalCoordinateSystemForMovement)
                {
                    MoveInRightDirection(velocity);
                }
                else
                {
                    GlobalMove(velocity);
                }
            }
            
            if (keyboardState.IsKeyDown(DebugCameraKeyBindings.MoveUpKey)
                    && !keyboardState.IsKeyDown(DebugCameraKeyBindings.MoveDownKey))
            {
                if (UseLocalCoordinateSystemForMovement)
                {
                    MoveInUpDirection(velocity);
                }
                else
                {
                    GlobalMove(deltaY: velocity);
                }
            }
            
            if (keyboardState.IsKeyDown(DebugCameraKeyBindings.MoveDownKey)
                    && !keyboardState.IsKeyDown(DebugCameraKeyBindings.MoveUpKey))
            {
                if (UseLocalCoordinateSystemForMovement)
                {
                    MoveInUpDirection(-velocity);
                }
                else
                {
                    GlobalMove(deltaY: -velocity);
                }
            }
            
            if (keyboardState.IsKeyDown(DebugCameraKeyBindings.LeftTiltKey)
                    && !keyboardState.IsKeyDown(DebugCameraKeyBindings.RightTiltKey))
            {
                if (UseLocalCoordinateSystemForMovement)
                {
                    RotateViewDirection(-velocity, ViewDirection);
                }
                else
                {
                    RotateViewDirection(-velocity, -Vector3.UnitZ);
                }
            }

            if (keyboardState.IsKeyDown(DebugCameraKeyBindings.RightTiltKey)
                    && !keyboardState.IsKeyDown(DebugCameraKeyBindings.LeftTiltKey))
            {
                if (UseLocalCoordinateSystemForMovement)
                {
                    RotateViewDirection(velocity, ViewDirection);
                }
                else
                {
                    RotateViewDirection(velocity, -Vector3.UnitZ);
                }
            }
            
            if (keyboardState.IsKeyDown(DebugCameraKeyBindings.ResetPositionKey))
            {
                Position = Scene.DefaultCameraPosition;
            }
            
            if (keyboardState.IsKeyDown(DebugCameraKeyBindings.LookAtZeroKey))
            {
                LookAt(Vector3.Zero);
            }
            
            if (keyboardState.IsKeyDown(DebugCameraKeyBindings.SwitchCoordinateSystemForMovementKey))
            {
                SwitchCoordinateSystemForMovement();
            }

            if (DebugCameraKeyBindings.ResetKeys.All(keyboardState.IsKeyDown))
            {
                Reset();
                MovementSpeed = 2.0f;
            }
        }
        
        public void ProcessMouseMovement(MouseState mouseState)
        {
            if (_isFirstMouseMove)
            {
                _previousMousePosition = new Vector2(mouseState.X, mouseState.Y);
                _isFirstMouseMove = false;
            }
            else
            {
                var (x, y) = (mouseState.Position - _previousMousePosition) * -_mouseSensitivity;

                x = MathHelper.Clamp(x, -_maxRotationSpeed, _maxRotationSpeed);
                y = MathHelper.Clamp(y, -_maxRotationSpeed, _maxRotationSpeed);
                _previousMousePosition = mouseState.Position;
                RotateViewDirection(x, UpDirection);
                RotateViewDirection(y, RightDirection);
            }
        }
        
        public void ProcessMouseScroll(MouseWheelEventArgs args, KeyboardState keyboardState)
        {
            if (keyboardState.IsKeyDown(DebugCameraKeyBindings.FovChangeKey))
            {
                Fov -= args.OffsetY * _scrollSensitivity;
            }
            else
            {
                if (args.OffsetY > 0.0f)
                {
                    MovementSpeed += _movementSpeedDelta;
                }
                else
                {
                    MovementSpeed -= _movementSpeedDelta;
                }
            }
        }
    }
}