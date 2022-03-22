using System.Globalization;
using MiodenusAnimationConverter.Shaders;
using NLog;
using OpenTK.Graphics.OpenGL;
using OpenTK.Mathematics;

namespace MiodenusAnimationConverter.Scene.Models.Meshes
{
    public class Mesh
    {
        private const int ColorChannelsAmount = 4;
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();
        private readonly float[] _vertexColorsBuffer;
        public readonly Triangle[] Triangles;
        private readonly int _vertexesAmount;
        private Vector3 _scale = Vector3.One;
        private VertexArrayObject _vao;
        private int _colorsVboIndex;
        public readonly Pivot Pivot = new ();
        public bool IsVisible = true;
        private bool _wasColorChanged;

        /*
         * Если создать несколько мешей на одном и том же массиве полигонов - цвет вершин будет изменяться одновременно
         * у всех "связанных" мешей.
         */
        public Mesh(in Triangle[] triangles)
        {
            Triangles = triangles;
            _vertexesAmount = Triangles.Length * Triangle.VertexesAmount;
            _vertexColorsBuffer = new float[_vertexesAmount * ColorChannelsAmount];
        }

        private (float[], float[]) VertexPositionsAndNormals
        {
            get
            {
                var trianglesAmount = Triangles.Length;
                var positions = new float[_vertexesAmount * 3];
                var normals = new float[_vertexesAmount * 3];
                var positionsIndex = 0;
                var normalsIndex = 0;

                for (var i = 0; i < trianglesAmount; i++)
                {
                    for (var j = 0; j < Triangle.VertexesAmount; j++)
                    {
                        positions[positionsIndex++] = Triangles[i].Vertexes[j].Position.X;
                        positions[positionsIndex++] = Triangles[i].Vertexes[j].Position.Y;
                        positions[positionsIndex++] = Triangles[i].Vertexes[j].Position.Z;
                    
                        normals[normalsIndex++] = Triangles[i].Vertexes[j].Normal.X;
                        normals[normalsIndex++] = Triangles[i].Vertexes[j].Normal.Y;
                        normals[normalsIndex++] = Triangles[i].Vertexes[j].Normal.Z;
                    }
                }

                return (positions, normals);
            }
        }

        private void UpdateVertexColorsBuffer()
        {
            var trianglesAmount = Triangles.Length;
            var index = 0;
            
            for (var i = 0; i < trianglesAmount; i++)
            {
                for (var j = 0; j < Triangle.VertexesAmount; j++)
                {
                    _vertexColorsBuffer[index++] = Triangles[i].Vertexes[j].Color.R;
                    _vertexColorsBuffer[index++] = Triangles[i].Vertexes[j].Color.G;
                    _vertexColorsBuffer[index++] = Triangles[i].Vertexes[j].Color.B;
                    _vertexColorsBuffer[index++] = Triangles[i].Vertexes[j].Color.A;
                }
            }
        }

        public void Scale(float scaleX = 1.0f, float scaleY = 1.0f, float scaleZ = 1.0f)
        {
            if (scaleX > 0.0f && scaleY > 0.0f && scaleZ > 0.0f)
            {
                _scale.X *= scaleX;
                _scale.Y *= scaleY;
                _scale.Z *= scaleZ;
            }
            else
            {
                Logger.Warn("Wrong scale parameters. Expected: values greater than 0 for X, Y and Z"
                        + $" components. Got: ({scaleX}; {scaleY}; {scaleZ}). Scale was not changed.");
            }
        }

        public void ResetScale() => _scale = Vector3.One;
        public Vector3 GetScale() => _scale;

        public Color4 Color
        {
            set
            {
                var trianglesAmount = Triangles.Length;
                
                for (var i = 0; i < trianglesAmount; i++)
                {
                    Triangles[i].Color = value;
                }

                _wasColorChanged = true;
            }
        }
        
        public void InitializeVao()
        {
            var (positions, normals) = VertexPositionsAndNormals;

            UpdateVertexColorsBuffer();
            _vao = new VertexArrayObject();
            _vao.AddVertexBufferObject(positions, 3);
            _vao.AddVertexBufferObject(normals, 3);
            _vao.AddVertexBufferObject(_vertexColorsBuffer, ColorChannelsAmount, BufferUsageHint.StreamDraw);
            _colorsVboIndex = _vao.VertexBufferObjectIndexes[^1];
        }
        
        private void UpdateVertexColorsVbo()
        {
            if (_wasColorChanged)
            {
                UpdateVertexColorsBuffer();
                _vao.UpdateVertexBufferObject(_colorsVboIndex, _vertexColorsBuffer);
                _wasColorChanged = false;
            }
        }

        /*
         * Если цвет был изменен не через свойство Color, а напрямую у каких-либо вершин
         * (через Triangles) - необходимо вручную сообщить о необходимости обновления цвета.
         */
        public void UpdateColors() => _wasColorChanged = true;
        
        /* Не может быть вызван самостоятельно (не из Model). */
        public void Draw(in ShaderProgram shaderProgram, PrimitiveType mode = PrimitiveType.Triangles)
        {
            if (IsVisible)
            {
                UpdateVertexColorsVbo();

                shaderProgram.SetVector3("mesh_pivot.position", Pivot.Position);
                shaderProgram.SetVector4("mesh_pivot.rotation", new Vector4(Pivot.Rotation.Xyz, Pivot.Rotation.W));
                shaderProgram.SetVector3("mesh_scale", _scale);

                _vao.Draw(_vertexesAmount, mode);
            }
        }

        public void DeleteVao() => _vao.Delete();

        public override string ToString()
        {
            return string.Format(CultureInfo.InvariantCulture, $"Mesh:\n\tTriangles amount: {Triangles.Length}\n\t"
                    + $"Scale: ({_scale.X}; {_scale.Y}; {_scale.Z})\n\t" + Pivot);
        }
    }
}