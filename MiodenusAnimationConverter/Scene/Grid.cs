using System;
using System.Collections.Generic;
using MiodenusAnimationConverter.Scene.Cameras;
using MiodenusAnimationConverter.Shaders;
using MiodenusAnimationConverter.Shaders.FragmentShaders;
using MiodenusAnimationConverter.Shaders.VertexShaders;
using NLog;
using OpenTK.Graphics.OpenGL;
using OpenTK.Mathematics;

namespace MiodenusAnimationConverter.Scene
{
    public class Grid
    {
        private const int ColorChannelsAmount = 4;
        private static readonly Color4 DefaultColor = Color4.DarkGray;
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();
        private bool _isXyPlaneVisible = true;
        private bool _isYzPlaneVisible = true;
        private bool _isXzPlaneVisible = true;
        private float _cellSize = 1.0f;
        private int _xSizeInCells;
        private int _ySizeInCells;
        private int _zSizeInCells;
        private float _lineWidth = 1.0f;
        private Color4 _xyPlaneColor = DefaultColor;
        private Color4 _xzPlaneColor = DefaultColor;
        private Color4 _yzPlaneColor = DefaultColor;
        private int _vertexesVboIndex;
        private int _colorsVboIndex;
        private VertexArrayObject _vao;
        private ShaderProgram _shaderProgram;
        private bool _isTimeToUpdateVbo;
        private int _vertexesAmount;
        public readonly Pivot Pivot;
        public bool IsVisible = true;

        public Grid(int xSizeInCells = 0, int ySizeInCells = 0, int zSizeInCells = 0, float cellSize = 1.0f,
                float lineWidth = 1.0f) : this(Vector3.Zero, DefaultColor, xSizeInCells, ySizeInCells,
                zSizeInCells, cellSize, lineWidth) {}
        
        public Grid(Vector3 position, Color4 color, int xSizeInCells = 0, int ySizeInCells = 0, int zSizeInCells = 0,
                float cellSize = 1.0f, float lineWidth = 1.0f) : this(position, color, color,
                color, xSizeInCells, ySizeInCells, zSizeInCells, cellSize, lineWidth) {}
        
        public Grid(Vector3 position, Color4 xyPlaneColor, Color4 xzPlaneColor, Color4 yzPlaneColor,
                int xSizeInCells = 0, int ySizeInCells = 0, int zSizeInCells = 0, float cellSize = 1.0f,
                float lineWidth = 1.0f) : this(new Pivot(position, lineWidth + 1.0f,
                xSizeInCells * cellSize / 2.0f, ySizeInCells * cellSize / 2.0f,
                zSizeInCells * cellSize / 2.0f), xyPlaneColor, xzPlaneColor, yzPlaneColor, xSizeInCells,
                ySizeInCells, zSizeInCells, cellSize, lineWidth) {}
        
        public Grid(in Pivot pivot, Color4 color, int xSizeInCells = 0, int ySizeInCells = 0, int zSizeInCells = 0,
                float cellSize = 1.0f, float lineWidth = 1.0f) : this(pivot, color, color,
                color, xSizeInCells, ySizeInCells, zSizeInCells, cellSize, lineWidth) {}

        public Grid(in Pivot pivot, Color4 xyPlaneColor, Color4 xzPlaneColor, Color4 yzPlaneColor,
                int xSizeInCells = 0, int ySizeInCells = 0, int zSizeInCells = 0, float cellSize = 1.0f, 
                float lineWidth = 1.0f)
        {
            Pivot = (Pivot)pivot.Clone();
            CellSize = cellSize;
            XSizeInCells = xSizeInCells;
            YSizeInCells = ySizeInCells;
            ZSizeInCells = zSizeInCells;
            LineWidth = lineWidth;
            XyPlaneColor = xyPlaneColor;
            XzPlaneColor = xzPlaneColor;
            YzPlaneColor = yzPlaneColor;
            _isTimeToUpdateVbo = false;
        }

        private void UpdateVertexesAmount()
        {
            var xyPlaneVertexesAmount = !_isXyPlaneVisible || _xSizeInCells <= 0 || _ySizeInCells <= 0 ? 0 : 2
                    * (_xSizeInCells + 1) + 2 * (_ySizeInCells + 1);
            var xzPlaneVertexesAmount = !_isXzPlaneVisible || _xSizeInCells <= 0 || _zSizeInCells <= 0 ? 0 : 2
                    * (_xSizeInCells + 1) + 2 * (_zSizeInCells + 1);
            var yzPlaneVertexesAmount = !_isYzPlaneVisible || _ySizeInCells <= 0 || _zSizeInCells <= 0 ? 0 : 2
                    * (_ySizeInCells + 1) + 2 * (_zSizeInCells + 1);
                
            _vertexesAmount = xyPlaneVertexesAmount + xzPlaneVertexesAmount + yzPlaneVertexesAmount;
        }

