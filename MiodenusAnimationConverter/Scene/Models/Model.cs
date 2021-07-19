using System.Collections.Generic;
using System.Numerics;
using MiodenusAnimationConverter.Scene.Models.Meshes;

namespace MiodenusAnimationConverter.Scene.Models
{
    public class Model : IModel
    {
        public readonly Triangle[] Triangles;
        public List<Mesh> Meshes;
        
        public Model(Triangle[] triangles)
        {
            Triangles = triangles;
        }

        public void Scale(float scaleX, float scaleY, float scaleZ)
        {
            
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
    }
}