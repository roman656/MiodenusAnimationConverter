using OpenTK.Mathematics;

namespace MiodenusAnimationConverter.Scene
{
    public class LightPoint : IMovable, IRotatable
    {
        public Vector3 Position;
        public Vector4 Color;
        
        public LightPoint(Vector3 position, Color4 color) : this(position, (Vector4)color) {}
        
        public LightPoint(Vector3 position, Vector4 color)
        {
            Position = position;
            Color = color;
        }

        public void Move(float deltaX, float deltaY, float deltaZ)
        {
            Position.X += deltaX;
            Position.Y += deltaY;
            Position.Z += deltaZ;
        }

        public void Rotate(float angle, Vector3 vector)
        {
            Position = Quaternion.FromAxisAngle(vector, angle) * Position;
        }
    }
}