        private (float[], float[]) VertexesAndColors
        {
            get
            {
                UpdateVertexesAmount();

                if (_vertexesAmount == 0)
                {
                    return (Array.Empty<float>(), Array.Empty<float>());
                }
                
                var vertexes = new float[_vertexesAmount * 3];
                var colors = new float[_vertexesAmount * ColorChannelsAmount];
                var vertexIndex = 0;
                var colorIndex = 0;
                var xSize = _xSizeInCells * _cellSize;
                var ySize = _ySizeInCells * _cellSize;
                var zSize = _zSizeInCells * _cellSize;

                if (_isXzPlaneVisible && _xSizeInCells > 0 && _zSizeInCells > 0)
                {
                    for (var i = 0; i <= _zSizeInCells; i++)
                    {
                        vertexes[vertexIndex++] = -xSize / 2.0f;
                        vertexes[vertexIndex++] = 0.0f;
                        vertexes[vertexIndex++] = -zSize / 2.0f + _cellSize * i;
                        
                        colors[colorIndex++] = _xzPlaneColor.R;
                        colors[colorIndex++] = _xzPlaneColor.G;
                        colors[colorIndex++] = _xzPlaneColor.B;
                        colors[colorIndex++] = _xzPlaneColor.A;
                        
                        vertexes[vertexIndex++] = xSize / 2.0f;
                        vertexes[vertexIndex++] = 0.0f;
                        vertexes[vertexIndex++] = -zSize / 2.0f + _cellSize * i;
                        
                        colors[colorIndex++] = _xzPlaneColor.R;
                        colors[colorIndex++] = _xzPlaneColor.G;
                        colors[colorIndex++] = _xzPlaneColor.B;
                        colors[colorIndex++] = _xzPlaneColor.A;
                    }

                    for (var i = 0; i <= _xSizeInCells; i++)
                    {
                        vertexes[vertexIndex++] = -xSize / 2.0f + _cellSize * i;
                        vertexes[vertexIndex++] = 0.0f;
                        vertexes[vertexIndex++] = -zSize / 2.0f;
                        
                        colors[colorIndex++] = _xzPlaneColor.R;
                        colors[colorIndex++] = _xzPlaneColor.G;
                        colors[colorIndex++] = _xzPlaneColor.B;
                        colors[colorIndex++] = _xzPlaneColor.A;
                        
                        vertexes[vertexIndex++] = -xSize / 2.0f + _cellSize * i;
                        vertexes[vertexIndex++] = 0.0f;
                        vertexes[vertexIndex++] = zSize / 2.0f;
                        
                        colors[colorIndex++] = _xzPlaneColor.R;
                        colors[colorIndex++] = _xzPlaneColor.G;
                        colors[colorIndex++] = _xzPlaneColor.B;
                        colors[colorIndex++] = _xzPlaneColor.A;
                    }
                }

                if (_isXyPlaneVisible && _xSizeInCells > 0 && _ySizeInCells > 0)
                {
                    for (var i = 0; i <= _ySizeInCells; i++)
                    {
                        vertexes[vertexIndex++] = -xSize / 2.0f;
                        vertexes[vertexIndex++] = -ySize / 2.0f + _cellSize * i;
                        vertexes[vertexIndex++] = 0.0f;
                        
                        colors[colorIndex++] = _xyPlaneColor.R;
                        colors[colorIndex++] = _xyPlaneColor.G;
                        colors[colorIndex++] = _xyPlaneColor.B;
                        colors[colorIndex++] = _xyPlaneColor.A;
                        
                        vertexes[vertexIndex++] = xSize / 2.0f;
                        vertexes[vertexIndex++] = -ySize / 2.0f + _cellSize * i;
                        vertexes[vertexIndex++] = 0.0f;
                        
                        colors[colorIndex++] = _xyPlaneColor.R;
                        colors[colorIndex++] = _xyPlaneColor.G;
                        colors[colorIndex++] = _xyPlaneColor.B;
                        colors[colorIndex++] = _xyPlaneColor.A;
                    }

                    for (var i = 0; i <= _xSizeInCells; i++)
                    {
                        vertexes[vertexIndex++] = -xSize / 2.0f + _cellSize * i;
                        vertexes[vertexIndex++] = -ySize / 2.0f;
                        vertexes[vertexIndex++] = 0.0f;
                        
                        colors[colorIndex++] = _xyPlaneColor.R;
                        colors[colorIndex++] = _xyPlaneColor.G;
                        colors[colorIndex++] = _xyPlaneColor.B;
                        colors[colorIndex++] = _xyPlaneColor.A;
                        
                        vertexes[vertexIndex++] = -xSize / 2.0f + _cellSize * i;
                        vertexes[vertexIndex++] = ySize / 2.0f;
                        vertexes[vertexIndex++] = 0.0f;
                        
                        colors[colorIndex++] = _xyPlaneColor.R;
                        colors[colorIndex++] = _xyPlaneColor.G;
                        colors[colorIndex++] = _xyPlaneColor.B;
                        colors[colorIndex++] = _xyPlaneColor.A;
                    }
                }

                if (_isYzPlaneVisible && _ySizeInCells > 0 && _zSizeInCells > 0)
                {
                    for (var i = 0; i <= _ySizeInCells; i++)
                    {
                        vertexes[vertexIndex++] = 0.0f;
                        vertexes[vertexIndex++] = -ySize / 2.0f + _cellSize * i;
                        vertexes[vertexIndex++] = -zSize / 2.0f;
                        
                        colors[colorIndex++] = _yzPlaneColor.R;
                        colors[colorIndex++] = _yzPlaneColor.G;
                        colors[colorIndex++] = _yzPlaneColor.B;
                        colors[colorIndex++] = _yzPlaneColor.A;
                        
                        vertexes[vertexIndex++] = 0.0f;
                        vertexes[vertexIndex++] = -ySize / 2.0f + _cellSize * i;
                        vertexes[vertexIndex++] = zSize / 2.0f;
                        
                        colors[colorIndex++] = _yzPlaneColor.R;
                        colors[colorIndex++] = _yzPlaneColor.G;
                        colors[colorIndex++] = _yzPlaneColor.B;
                        colors[colorIndex++] = _yzPlaneColor.A;
                    }

                    for (var i = 0; i <= _zSizeInCells; i++)
                    {
                        vertexes[vertexIndex++] = 0.0f;
                        vertexes[vertexIndex++] = -ySize / 2.0f;
                        vertexes[vertexIndex++] = -zSize / 2.0f + _cellSize * i;
                        
                        colors[colorIndex++] = _yzPlaneColor.R;
                        colors[colorIndex++] = _yzPlaneColor.G;
                        colors[colorIndex++] = _yzPlaneColor.B;
                        colors[colorIndex++] = _yzPlaneColor.A;
                        
                        vertexes[vertexIndex++] = 0.0f;
                        vertexes[vertexIndex++] = ySize / 2.0f;
                        vertexes[vertexIndex++] = -zSize / 2.0f + _cellSize * i;
                        
                        colors[colorIndex++] = _yzPlaneColor.R;
                        colors[colorIndex++] = _yzPlaneColor.G;
                        colors[colorIndex++] = _yzPlaneColor.B;
                        colors[colorIndex++] = _yzPlaneColor.A;
                    }
                }

                return (vertexes, colors);
            }
        }

