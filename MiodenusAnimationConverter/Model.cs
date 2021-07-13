using OpenTK.Graphics.OpenGL;

namespace MiodenusAnimationConverter
{
    public class Model
    {
        public readonly Triangle[] Triangles;
        
        public Model(Triangle[] triangles)
        {
            Triangles = triangles;
        }

        public void Scale(float scaleX, float scaleY, float scaleZ)
        {
            for (uint i = 0; i < Triangles.Length; i++)
            {
                for (byte j = 0; j < Triangle.VertexesAmount; j++)
                {
                    Triangles[i].Vertexes[j].Position.X *= scaleX;
                    Triangles[i].Vertexes[j].Position.Y *= scaleY;
                    Triangles[i].Vertexes[j].Position.Z *= scaleZ;
                }
            }
        }
        
        public void Scale(float scale)
        {
            Scale(scale, scale, scale);
        }
    }
}