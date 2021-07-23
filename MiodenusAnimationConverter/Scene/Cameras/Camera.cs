using Vector3 = OpenTK.Mathematics.Vector3;

namespace MiodenusAnimationConverter.Scene.Cameras
{
    public class Camera : ICamera
    {
        public Camera()
        {
            
        }

        public void Move(float deltaX, float deltaY, float deltaZ)
        {
            throw new System.NotImplementedException();
        }

        public void Rotate(float angle, Vector3 vector)
        {
            throw new System.NotImplementedException();
        }

        public void Scale(float scaleX, float scaleY, float scaleZ)
        {
            throw new System.NotImplementedException();
        }
    }
}