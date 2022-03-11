using System;
using System.Collections.Generic;
using System.Globalization;
using MiodenusAnimationConverter.Scene.Cameras;
using MiodenusAnimationConverter.Shaders;
using MiodenusAnimationConverter.Shaders.FragmentShaders;
using MiodenusAnimationConverter.Shaders.VertexShaders;
using NLog;
using OpenTK.Graphics.OpenGL;
using OpenTK.Mathematics;

namespace MiodenusAnimationConverter.Scene
{
    public class Pivot : ICloneable
    {
        private const int VertexesAmount = 6;
        private const int ColorChannelsAmount = 4;
        private static readonly Color4 XAxisColor = Color4.Red;
        private static readonly Color4 YAxisColor = Color4.GreenYellow;
        private static readonly Color4 ZAxisColor = Color4.DeepSkyBlue;
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();
        private Vector3 _xAxisPositiveDirection = Vector3.UnitX;
        private Vector3 _yAxisPositiveDirection = Vector3.UnitY;
        private Vector3 _zAxisPositiveDirection = Vector3.UnitZ;
        private Quaternion _rotation = Quaternion.Identity;
        private VertexArrayObject _vao;
        private ShaderProgram _shaderProgram;
        private bool _isTimeToUpdateVbo;
        private bool _isTimeToUpdateUniform = true;
        private int _vertexesVboIndex;
        private Vector3 _position;
        private float _lineWidth;
        private float _xAxisSize;
        private float _yAxisSize;
        private float _zAxisSize;
        public bool IsVisible = true;

        public Pivot(Vector3 position, float lineWidth = 1.0f, float xAxisSize = 1.0f, float yAxisSize = 1.0f,
                float zAxisSize = 1.0f)
        {
            _position = position;
            _lineWidth = lineWidth;
            _xAxisSize = xAxisSize;
            _yAxisSize = yAxisSize;
            _zAxisSize = zAxisSize;
        }

        public Pivot(float lineWidth = 1.0f, float xAxisSize = 1.0f, float yAxisSize = 1.0f, float zAxisSize = 1.0f) :
                this(Vector3.Zero, lineWidth, xAxisSize, yAxisSize, zAxisSize) {}
        
        public Pivot(in Pivot pivot)
        {
            _position = pivot.Position;
            _rotation = pivot.Rotation;
            _xAxisPositiveDirection = pivot.XAxisPositiveDirection;
            _yAxisPositiveDirection = pivot.YAxisPositiveDirection;
            _zAxisPositiveDirection = pivot.ZAxisPositiveDirection;
            _lineWidth = pivot.LineWidth;
            _xAxisSize = pivot.XAxisSize;
            _yAxisSize = pivot.YAxisSize;
            _zAxisSize = pivot.ZAxisSize;
        }
        
        public Vector3 XAxisPositiveDirection => _xAxisPositiveDirection;
        public Vector3 YAxisPositiveDirection => _yAxisPositiveDirection;
        public Vector3 ZAxisPositiveDirection => _zAxisPositiveDirection;
        public Quaternion Rotation => _rotation;
        
