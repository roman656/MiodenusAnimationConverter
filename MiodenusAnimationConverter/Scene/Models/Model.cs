using MiodenusAnimationConverter.Scene.Cameras;
using MiodenusAnimationConverter.Scene.Models.Meshes;
using MiodenusAnimationConverter.Shaders;
using OpenTK.Graphics.OpenGL;
using OpenTK.Mathematics;

namespace MiodenusAnimationConverter.Scene.Models
{
    public class Model 
    {
        private const int ColorChannelsAmount = 4;
        public string Name;
        public readonly Mesh Mesh;
        private readonly int _vertexesAmount;
        private readonly float[] _vertexesColorsBuffer;
        private VertexArrayObject _vao;
        private int _colorsVboIndex;
        private bool _wasColorChanged;
        public bool IsVisible = true;

        public Model(in Mesh mesh)
        {
            Mesh = new Mesh(mesh.Triangles, mesh.Transformation);
            _vertexesAmount = Mesh.Triangles.Length * Triangle.VertexesAmount;
            _vertexesColorsBuffer = new float[_vertexesAmount * ColorChannelsAmount];
        }

        private void UpdateVertexesColorsBuffer()
        {
            var index = 0;
            var trianglesAmount = Mesh.Triangles.Length;
            
            for (var i = 0; i < trianglesAmount; i++)
            {
                for (var j = 0; j < Triangle.VertexesAmount; j++)
                {
                    _vertexesColorsBuffer[index++] = Mesh.Triangles[i].Vertexes[j].Color.R;
                    _vertexesColorsBuffer[index++] = Mesh.Triangles[i].Vertexes[j].Color.G;
                    _vertexesColorsBuffer[index++] = Mesh.Triangles[i].Vertexes[j].Color.B;
                    _vertexesColorsBuffer[index++] = Mesh.Triangles[i].Vertexes[j].Color.A;
                }
            }
        }

        public void Initialize()
        {
            var trianglesAmount = Mesh.Triangles.Length;
            var vertexesPositions = new float[_vertexesAmount * 3];
            var vertexesNormals = new float[_vertexesAmount * 3];
            var positionsIndex = 0;
            var normalsIndex = 0;

            UpdateVertexesColorsBuffer();

            for (var i = 0; i < trianglesAmount; i++)
            {
                for (var j = 0; j < Triangle.VertexesAmount; j++)
                {
                    vertexesPositions[positionsIndex++] = Mesh.Triangles[i].Vertexes[j].Position.X;
                    vertexesPositions[positionsIndex++] = Mesh.Triangles[i].Vertexes[j].Position.Y;
                    vertexesPositions[positionsIndex++] = Mesh.Triangles[i].Vertexes[j].Position.Z;
                    
                    vertexesNormals[normalsIndex++] = Mesh.Triangles[i].Vertexes[j].Normal.X;
                    vertexesNormals[normalsIndex++] = Mesh.Triangles[i].Vertexes[j].Normal.Y;
                    vertexesNormals[normalsIndex++] = Mesh.Triangles[i].Vertexes[j].Normal.Z;
                }
            }
            
            _vao = new VertexArrayObject();
            
            _vao.AddVertexBufferObject(vertexesPositions, 3);
            _vao.AddVertexBufferObject(vertexesNormals, 3);
            _vao.AddVertexBufferObject(_vertexesColorsBuffer, ColorChannelsAmount,
                    BufferUsageHint.StreamDraw);

            _colorsVboIndex = _vao.VertexBufferObjectIndexes[^1];
        }
        
        private void UpdateVertexesColorsVbo()
        {
            if (_wasColorChanged)
            {
                UpdateVertexesColorsBuffer();
                _vao.UpdateVertexBufferObject(_colorsVboIndex, _vertexesColorsBuffer);
                _wasColorChanged = false;
            }
        }

        public void UpdateMeshColor()
        {
            _wasColorChanged = true;
        }
        
        public void Delete()
        {
            _vao.Delete();
        }

        public void Draw(in ShaderProgram shaderProgram, in Camera camera, PrimitiveType mode = PrimitiveType.Triangles)
        {
            if (IsVisible)
            {
                UpdateVertexesColorsVbo();

                var rotation = new Vector4(Mesh.Transformation.Rotation.Xyz, Mesh.Transformation.Rotation.W);

                shaderProgram.SetVector3("t_location", Mesh.Transformation.Location);
                shaderProgram.SetVector4("t_rotation", rotation);
                shaderProgram.SetVector3("t_scale", Mesh.Transformation.Scale);
                shaderProgram.SetMatrix4("view", camera.ViewMatrix, false);
                shaderProgram.SetMatrix4("projection", camera.ProjectionMatrix, false);

                _vao.Draw(_vertexesAmount, mode);
            }
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