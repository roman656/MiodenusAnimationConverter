using System;
using Vector3 = OpenTK.Mathematics.Vector3;

namespace MiodenusAnimationConverter.Scene.Models.Meshes
{
    public class Mesh : IMovable, IRotatable, IScalable 
    {
        public readonly Triangle[] Triangles;

        public Mesh(in Triangle[] triangles)
        {
            Triangles = new Triangle[triangles.Length];
            Array.Copy(triangles, Triangles, triangles.Length);
        }

        public void Move(float deltaX, float deltaY, float deltaZ)
        {
            foreach (var triangle in Triangles)
            {
                triangle.Move(deltaX, deltaY, deltaZ);
            }
        }

        public void Rotate(float angle, Vector3 vector)
        {
            foreach (var triangle in Triangles)
            {
                triangle.Rotate(angle, vector);
            }
        }

        public void Scale(float scaleX, float scaleY, float scaleZ)
        {
            foreach (var triangle in Triangles)
            {
                triangle.Scale(scaleX, scaleY, scaleZ);
            }
        }
    }
}