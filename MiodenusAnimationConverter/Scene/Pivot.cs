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
        private const int ColorChannelsAmount = 4;
        private static readonly Color4 XAxisColor = Color4.Red;
        private static readonly Color4 YAxisColor = Color4.GreenYellow;
        private static readonly Color4 ZAxisColor = Color4.DeepSkyBlue;
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();
        private Vector3 _xAxisPositiveDirection = Vector3.UnitX;
        private Vector3 _yAxisPositiveDirection = Vector3.UnitY;
        private Vector3 _zAxisPositiveDirection = Vector3.UnitZ;
        private VertexArrayObject _vao;
        private ShaderProgram _shaderProgram;
        private bool _wasParametersChanged;
        private int _vertexesVboIndex;
        private Quaternion _rotation = Quaternion.Identity;
        public Vector3 Position;
        public float _lineWidth = 3.0f;
        private float _xAxisSize = 1.0f;
        private float _yAxisSize = 1.0f;
        private float _zAxisSize = 1.0f;
        public bool IsVisible = true;

        public Pivot(Vector3 position)
        {
            Position = position;
        }
        
        public Pivot() : this(Vector3.Zero) {}

        public Pivot(Pivot pivot)
        {
            Position = pivot.Position;
            _xAxisPositiveDirection = pivot.XAxisPositiveDirection;
            _yAxisPositiveDirection = pivot.YAxisPositiveDirection;
            _zAxisPositiveDirection = pivot.ZAxisPositiveDirection;
        }
        
        public Vector3 XAxisPositiveDirection => _xAxisPositiveDirection;
        public Vector3 YAxisPositiveDirection => _yAxisPositiveDirection;
        public Vector3 ZAxisPositiveDirection => _zAxisPositiveDirection;

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
            if (_wasParametersChanged)
            {
                _vao.UpdateVertexBufferObject(_vertexesVboIndex, Vertexes);
                _wasParametersChanged = false;
            }
        }

        public Quaternion Rotation
        {
            get
            {
                Quaternion xRotation, yRotation, zRotation, result;
                
                xRotation.Xyz = Vector3.Cross(Vector3.UnitX, _xAxisPositiveDirection);
                xRotation.W = (float)Math.Sqrt((Vector3.UnitX.Length * Vector3.UnitX.Length) * (_xAxisPositiveDirection.Length * _xAxisPositiveDirection.Length)) + Vector3.Dot(Vector3.UnitX, _xAxisPositiveDirection);
                xRotation.Normalize();
                
                yRotation.Xyz = Vector3.Cross(Vector3.UnitY, _yAxisPositiveDirection);
                yRotation.W = (float)Math.Sqrt((Vector3.UnitY.Length * Vector3.UnitY.Length) * (_yAxisPositiveDirection.Length * _yAxisPositiveDirection.Length)) + Vector3.Dot(Vector3.UnitY, _yAxisPositiveDirection);
                yRotation.Normalize();
                
                zRotation.Xyz = Vector3.Cross(Vector3.UnitZ, _zAxisPositiveDirection);
                zRotation.W = (float)Math.Sqrt((Vector3.UnitZ.Length * Vector3.UnitZ.Length) * (_zAxisPositiveDirection.Length * _zAxisPositiveDirection.Length)) + Vector3.Dot(Vector3.UnitZ, _zAxisPositiveDirection);
                zRotation.Normalize();

                if (xRotation == yRotation && yRotation == zRotation)
                {
                    result = xRotation;
                }
                else if (xRotation == yRotation)
                {
                    result = yRotation * zRotation;
                }
                else if (xRotation == zRotation)
                {
                    result = xRotation * yRotation;
                }
                else
                {
                    result = xRotation * yRotation * zRotation;
                }
                
                return result.Normalized();
            }
        }

        public void Draw(in Camera camera)
        {
            if (IsVisible)
            {
                UpdateVbo();
                
                _shaderProgram.SetMatrix4("view", camera.ViewMatrix, false);
                _shaderProgram.SetMatrix4("projection", camera.ProjectionMatrix, false);
                _shaderProgram.SetVector3("pivot.position", Position); //добавить проверку нужно ли обновлять данные
                /*_shaderProgram.SetVector3("pivot.x_axis_positive_direction", _xAxisPositiveDirection);
                _shaderProgram.SetVector3("pivot.y_axis_positive_direction", _yAxisPositiveDirection);
                _shaderProgram.SetVector3("pivot.z_axis_positive_direction", _zAxisPositiveDirection);*/

                var tmp = _rotation;
                var rotation = new Vector4(tmp.Xyz, tmp.W);
                _shaderProgram.SetVector4("pivot.rotation", rotation);
                    
                var prevLineWidth = GL.GetFloat(GetPName.LineWidth);
                
                GL.LineWidth(_lineWidth);
                _vao.Draw(6, PrimitiveType.Lines);
                GL.LineWidth(prevLineWidth);
            }
        }

        public void DeleteVao() => _vao.Delete();

        public void ResetLocalRotation()
        {
            _xAxisPositiveDirection = Vector3.UnitX;
            _yAxisPositiveDirection = Vector3.UnitY;
            _zAxisPositiveDirection = Vector3.UnitZ;
        }

        // Перемещение данной системы координат, посредством указания напрпавления перемещения в другой СК.
        public void Move(in Pivot pivot, float deltaX = 0.0f, float deltaY = 0.0f, float deltaZ = 0.0f)
        {
            Position.X += pivot.XAxisPositiveDirection.X * deltaX + pivot.YAxisPositiveDirection.X * deltaY 
                    + pivot.ZAxisPositiveDirection.X * deltaZ;
            Position.Y += pivot.XAxisPositiveDirection.Y * deltaX + pivot.YAxisPositiveDirection.Y * deltaY 
                    + pivot.ZAxisPositiveDirection.Y * deltaZ;
            Position.Z += pivot.XAxisPositiveDirection.Z * deltaX + pivot.YAxisPositiveDirection.Z * deltaY 
                    + pivot.ZAxisPositiveDirection.Z * deltaZ;
        }
        
        public void GlobalMove(float deltaX = 0.0f, float deltaY = 0.0f, float deltaZ = 0.0f)
        {
            Position.X += deltaX;
            Position.Y += deltaY;
            Position.Z += deltaZ;
        }

        public void LocalMove(float deltaX = 0.0f, float deltaY = 0.0f, float deltaZ = 0.0f)
        {
            Position.X += _xAxisPositiveDirection.X * deltaX + _yAxisPositiveDirection.X * deltaY
                    + _zAxisPositiveDirection.X * deltaZ;
            Position.Y += _xAxisPositiveDirection.Y * deltaX + _yAxisPositiveDirection.Y * deltaY
                    + _zAxisPositiveDirection.Y * deltaZ;
            Position.Z += _xAxisPositiveDirection.Z * deltaX + _yAxisPositiveDirection.Z * deltaY
                    + _zAxisPositiveDirection.Z * deltaZ;
        }
        
        public void Rotate(float angle, in Vector3 rotationVectorStartPoint, in Vector3 rotationVectorEndPoint)
        {
            var rotation = Quaternion.FromAxisAngle(rotationVectorEndPoint - rotationVectorStartPoint, angle);
            Position = rotation * (Position - rotationVectorStartPoint) + rotationVectorStartPoint;
        }

        public void GlobalRotate(float angle, in Vector3 vector)
        {
            Position = Quaternion.FromAxisAngle(vector, angle) * Position;
        }
        
        public void LocalRotate(float angle, in Vector3 vector)
        {
            var rotation = Quaternion.FromAxisAngle(vector, angle);
            _rotation = rotation * _rotation;
            
            _xAxisPositiveDirection = Vector3.Normalize(rotation * _xAxisPositiveDirection);
            _zAxisPositiveDirection = Vector3.Normalize(rotation * _zAxisPositiveDirection);
            _yAxisPositiveDirection = Vector3.Normalize(Vector3.Cross(_zAxisPositiveDirection,
                    _xAxisPositiveDirection));
        }
        
        public override string ToString()
        {
            return string.Format(CultureInfo.InvariantCulture, $"Pivot: [ Position: ({Position.X}; {Position.Y};"
                    + $" {Position.Z}) | X axis positive direction: ({_xAxisPositiveDirection.X};"
                    + $" {_xAxisPositiveDirection.Y}; {_xAxisPositiveDirection.Z}) | Y axis positive direction: "
                    + $"({_yAxisPositiveDirection.X}; {_yAxisPositiveDirection.Y}; {_yAxisPositiveDirection.Z}) "
                    + $"| Z axis positive direction: ({_zAxisPositiveDirection.X}; {_zAxisPositiveDirection.Y}; "
                    + $"{_zAxisPositiveDirection.Z}) ]");
        }

        public object Clone()
        {
            return new Pivot(this);
        }
    }
}