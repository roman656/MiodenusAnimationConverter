using OpenTK.Mathematics;

namespace MiodenusAnimationConverter.Scene
{
    public interface IRotatable
    {
        public void Rotate(float angle, Vector3 vector);
    }
}