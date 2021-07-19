using System.Numerics;

namespace MiodenusAnimationConverter.Scene.Models.Meshes
{
    public class Mesh : IMesh
    {
        public readonly Triangle[] Triangles;

        public Mesh(Triangle[] triangles)
        {
            Triangles = triangles;
        }
        
        public void Move(float deltaX, float deltaY, float deltaZ)
        {
            
        }

        public void MoveTo(float x, float y, float z)
        {
            
        }

        public void Rotate(float angle, Vector4 vector)
        {
            
        }

        public void Scale(float scaleX, float scaleY, float scaleZ)
        {
            
        }
    }
}