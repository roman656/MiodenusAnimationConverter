using System.Numerics;

namespace MiodenusAnimationConverter.Scene
{
    public interface IMovable : IMiodenusObject
    {
        public void Move(float deltaX, float deltaY, float deltaZ);
        public void MoveTo(float x, float y, float z);

        public void Move(Vector4 delta)
        {
            Move(delta.X, delta.Y, delta.Z);
        }

        public void MoveTo(Vector4 coordinates)
        {
            MoveTo(coordinates.X, coordinates.Y, coordinates.Z);
        }
    }
}