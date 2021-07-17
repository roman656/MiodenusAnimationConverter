using System.Numerics;

namespace MiodenusAnimationConverter.Scene
{
    public interface IScalable : IMiodenusObject
    {
        public void Scale(float scaleX, float scaleY, float scaleZ);
        
        public void Scale(float scale)
        {
            Scale(scale, scale, scale);
        }

        public void Scale(Vector4 scale)
        {
            Scale(scale.X, scale.Y, scale.Z);
        }
    }
}