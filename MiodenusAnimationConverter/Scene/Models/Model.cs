using MiodenusAnimationConverter.Scene.Models.Meshes;
using Vector3 = OpenTK.Mathematics.Vector3;

namespace MiodenusAnimationConverter.Scene.Models
{
    public class Model : IMovable, IRotatable, IScalable 
    {
        public readonly Mesh Mesh;
        
        public Model(in Mesh mesh)
        {
            Mesh = new Mesh(mesh.Triangles);
        }

        public void Move(float deltaX, float deltaY, float deltaZ)
        {
            Mesh.Move(deltaX, deltaY, deltaZ);
        }

        public void Rotate(float angle, Vector3 vector)
        {
            Mesh.Rotate(angle, vector);
        }

        public void Scale(float scaleX, float scaleY, float scaleZ)
        {
            Mesh.Scale(scaleX, scaleY, scaleZ);
        }
    }
}