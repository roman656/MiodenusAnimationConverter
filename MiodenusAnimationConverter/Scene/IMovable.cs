using OpenTK.Mathematics;

namespace MiodenusAnimationConverter.Scene
{
    public interface IMovable
    {
        public void Move(float deltaX, float deltaY, float deltaZ);

        public void Move(Vector3 delta)
        {
            Move(delta.X, delta.Y, delta.Z);
        }
    }
}