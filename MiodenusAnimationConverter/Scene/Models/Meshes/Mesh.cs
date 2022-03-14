using System;
using System.Globalization;
using MiodenusAnimationConverter.Shaders;
using NLog;
using OpenTK.Graphics.OpenGL;
using OpenTK.Mathematics;

namespace MiodenusAnimationConverter.Scene.Models.Meshes
{
    public class Mesh : ICloneable
    {
        private const int ColorChannelsAmount = 4;
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();
        private readonly float[] _vertexColorsBuffer;
        private readonly Triangle[] _triangles;
        private readonly int _vertexesAmount;
        private Vector3 _scale = Vector3.One;
        private VertexArrayObject _vao;
        private int _colorsVboIndex;
        public readonly Pivot Pivot = new ();
        public bool IsVisible = true;
        public bool WasColorChanged;

        public Mesh(in Triangle[] triangles) => Triangles = triangles;
        public Mesh(in Triangle[] triangles, in Pivot pivot) : this(triangles, pivot, Vector3.One) {}
        
        public Mesh(in Triangle[] triangles, in Pivot pivot, Vector3 scale)
        {
            Triangles = triangles;
            Pivot = (Pivot)pivot.Clone();
            Scale = scale;
        }
        
        public Mesh(in Mesh mesh)
        {
            Triangles = mesh._triangles;
            Pivot = (Pivot)mesh.Pivot.Clone();
            _scale = mesh._scale;
        }

        public Triangle[] Triangles
        {
            get => _triangles;
            private init
            {
                _triangles = new Triangle[value.Length];
                Array.Copy(value, _triangles, value.Length);
                _vertexesAmount = _triangles.Length * Triangle.VertexesAmount;
                _vertexColorsBuffer = new float[_vertexesAmount * ColorChannelsAmount];
            }
        }

        private (float[], float[]) VertexPositionsAndNormals
        {
            get
            {
                var trianglesAmount = _triangles.Length;
                var positions = new float[_vertexesAmount * 3];
                var normals = new float[_vertexesAmount * 3];
                var positionsIndex = 0;
                var normalsIndex = 0;

                for (var i = 0; i < trianglesAmount; i++)
                {
                    for (var j = 0; j < Triangle.VertexesAmount; j++)
                    {
                        positions[positionsIndex++] = _triangles[i].Vertexes[j].Position.X;
                        positions[positionsIndex++] = _triangles[i].Vertexes[j].Position.Y;
                        positions[positionsIndex++] = _triangles[i].Vertexes[j].Position.Z;
                    
                        normals[normalsIndex++] = _triangles[i].Vertexes[j].Normal.X;
                        normals[normalsIndex++] = _triangles[i].Vertexes[j].Normal.Y;
                        normals[normalsIndex++] = _triangles[i].Vertexes[j].Normal.Z;
                    }
                }

                return (positions, normals);
            }
        }

        private void UpdateVertexColorsBuffer()
        {
            var trianglesAmount = _triangles.Length;
            var index = 0;
            
            for (var i = 0; i < trianglesAmount; i++)
            {
                for (var j = 0; j < Triangle.VertexesAmount; j++)
                {
                    _vertexColorsBuffer[index++] = _triangles[i].Vertexes[j].Color.R;
                    _vertexColorsBuffer[index++] = _triangles[i].Vertexes[j].Color.G;
                    _vertexColorsBuffer[index++] = _triangles[i].Vertexes[j].Color.B;
                    _vertexColorsBuffer[index++] = _triangles[i].Vertexes[j].Color.A;
                }
            }
        }
        
        public void ResetScale() => _scale = Vector3.One;

        public Vector3 Scale
        {
            get => _scale;
            set
            {
                if (value.X > 0.0f && value.Y > 0.0f && value.Z > 0.0f)
                {
                    _scale = value;
                }
                else
                {
                    Logger.Warn("Wrong value for Scale parameter. Expected: value"
                            + $" greater than 0 for X, Y and Z components. Got: {value}. Scale was not changed.");
                }
            }
        }

        public Color4 Color
        {
            set
            {
                var trianglesAmount = _triangles.Length;
                
                for (var i = 0; i < trianglesAmount; i++)
                {
                    _triangles[i].Color = value;
                }

                WasColorChanged = true;
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
            if (WasColorChanged)
            {
                UpdateVertexColorsBuffer();
                _vao.UpdateVertexBufferObject(_colorsVboIndex, _vertexColorsBuffer);
                WasColorChanged = false;
            }
        }
        
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
        public object Clone() => new Mesh(this);

        public override string ToString()
        {
            return string.Format(CultureInfo.InvariantCulture, $"Mesh:\n\tTriangles amount: {_triangles.Length}\n\t"
                    + $"Scale: ({_scale.X}; {_scale.Y}; {_scale.Z})\n\t" + Pivot);
        }
    }
}