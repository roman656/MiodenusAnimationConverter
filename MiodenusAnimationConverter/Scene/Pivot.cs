using System.Globalization;
using OpenTK.Mathematics;

namespace MiodenusAnimationConverter.Scene
{
    public class Pivot
    {
        public Vector3 Position;
        private Vector3 _xAxisPositiveDirection = Vector3.UnitX;
        private Vector3 _yAxisPositiveDirection = Vector3.UnitY;
        private Vector3 _zAxisPositiveDirection = Vector3.UnitZ;

        public Pivot(Vector3 position)
        {
            Position = position;
        }
        
        public Pivot() : this(Vector3.Zero) {}
        
        public Vector3 XAxisPositiveDirection => _xAxisPositiveDirection;
        public Vector3 YAxisPositiveDirection => _yAxisPositiveDirection;
        public Vector3 ZAxisPositiveDirection => _zAxisPositiveDirection;
        
        public void ResetLocalRotation()
        {
            _xAxisPositiveDirection = Vector3.UnitX;
            _yAxisPositiveDirection = Vector3.UnitY;
            _zAxisPositiveDirection = Vector3.UnitZ;
        }
        
        public void GlobalMove(float deltaX = 0.0f, float deltaY = 0.0f, float deltaZ = 0.0f)
        {
            Position.X += deltaX;
            Position.Y += deltaY;
            Position.Z += deltaZ;
        }

        public void LocalMove(float deltaX = 0.0f, float deltaY = 0.0f, float deltaZ = 0.0f)
        {
            Position.X += _xAxisPositiveDirection.X * deltaX + _yAxisPositiveDirection.X * deltaY
                    + _zAxisPositiveDirection.X * deltaZ;
            Position.Y += _xAxisPositiveDirection.Y * deltaX + _yAxisPositiveDirection.Y * deltaY
                    + _zAxisPositiveDirection.Y * deltaZ;
            Position.Z += _xAxisPositiveDirection.Z * deltaX + _yAxisPositiveDirection.Z * deltaY
                    + _zAxisPositiveDirection.Z * deltaZ;
        }

        public void GlobalRotate(float angle, Vector3 vector)
        {
            Position = Quaternion.FromAxisAngle(vector, angle) * Position;
        }
        
        public void LocalRotate(float angle, Vector3 vector)
        {
            var rotation = Quaternion.FromAxisAngle(vector, angle);
            
            _xAxisPositiveDirection = Vector3.Normalize(rotation * _xAxisPositiveDirection);
            _zAxisPositiveDirection = Vector3.Normalize(rotation * _zAxisPositiveDirection);
            _yAxisPositiveDirection = Vector3.Normalize(Vector3.Cross(_zAxisPositiveDirection,
                    _xAxisPositiveDirection));
        }
        
        public override string ToString()
        {
            return string.Format(CultureInfo.InvariantCulture, $"Pivot: [ Position: ({Position.X}; {Position.Y};"
                    + $" {Position.Z}) | X axis positive direction: ({_xAxisPositiveDirection.X};"
                    + $" {_xAxisPositiveDirection.Y}; {_xAxisPositiveDirection.Z}) | Y axis positive direction: "
                    + $"({_yAxisPositiveDirection.X}; {_yAxisPositiveDirection.Y}; {_yAxisPositiveDirection.Z}) "
                    + $"| Z axis positive direction: ({_zAxisPositiveDirection.X}; {_zAxisPositiveDirection.Y}; "
                    + $"{_zAxisPositiveDirection.Z}) ]");
        }
    }
}