        public bool IsXyPlaneVisible
        {
            get => _isXyPlaneVisible;
            set
            {
                _isXyPlaneVisible = value;
                _isTimeToUpdateVbo = true;
            }
        }
        
        public bool IsXzPlaneVisible
        {
            get => _isXzPlaneVisible;
            set
            {
                _isXzPlaneVisible = value;
                _isTimeToUpdateVbo = true;
            }
        }
        
        public bool IsYzPlaneVisible
        {
            get => _isYzPlaneVisible;
            set
            {
                _isYzPlaneVisible = value;
                _isTimeToUpdateVbo = true;
            }
        }

        public float CellSize
        {
            get => _cellSize;
            set
            {
                if (value > 0.0f)
                {
                    _cellSize = value;
                    _isTimeToUpdateVbo = true;
                }
                else
                {
                    Logger.Warn("Wrong value for CellSize parameter. Expected: value"
                            + $" greater than 0. Got: {value}. Cell size was not changed.");
                }
            }
        }
        
        public int XyzSizeInCells
        {
            set
            {
                if (value >= 0)
                {
                    _xSizeInCells = value;
                    _ySizeInCells = value;
                    _zSizeInCells = value;
                    _isTimeToUpdateVbo = true;
                }
                else
                {
                    Logger.Warn("Wrong value for XyzSizeInCells parameter. Expected: value"
                            + $" greater than or equal to 0. Got: {value}. Grid size was not changed.");
                }
            }
        }
        