        public Vector3 Position
        {
            get => _position;
            set
            {
                _position = value;
                _isTimeToUpdateUniform = true;
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

        public float XAxisSize
        {
            get => _xAxisSize;
            set
            {
                _xAxisSize = value;
                _isTimeToUpdateVbo = true;
            }
        }
        
        public float YAxisSize
        {
            get => _yAxisSize;
            set
            {
                _yAxisSize = value;
                _isTimeToUpdateVbo = true;
            }
        }
        
        public float ZAxisSize
        {
            get => _zAxisSize;
            set
            {
                _zAxisSize = value;
                _isTimeToUpdateVbo = true;
            }
        }
        
        public float AxesSize
        {
            set
            {
                _xAxisSize = value;
                _yAxisSize = value;
                _zAxisSize = value;
                _isTimeToUpdateVbo = true;
            }
        }
        
        private float[] Vertexes => new []
        {
            0.0f, 0.0f, 0.0f,
            _xAxisSize, 0.0f, 0.0f,
            0.0f,  0.0f, 0.0f,
            0.0f, _yAxisSize, 0.0f,
            0.0f, 0.0f, 0.0f,
            0.0f, 0.0f, _zAxisSize
        };

        private static float[] Colors => new []
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
            _vao = new VertexArrayObject();
            InitializeShaderProgram();
            
            _vao.AddVertexBufferObject(Vertexes, 3, BufferUsageHint.StreamDraw);
            _vertexesVboIndex = _vao.VertexBufferObjectIndexes[^1];
            _vao.AddVertexBufferObject(Colors, ColorChannelsAmount);
        }
        
        private void InitializeShaderProgram()
        {
            var shaders = new List<Shader>
            {
                new (PivotVertexShader.Code, PivotVertexShader.Type),
                new (PivotFragmentShader.Code, PivotFragmentShader.Type)
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
                _vao.UpdateVertexBufferObject(_vertexesVboIndex, Vertexes);
                _isTimeToUpdateVbo = false;
            }
        }

        private void UpdateUniform()
        {
            if (_isTimeToUpdateUniform)
            {
                _shaderProgram.SetVector3("pivot.position", _position);
                _shaderProgram.SetVector4("pivot.rotation", new Vector4(_rotation.Xyz, _rotation.W));
                _isTimeToUpdateUniform = false;
            }
        }

        public void Draw(in Camera camera)
        {
            if (IsVisible)
            {
                UpdateVbo();
                UpdateUniform();
                
                _shaderProgram.SetMatrix4("view", camera.ViewMatrix, false);
                _shaderProgram.SetMatrix4("projection", camera.ProjectionMatrix, false);

                var prevLineWidth = GL.GetFloat(GetPName.LineWidth);
                
                GL.LineWidth(_lineWidth);
                _vao.Draw(VertexesAmount, PrimitiveType.Lines);
                GL.LineWidth(prevLineWidth);
            }
        }

        public void DeleteVao() => _vao.Delete();

        public void ResetLocalRotation()
        {
            _xAxisPositiveDirection = Vector3.UnitX;
            _yAxisPositiveDirection = Vector3.UnitY;
            _zAxisPositiveDirection = Vector3.UnitZ;
            _rotation = Quaternion.Identity;
            _isTimeToUpdateUniform = true;
        }

        // Перемещение данной системы координат, посредством указания напрпавления перемещения в другой СК.
        public void Move(in Pivot pivot, float deltaX = 0.0f, float deltaY = 0.0f, float deltaZ = 0.0f)
        {
            _position.X += pivot.XAxisPositiveDirection.X * deltaX + pivot.YAxisPositiveDirection.X * deltaY 
                    + pivot.ZAxisPositiveDirection.X * deltaZ;
            _position.Y += pivot.XAxisPositiveDirection.Y * deltaX + pivot.YAxisPositiveDirection.Y * deltaY 
                    + pivot.ZAxisPositiveDirection.Y * deltaZ;
            _position.Z += pivot.XAxisPositiveDirection.Z * deltaX + pivot.YAxisPositiveDirection.Z * deltaY 
                    + pivot.ZAxisPositiveDirection.Z * deltaZ;
            _isTimeToUpdateUniform = true;
        }
        
        public void GlobalMove(float deltaX = 0.0f, float deltaY = 0.0f, float deltaZ = 0.0f)
        {
            _position.X += deltaX;
            _position.Y += deltaY;
            _position.Z += deltaZ;
            _isTimeToUpdateUniform = true;
        }

        public void LocalMove(float deltaX = 0.0f, float deltaY = 0.0f, float deltaZ = 0.0f)
        {
            _position.X += _xAxisPositiveDirection.X * deltaX + _yAxisPositiveDirection.X * deltaY
                    + _zAxisPositiveDirection.X * deltaZ;
            _position.Y += _xAxisPositiveDirection.Y * deltaX + _yAxisPositiveDirection.Y * deltaY
                    + _zAxisPositiveDirection.Y * deltaZ;
            _position.Z += _xAxisPositiveDirection.Z * deltaX + _yAxisPositiveDirection.Z * deltaY 
                    + _zAxisPositiveDirection.Z * deltaZ;
            _isTimeToUpdateUniform = true;
        }
        
        public void Rotate(float angle, in Vector3 rotationVectorStartPoint, in Vector3 rotationVectorEndPoint)
        {
            var rotation = Quaternion.FromAxisAngle(rotationVectorEndPoint - rotationVectorStartPoint, angle);
            _position = rotation * (_position - rotationVectorStartPoint) + rotationVectorStartPoint;
            _isTimeToUpdateUniform = true;
        }

        public void GlobalRotate(float angle, in Vector3 vector)
        {
            _position = Quaternion.FromAxisAngle(vector, angle) * _position;
            _isTimeToUpdateUniform = true;
        }
        
        public void LocalRotate(float angle, in Vector3 vector)
        {
            var rotation = Quaternion.FromAxisAngle(vector, angle);

            _rotation = rotation * _rotation;
            _isTimeToUpdateUniform = true;
            _xAxisPositiveDirection = Vector3.Normalize(rotation * _xAxisPositiveDirection);
            _zAxisPositiveDirection = Vector3.Normalize(rotation * _zAxisPositiveDirection);
            _yAxisPositiveDirection = Vector3.Normalize(Vector3.Cross(_zAxisPositiveDirection,
                    _xAxisPositiveDirection));
        }
        
        public override string ToString()
        {
            return string.Format(CultureInfo.InvariantCulture, $"Pivot:\n\tPosition: ({_position.X}; {_position.Y};"
                    + $" {_position.Z})\n\tX axis positive direction: ({_xAxisPositiveDirection.X};"
                    + $" {_xAxisPositiveDirection.Y}; {_xAxisPositiveDirection.Z})\n\tY axis positive direction: "
                    + $"({_yAxisPositiveDirection.X}; {_yAxisPositiveDirection.Y}; {_yAxisPositiveDirection.Z})\n\t"
                    + $"Z axis positive direction: ({_zAxisPositiveDirection.X}; {_zAxisPositiveDirection.Y}; "
                    + $"{_zAxisPositiveDirection.Z})\n\tRotation: ({_rotation.X}; {_rotation.Y}; {_rotation.Z};"
                    + $" {_rotation.W})\n\tLine width: {_lineWidth}\n\tX axis size: {_xAxisSize}\n\tY axis size:"
                    + $" {_yAxisSize}\n\tZ axis size: {_zAxisSize}\n");
        }

        public object Clone()
        {
            return new Pivot(this);
        }
    }
}