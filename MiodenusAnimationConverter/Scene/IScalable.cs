using OpenTK.Mathematics;

namespace MiodenusAnimationConverter.Scene
{
    public interface IScalable
    {
        public void Scale(float scaleX, float scaleY, float scaleZ);

        public void Scale(float scale)
        {
            Scale(scale, scale, scale);
        }

        public void Scale(Vector3 scale)
        {
            Scale(scale.X, scale.Y, scale.Z);
        }
    }
}