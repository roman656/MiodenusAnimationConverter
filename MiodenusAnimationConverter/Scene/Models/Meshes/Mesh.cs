using System;
using System.Globalization;
using MiodenusAnimationConverter.Scene.Cameras;
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
        private Vector3 _scale = Vector3.One;
        private VertexArrayObject _vao;
        public readonly int VertexesAmount;
        public readonly Pivot Pivot = new ();
        public bool WasColorChanged;
        private int _vertexesVboIndex;
        private int _colorsVboIndex;
        private bool _isTimeToUpdateVbo;
        private bool _isTimeToUpdateUniform = true;
        public bool IsVisible = true;
        
        public void InitializeVao()
        {
            UpdateVertexesColorsBuffer();

            var (positions, normals) = Meshes["name"].VertexPositionsAndNormals;
            var colors = Meshes["name"].VertexColorsBuffer;
            
            _vao = new VertexArrayObject();
            _vao.AddVertexBufferObject(positions, 3);
            _vao.AddVertexBufferObject(normals, 3);
            _vao.AddVertexBufferObject(colors, ColorChannelsAmount,
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
        
        public void Draw(in ShaderProgram shaderProgram, in Camera camera, PrimitiveType mode = PrimitiveType.Triangles)
        {
            if (IsVisible)
            {
                UpdateVertexesColorsVbo();

                var rotation = new Vector4(Meshes.Transformation.Rotation.Xyz, Meshes.Transformation.Rotation.W);
                
                shaderProgram.SetMatrix4("view", camera.ViewMatrix, false);
                shaderProgram.SetMatrix4("projection", camera.ProjectionMatrix, false);
                shaderProgram.SetVector3("pivot.position", Pivot.Position);
                shaderProgram.SetVector4("pivot.rotation", new Vector4(Pivot.Rotation.Xyz, Pivot.Rotation.W));
                shaderProgram.SetVector3("scale", Meshes.Transformation.Scale);

                _vao.Draw(_vertexesAmount, mode);
            }
        }

        
        public void DeleteVao() => _vao.Delete();

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
                VertexesAmount = _triangles.Length * Triangle.VertexesAmount;
                _vertexColorsBuffer = new float[VertexesAmount * ColorChannelsAmount];
                UpdateVertexColorsBuffer();
            }
        }

        public (float[], float[]) VertexPositionsAndNormals
        {
            get
            {
                var trianglesAmount = _triangles.Length;
                var positions = new float[VertexesAmount * 3];
                var normals = new float[VertexesAmount * 3];
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

        public float[] VertexColorsBuffer
        {
            get
            {
                if (WasColorChanged)
                {
                    UpdateVertexColorsBuffer();
                    WasColorChanged = false;
                }

                return _vertexColorsBuffer;
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
        
        public object Clone()
        {
            return new Mesh(this);
        }

        public override string ToString()
        {
            return string.Format(CultureInfo.InvariantCulture, $"Mesh:\n\tTriangles amount: {_triangles.Length}\n\t"
                    + $"Scale: ({_scale.X}; {_scale.Y}; {_scale.Z})\n\t" + Pivot);
        }
    }
}