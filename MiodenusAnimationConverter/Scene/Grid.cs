using System.Collections.Generic;
using MiodenusAnimationConverter.Scene.Cameras;
using MiodenusAnimationConverter.Shaders;
using MiodenusAnimationConverter.Shaders.FragmentShaders;
using MiodenusAnimationConverter.Shaders.GeometryShaders;
using MiodenusAnimationConverter.Shaders.VertexShaders;
using NLog;
using OpenTK.Graphics.OpenGL;
using OpenTK.Mathematics;

namespace MiodenusAnimationConverter.Scene
{
    public class Grid
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();
        public bool IsXyPlaneVisible = true;
        public bool IsYzPlaneVisible = true;
        public bool IsXzPlaneVisible = true;
        public bool IsCoordinateSystemVisible = true;
        public Vector3 Position;
        private float _cellSize;
        private int _xSizeInCells;
        private int _ySizeInCells;
        private int _zSizeInCells;
        private float _lineWidth;
        private Color4 _xyPlaneColor;
        private Color4 _xzPlaneColor;
        private Color4 _yzPlaneColor;
        private VertexArrayObject _vao;
        private ShaderProgram _shaderProgram;
        private bool _wasParametersChanged;
        private float _colorRotationSpeed = 0.03f;
        private float time = 0.0f;

        public float CellSize
        {
            get => _cellSize;
            set
            {
                if (value > 0.0f)
                {
                    _cellSize = value;
                    _wasParametersChanged = true;
                }
                else
                {
                    Logger.Warn("Wrong value for CellSize parameter. Expected: value"
                            + $" greater than 0. Got: {value}. Cell size was not changed.");
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
                    _wasParametersChanged = true;
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
                    _wasParametersChanged = true;
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
                    _wasParametersChanged = true;
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
                    _wasParametersChanged = true;
                }
                else
                {
                    Logger.Warn("Wrong value for LineWidth parameter. Expected: value"
                                + $" greater than 0. Got: {value}. Line width was not changed.");
                }
            }
        }

        public Grid(int xSizeInCells = 0, int ySizeInCells = 0, int zSizeInCells = 0, float cellSize = 1.0f,
                float lineWidth = 1.0f) : this(Vector3.Zero, Color4.DarkGray, xSizeInCells, ySizeInCells,
                zSizeInCells, cellSize, lineWidth) {}
        
        public Grid(Vector3 position, Color4 color, int xSizeInCells = 0, int ySizeInCells = 0, int zSizeInCells = 0,
                float cellSize = 1.0f, float lineWidth = 1.0f) : this(position, color, color,
                color, xSizeInCells, ySizeInCells, zSizeInCells, cellSize, lineWidth) {}

        public Grid(Vector3 position, Color4 xyPlaneColor, Color4 xzPlaneColor, Color4 yzPlaneColor,
                int xSizeInCells = 0, int ySizeInCells = 0, int zSizeInCells = 0, float cellSize = 1.0f,
                float lineWidth = 1.0f)
        {
            Position = position;
            _cellSize = cellSize;
            _xSizeInCells = xSizeInCells;
            _ySizeInCells = ySizeInCells;
            _zSizeInCells = zSizeInCells;
            _lineWidth = lineWidth;
            _xyPlaneColor = xyPlaneColor;
            _xzPlaneColor = xzPlaneColor;
            _yzPlaneColor = yzPlaneColor;
        }
        
        public void InitializeVao()
        {
            _vao = new VertexArrayObject();
            InitializeShaderProgram();
            UpdateUniform();
            _shaderProgram.SetVector2("resolution", new Vector2(800, 800));
        }
        
        private void InitializeShaderProgram()
        {
            var shaders = new List<Shader>
            {
                new (GridVertexShader.Code, GridVertexShader.Type),
                new (GridGeometryShader.Code, GridGeometryShader.Type),
                new (GridFragmentShader.Code, GridFragmentShader.Type)
            };

            _shaderProgram = new ShaderProgram(shaders);

            for (var i = 0; i < shaders.Count; i++)
            {
                shaders[i].Delete();
            }
        }
        
        private void UpdateUniform()
        {
            _shaderProgram.SetVector3("grid.position", Position);
            _shaderProgram.SetFloat("grid.cell_size", _cellSize);
            _shaderProgram.SetInt("grid.x_size_in_cells", IsXyPlaneVisible || IsXzPlaneVisible ? _xSizeInCells : 0);
            _shaderProgram.SetInt("grid.y_size_in_cells", IsXyPlaneVisible || IsYzPlaneVisible ? _ySizeInCells : 0);
            _shaderProgram.SetInt("grid.z_size_in_cells", IsXzPlaneVisible || IsYzPlaneVisible ? _zSizeInCells : 0);
            _shaderProgram.SetColor4("grid.xy_plane_color", _xyPlaneColor);
            _shaderProgram.SetColor4("grid.xz_plane_color", _xzPlaneColor);
            _shaderProgram.SetColor4("grid.yz_plane_color", _yzPlaneColor);
            _shaderProgram.SetBool("use_vertex_color", false);
            _shaderProgram.SetFloat("color_rotation_speed", _colorRotationSpeed);
        }

        public void Draw(in Camera camera)
        {
            if (IsXzPlaneVisible || IsXyPlaneVisible || IsYzPlaneVisible)
            {
                if (_wasParametersChanged)
                {
                    UpdateUniform();
                }

                _shaderProgram.SetMatrix4("view", camera.ViewMatrix, false);
                _shaderProgram.SetMatrix4("projection", camera.ProjectionMatrix, false);
                _shaderProgram.SetFloat("time", time);
                time += 1.0f;

                var prevLineWidth = GL.GetFloat(GetPName.LineWidth);
                
                GL.LineWidth(_lineWidth);
                _vao.Draw(1, PrimitiveType.Points);
                GL.LineWidth(prevLineWidth);
            }
        }
        
        public void DeleteVao() => _vao.Delete();
    }
}