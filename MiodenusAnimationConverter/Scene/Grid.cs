using System.Collections.Generic;
using MiodenusAnimationConverter.Scene.Cameras;
using MiodenusAnimationConverter.Shaders;
using MiodenusAnimationConverter.Shaders.FragmentShaders;
using MiodenusAnimationConverter.Shaders.VertexShaders;
using NLog;
using OpenTK.Mathematics;

namespace MiodenusAnimationConverter.Scene
{
    public class Grid
    {
        private const int ColorChannelsAmount = 4;
        private static readonly Color4 DefaultGridColor = Color4.DarkGray;
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();
        public bool IsXyPlaneVisible = true;
        public bool IsYzPlaneVisible = true;
        public bool IsXzPlaneVisible = true;
        private Vector3 _position;
        private float _cellSize;
        private int _xSizeInCells;
        private int _ySizeInCells;
        private int _zSizeInCells;
        private float _lineWidth;
        private Color4 _xyPlaneColor;
        private Color4 _xzPlaneColor;
        private Color4 _yzPlaneColor;
        private VertexArrayObject _gridVao;
        private ShaderProgram _gridShaderProgram;
        private bool _wasParametersChanged;
        public Pivot _pivot = new (new Vector3(1,0,0));
        
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

        public void InitializeVao()
        {
            _gridVao = new VertexArrayObject();
            // InitializeGridShaderProgram();
            //_pivot.Rotate(MathHelper.DegreesToRadians(45.0f), new Vector3(1.0f, 0.0f, 0.0f), new Vector3(1.0f, 1.0f, 0.0f));
            _pivot.InitializeVao();
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

        public void Draw(in Camera camera)
        {
            _pivot.Draw(camera);
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
            _pivot.DeleteVao();
        }
    }
}