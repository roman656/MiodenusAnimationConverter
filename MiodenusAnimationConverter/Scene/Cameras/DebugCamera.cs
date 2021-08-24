using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.GraphicsLibraryFramework;

namespace MiodenusAnimationConverter.Scene.Cameras
{
    public class DebugCamera : Camera
    {
        private float _movementSpeed = 2.0f;
        private float _movementSpeedDelta = 0.1f;
        private bool _isFirstMove = true;
        private Vector2 _previousMousePosition;
        public bool UseLocalCoordinateSystem = true;
        
        public DebugCamera(Vector3 position, int viewportWidth, int viewportHeight)
                : base(position, viewportWidth, viewportHeight) {}
        
        public void ProcessKeyboard(KeyboardState keyboardState, double deltaTime)
        {
            var velocity = (float)(_movementSpeed * deltaTime);

            if (keyboardState.IsKeyDown(Keys.L))
            {
                UseLocalCoordinateSystem = !UseLocalCoordinateSystem;
            }
            
            if (keyboardState.IsKeyDown(Keys.W))
            {
                if (UseLocalCoordinateSystem)
                {
                    MoveInViewDirection(velocity);
                }
                else
                {
                    Move(0.0f, 0.0f, -velocity);
                }
            }
            
            if (keyboardState.IsKeyDown(Keys.A))
            {
                if (UseLocalCoordinateSystem)
                {
                    MoveInRightDirection(-velocity);
                }
                else
                {
                    Move(-velocity, 0.0f, 0.0f);
                }
            }
            
            if (keyboardState.IsKeyDown(Keys.S))
            {
                if (UseLocalCoordinateSystem)
                {
                    MoveInViewDirection(-velocity);
                }
                else
                {
                    Move(0.0f, 0.0f, velocity);
                }
            }
            
            if (keyboardState.IsKeyDown(Keys.D))
            {
                if (UseLocalCoordinateSystem)
                {
                    MoveInRightDirection(velocity);
                }
                else
                {
                    Move(velocity, 0.0f, 0.0f);
                }
            }
            
            if (keyboardState.IsKeyDown(Keys.Space))
            {
                if (UseLocalCoordinateSystem)
                {
                    MoveInUpDirection(velocity);
                }
                else
                {
                    Move(0.0f, velocity, 0.0f);
                }
            }
            
            if (keyboardState.IsKeyDown(Keys.LeftShift))
            {
                if (UseLocalCoordinateSystem)
                {
                    MoveInUpDirection(-velocity);
                }
                else
                {
                    Move(0.0f, -velocity, 0.0f);
                }
            }
        }
        
        public void ProcessMouseMovement(MouseState mouseState, float mouseSensitivity = 0.01f)
        {
            if (_isFirstMove)
            {
                _previousMousePosition = new Vector2(mouseState.X, mouseState.Y);
                _isFirstMove = false;
            }
            else
            {
                var (x, y) = (mouseState.Position - _previousMousePosition) * -mouseSensitivity;
                
                _previousMousePosition = mouseState.Position;
                RotateViewDirection(x, Up);
                RotateViewDirection(y, Right);
            }
        }
        
        public void ProcessMouseScroll(MouseWheelEventArgs args, KeyboardState keyboardState, float scrollSensitivity = 2.0f)
        {
            if (keyboardState.IsKeyDown(Keys.LeftControl))
            {
                Fov -= args.OffsetY * scrollSensitivity;
            }
            else
            {
                if (args.OffsetY > 0.0f)
                {
                    _movementSpeed += _movementSpeedDelta;
                }
                else if (_movementSpeed > _movementSpeedDelta)
                {
                    _movementSpeed -= _movementSpeedDelta;
                }
            }
        }
    }
}