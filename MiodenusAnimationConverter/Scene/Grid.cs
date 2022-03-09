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
        private static readonly Color4 DefaultGridColor = Color4.DarkGray;
        private static readonly Color4 XAxisColor = Color4.Red;
        private static readonly Color4 YAxisColor = Color4.GreenYellow;
        private static readonly Color4 ZAxisColor = Color4.DeepSkyBlue;
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();
        public bool IsXyPlaneVisible = true;
        public bool IsYzPlaneVisible = true;
        public bool IsXzPlaneVisible = true;
        public bool IsCoordinateSystemVisible = true;
        private Vector3 _position;
        private float _cellSize;
        private int _xSizeInCells;
        private int _ySizeInCells;
        private int _zSizeInCells;
        private float _lineWidth;
        private Color4 _xyPlaneColor;
        private Color4 _xzPlaneColor;
        private Color4 _yzPlaneColor;
        private float _xAxisSize = 1.0f;
        private float _yAxisSize = 1.0f;
        private float _zAxisSize = 1.0f;
        private VertexArrayObject _gridVao;
        private ShaderProgram _gridShaderProgram;
        private VertexArrayObject _coordinateSystemVao;
        private ShaderProgram _coordinateSystemShaderProgram;
        private int _coordinateSystemVertexesVboIndex;
        private bool _wasParametersChanged;
        
        public Vector3 Position
        {
            get => _position;
            set
            {
                _position = value;
                _wasParametersChanged = true;
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
        
        public Color4 XzPlaneColor
        {
            get => _xzPlaneColor;
            set
            {
                _xzPlaneColor = value;
                _wasParametersChanged = true;
            }
        }
        
        public Color4 XyPlaneColor
        {
            get => _xyPlaneColor;
            set
            {
                _xyPlaneColor = value;
                _wasParametersChanged = true;
            }
        }
        
        public Color4 YzPlaneColor
        {
            get => _yzPlaneColor;
            set
            {
                _yzPlaneColor = value;
                _wasParametersChanged = true;
            }
        }

        public Grid(int xSizeInCells = 0, int ySizeInCells = 0, int zSizeInCells = 0, float cellSize = 1.0f,
                float lineWidth = 1.0f) : this(Vector3.Zero, DefaultGridColor, xSizeInCells, ySizeInCells,
                zSizeInCells, cellSize, lineWidth) {}
        
        public Grid(Vector3 position, Color4 color, int xSizeInCells = 0, int ySizeInCells = 0, int zSizeInCells = 0,
                float cellSize = 1.0f, float lineWidth = 1.0f) : this(position, color, color,
                color, xSizeInCells, ySizeInCells, zSizeInCells, cellSize, lineWidth) {}

        public Grid(Vector3 position, Color4 xyPlaneColor, Color4 xzPlaneColor, Color4 yzPlaneColor,
                int xSizeInCells = 0, int ySizeInCells = 0, int zSizeInCells = 0, float cellSize = 1.0f,
                float lineWidth = 1.0f)
        {
            _position = position;
            _cellSize = cellSize;
            _xSizeInCells = xSizeInCells;
            _ySizeInCells = ySizeInCells;
            _zSizeInCells = zSizeInCells;
            _lineWidth = lineWidth;
            _xyPlaneColor = xyPlaneColor;
            _xzPlaneColor = xzPlaneColor;
            _yzPlaneColor = yzPlaneColor;
        }
        
        private float[] CoordinateSystemVertexes => new []
        {
            _position.X, _position.Y, _position.Z,
            _position.X + _xAxisSize, _position.Y, _position.Z,
            _position.X, _position.Y, _position.Z,
            _position.X, _position.Y + _yAxisSize, _position.Z,
            _position.X, _position.Y, _position.Z,
            _position.X, _position.Y, _position.Z + _zAxisSize
        };

        private static float[] CoordinateSystemColors => new []
        {
            XAxisColor.R, XAxisColor.G, XAxisColor.B, XAxisColor.A,
            XAxisColor.R, XAxisColor.G, XAxisColor.B, XAxisColor.A,
            YAxisColor.R, YAxisColor.G, YAxisColor.B, YAxisColor.A,
            YAxisColor.R, YAxisColor.G, YAxisColor.B, YAxisColor.A,
            ZAxisColor.R, ZAxisColor.G, ZAxisColor.B, ZAxisColor.A,
            ZAxisColor.R, ZAxisColor.G, ZAxisColor.B, ZAxisColor.A
        };

        public void InitializeVao()
        {
            _gridVao = new VertexArrayObject();
            _coordinateSystemVao = new VertexArrayObject();
           // InitializeGridShaderProgram();
            InitializeCoordinateSystemShaderProgram();
            
            _coordinateSystemVao.AddVertexBufferObject(CoordinateSystemVertexes, 3, BufferUsageHint.StreamDraw);
            _coordinateSystemVertexesVboIndex = _coordinateSystemVao.VertexBufferObjectIndexes[^1];
            _coordinateSystemVao.AddVertexBufferObject(CoordinateSystemColors, ColorChannelsAmount);
        }
        
        private void InitializeGridShaderProgram()
        {
            var shaders = new List<Shader>
            {
                new (GridVertexShader.Code, GridVertexShader.Type),
                new (GridFragmentShader.Code, GridFragmentShader.Type)
            };

            _gridShaderProgram = new ShaderProgram(shaders);

            for (var i = 0; i < shaders.Count; i++)
            {
                shaders[i].Delete();
            }
        }
        
        private void InitializeCoordinateSystemShaderProgram()
        {
            var shaders = new List<Shader>
            {
                new (CoordinateSystemVertexShader.Code, CoordinateSystemVertexShader.Type),
                new (CoordinateSystemFragmentShader.Code, CoordinateSystemFragmentShader.Type)
            };

            _coordinateSystemShaderProgram = new ShaderProgram(shaders);

            for (var i = 0; i < shaders.Count; i++)
            {
                shaders[i].Delete();
            }
        }
        
        private void UpdateVbo()
        {
            if (_wasParametersChanged)
            {
                _coordinateSystemVao.UpdateVertexBufferObject(_coordinateSystemVertexesVboIndex, CoordinateSystemVertexes);
                _wasParametersChanged = false;
            }
        }

        public void Draw(in Camera camera)
        {
            if (IsCoordinateSystemVisible)
            {
                UpdateVbo();
                
                _coordinateSystemShaderProgram.SetMatrix4("view", camera.ViewMatrix, false);
                _coordinateSystemShaderProgram.SetMatrix4("projection", camera.ProjectionMatrix, false);
                
                var prevLineWidth = GL.GetFloat(GetPName.LineWidth);
                
                GL.LineWidth(_lineWidth);
                _coordinateSystemVao.Draw(6, PrimitiveType.Lines);
                GL.LineWidth(prevLineWidth);
            }
            /*
            if (IsXzPlaneVisible || IsXyPlaneVisible || IsYzPlaneVisible)
            {
                if (_wasParametersChanged)
                {
                    //UpdateUniform();
                }

                _gridShaderProgram.SetMatrix4("view", camera.ViewMatrix, false);
                _gridShaderProgram.SetMatrix4("projection", camera.ProjectionMatrix, false);

                var prevLineWidth = GL.GetFloat(GetPName.LineWidth);
                
                GL.LineWidth(_lineWidth);
                _gridVao.Draw(1, PrimitiveType.Points);
                GL.LineWidth(prevLineWidth);
            }*/
        }
        
        public void DeleteVao()
        {
            _gridVao.Delete();
            _coordinateSystemVao.Delete();
        }
    }
}