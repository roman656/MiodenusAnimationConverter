using System.Globalization;
using OpenTK.Mathematics;

namespace MiodenusAnimationConverter.Scene.Models.Meshes
{
    public struct Vertex : IMovable, IRotatable, IScalable
    {
        public const byte SizeInBytes = (3 + 3 + 4) * sizeof(float) + Transformation.SizeInBytes;
        public readonly Vector3 Position;
        public readonly Vector3 Normal;
        public readonly Color4 Color;
        private Transformation _transformation;

        public Transformation Transformation => _transformation;

        public Vertex(Vector3 position, Vector3 normal, Color4 color)
        {
            Position = position;
            Normal = normal;
            Color = color;
            _transformation = new Transformation(Vector3.Zero, Quaternion.Identity, Vector3.One);
        }

        public void ResetTransformation()
        {
            _transformation.Reset();
        }

        public void Move(float deltaX, float deltaY, float deltaZ)
        {
            _transformation.Location.X += deltaX;
            _transformation.Location.Y += deltaY;
            _transformation.Location.Z += deltaZ;
        }

        public void Rotate(float angle, Vector3 vector)
        {
            _transformation.Rotation *= Quaternion.FromAxisAngle(vector, angle);
            _transformation.Rotation.Normalize();
        }

        public void Scale(float scaleX, float scaleY, float scaleZ)
        {
            _transformation.Scale.X *= scaleX;
            _transformation.Scale.Y *= scaleY;
            _transformation.Scale.Z *= scaleZ;
        }

        public override string ToString()
        {
            return string.Format(CultureInfo.InvariantCulture,
                    $"Vertex:\n\tPosition: ({Position.X}, {Position.Y}, {Position.Z})\n\t"
                    + $"Normal: ({Normal.X}, {Normal.Y}, {Normal.Z})\n\t"
                    + $"Color: ({Color.R}, {Color.G}, {Color.B}, {Color.A})\n\t"
                    + _transformation);
        }
    }
}