using System.Numerics;

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

        public void MoveTo(float x, float y, float z)
        {
            throw new System.NotImplementedException();
        }

        public void Rotate(float angle, Vector4 vector)
        {
            throw new System.NotImplementedException();
        }

        public void Scale(float scaleX, float scaleY, float scaleZ)
        {
            throw new System.NotImplementedException();
        }
    }
}