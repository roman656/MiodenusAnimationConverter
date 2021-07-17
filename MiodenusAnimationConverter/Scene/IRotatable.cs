using System.Numerics;

namespace MiodenusAnimationConverter.Scene
{
    public interface IRotatable : IMiodenusObject
    {
        public void Rotate(float angle, Vector4 vector);
    }
}