        public int XSizeInCells
        {
            get => _xSizeInCells;
            set
            {
                if (value >= 0)
                {
                    _xSizeInCells = value;
                    _isTimeToUpdateVbo = true;
                }
                else
                {
                    Logger.Warn("Wrong value for XSizeInCells parameter. Expected: value"
                            + $" greater than or equal to 0. Got: {value}. X size in cells was not changed.");
                }
            }
        }
        
        public int YSizeInCells
        {
            get => _ySizeInCells;
            set
            {
                if (value >= 0)
                {
                    _ySizeInCells = value;
                    _isTimeToUpdateVbo = true;
                }
                else
                {
                    Logger.Warn("Wrong value for YSizeInCells parameter. Expected: value"
                            + $" greater than or equal to 0. Got: {value}. Y size in cells was not changed.");
                }
            }
        }
        
        public int ZSizeInCells
        {
            get => _zSizeInCells;
            set
            {
                if (value >= 0)
                {
                    _zSizeInCells = value;
                    _isTimeToUpdateVbo = true;
                }
                else
                {
                    Logger.Warn("Wrong value for ZSizeInCells parameter. Expected: value"
                            + $" greater than or equal to 0. Got: {value}. Z size in cells was not changed.");
                }
            }
        }
        
        public float LineWidth
        {
            get => _lineWidth;
            set
            {
                if (value > 0.0f)
                {
                    _lineWidth = value;
                }
                else
                {
                    Logger.Warn("Wrong value for LineWidth parameter. Expected: value"
                            + $" greater than 0. Got: {value}. Line width was not changed.");
                }
            }
        }
        
        public Color4 Color
        {
            set
            {
                _xzPlaneColor = value;
                _xyPlaneColor = value;
                _yzPlaneColor = value;
                _isTimeToUpdateVbo = true;
            }
        }
        
        public Color4 XzPlaneColor
        {
            get => _xzPlaneColor;
            set
            {
                _xzPlaneColor = value;
                _isTimeToUpdateVbo = true;
            }
        }
        
        public Color4 XyPlaneColor
        {
            get => _xyPlaneColor;
            set
            {
                _xyPlaneColor = value;
                _isTimeToUpdateVbo = true;
            }
        }
        
        public Color4 YzPlaneColor
        {
            get => _yzPlaneColor;
            set
            {
                _yzPlaneColor = value;
                _isTimeToUpdateVbo = true;
            }
        }

        public void InitializeVao()
        {
            var (vertexes, colors) = VertexesAndColors;
            
            Pivot.InitializeVao();
            InitializeShaderProgram();
            _vao = new VertexArrayObject();
            _vao.AddVertexBufferObject(vertexes, 3, BufferUsageHint.StreamDraw);
            _vertexesVboIndex = _vao.VertexBufferObjectIndexes[^1];
            _vao.AddVertexBufferObject(colors, ColorChannelsAmount, BufferUsageHint.StreamDraw);
            _colorsVboIndex = _vao.VertexBufferObjectIndexes[^1];
        }
        
        private void InitializeShaderProgram()
        {
            var shaders = new List<Shader>
            {
                new (GridVertexShader.Code, GridVertexShader.Type),
                new (GridFragmentShader.Code, GridFragmentShader.Type)
            };

            _shaderProgram = new ShaderProgram(shaders);

            for (var i = 0; i < shaders.Count; i++)
            {
                shaders[i].Delete();
            }
        }
        
        private void UpdateVbo()
        {
            if (_isTimeToUpdateVbo)
            {
                var (vertexes, colors) = VertexesAndColors;
                
                _vao.UpdateVertexBufferObject(_vertexesVboIndex, vertexes);
                _vao.UpdateVertexBufferObject(_colorsVboIndex, colors);
                _isTimeToUpdateVbo = false;
            }
        }

        public void Draw(in Camera camera)
        {
            if (IsVisible)
            {
                if (_isXzPlaneVisible || _isXyPlaneVisible || _isYzPlaneVisible)
                {
                    UpdateVbo();

                    _shaderProgram.SetMatrix4("view", camera.ViewMatrix, false);
                    _shaderProgram.SetMatrix4("projection", camera.ProjectionMatrix, false);
                    _shaderProgram.SetVector3("pivot.position", Pivot.Position);
                    _shaderProgram.SetVector4("pivot.rotation", new Vector4(Pivot.Rotation.Xyz, Pivot.Rotation.W));

                    var prevLineWidth = GL.GetFloat(GetPName.LineWidth);

                    GL.LineWidth(_lineWidth);
                    _vao.Draw(_vertexesAmount, PrimitiveType.Lines);
                    GL.LineWidth(prevLineWidth);
                }

                Pivot.Draw(camera);
            }
        }
        
        public void DeleteVao()
        {
            Pivot.DeleteVao();
            _vao.Delete();
        }
    }
}