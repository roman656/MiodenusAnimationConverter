namespace MiodenusAnimationConverter.Scene.Cameras;

using System.Globalization;
using Shaders;
using NLog;
using OpenTK.Graphics.OpenGL;
using OpenTK.Mathematics;

public class Camera
{
    private const float FovMinValue = 1.0f;
    private const float FovMaxValue = 180.0f;
    private const float ProjectionVolumeMinValue = 0.001f;
    private static readonly Logger Logger = LogManager.GetCurrentClassLogger();
    private readonly Pivot _pivot;
    private int _viewportWidth;
    private int _viewportHeight;
    private float _viewportAspectRatio;
    private float _fov = MathHelper.PiOver3;    // Угол поля зрения в направлении оси OY (в радианах).
    private float _distanceToTheNearClipPlane = 0.01f;
    private float _distanceToTheFarClipPlane = 100.0f;
    private float _projectionVolume = 10.0f;
    private Matrix4 _view;
    private Matrix4 _projection;
    private ProjectionTypeEnum _projectionType = ProjectionTypeEnum.Perspective;
    private VertexArrayObject _vao;
    public bool IsVisibleInDebug = true;

    private int _fovVboIndex;
    private int _distanceToTheNearClipPlaneVboIndex;
    private int _distanceToTheFarClipPlaneVboIndex;
    private int _viewDirectionVboIndex;
    private int _rightDirectionVboIndex;
    private int _upDirectionVboIndex;
    private int _positionVboIndex;

    public Camera(Vector3 position, int viewportWidth = 1, int viewportHeight = 1)
    {
        _pivot = new Pivot(position);
        ViewportWidth = viewportWidth;
        ViewportHeight = viewportHeight;
        UpdateViewMatrix();
        UpdateProjectionMatrix();
    }

    public Camera(int viewportWidth = 1, int viewportHeight = 1) :
            this(Vector3.Zero, viewportWidth, viewportHeight) {}
        
    public void Reset()
    {
        _pivot.ResetLocalRotation();
        _fov = MathHelper.PiOver3;
        _projectionVolume = 10.0f;
        _distanceToTheNearClipPlane = 0.01f;
        _distanceToTheFarClipPlane = 100.0f;
        UpdateViewMatrix();
        UpdateProjectionMatrix();
    }

    public void ResetLocalRotation()
    {
        _pivot.ResetLocalRotation();
        UpdateViewMatrix();
    }

    public int ViewportWidth
    {
        get => _viewportWidth;
        set
        {
            if (value > 0)
            {
                _viewportWidth = value;
                UpdateViewportAspectRatio();
                UpdateProjectionMatrix();
            }
            else
            {
                Logger.Warn("Wrong value for ViewportWidth parameter. Expected: value greater than 0. Got:"
                        + $" {value}. Viewport width was not changed.");
            }
        }
    }
        
    public int ViewportHeight
    {
        get => _viewportHeight;
        set
        {
            if (value > 0)
            {
                _viewportHeight = value;
                UpdateViewportAspectRatio();
                UpdateProjectionMatrix();
            }
            else
            {
                Logger.Warn("Wrong value for ViewportHeight parameter. Expected: value greater than 0. Got:"
                        + $" {value}. Viewport height was not changed.");
            }
        }
    }

    public float Fov
    {
        get => MathHelper.RadiansToDegrees(_fov);
        set
        {
            if (value >= FovMinValue && value <= FovMaxValue)
            {
                _fov = MathHelper.DegreesToRadians(value);
                UpdateProjectionMatrix();
            }
            else
            {
                Logger.Warn("Wrong value for Fov parameter. Expected: minimum value"
                        + $" is {FovMinValue}; maximum value is {FovMaxValue}. Got: {value}. FOV was not changed.");
            }
        }
    }

    public float DistanceToTheNearClipPlane
    {
        get => _distanceToTheNearClipPlane;
        set
        {
            if ((value > 0.0f) && (value < _distanceToTheFarClipPlane))
            {
                _distanceToTheNearClipPlane = value;
                UpdateProjectionMatrix();
            }
            else
            {
                Logger.Warn("Wrong value for DistanceToTheNearClipPlane parameter. Expected: value"
                        + $" greater than 0 and less than {_distanceToTheFarClipPlane} (distance to the far"
                        + $" clip plane). Got: {value}. Distance to the near clip plane was not changed.");
            }
        }
    }
        
    public float DistanceToTheFarClipPlane
    {
        get => _distanceToTheFarClipPlane;
        set
        {
            if (value > _distanceToTheNearClipPlane)
            {
                _distanceToTheFarClipPlane = value;
                UpdateProjectionMatrix();
            }
            else
            {
                Logger.Warn("Wrong value for DistanceToTheFarClipPlane parameter. Expected: value"
                        + $" greater than {_distanceToTheNearClipPlane} (distance to the near"
                        + $" clip plane). Got: {value}. Distance to the far clip plane was not changed.");
            }
        }
    }

    public ProjectionTypeEnum ProjectionType
    {
        get => _projectionType;
        set
        {
            _projectionType = value;
            UpdateProjectionMatrix();
        }
    }
        
    public float ProjectionVolume
    {
        get => _projectionVolume;
        set
        {
            if (value >= ProjectionVolumeMinValue)
            {
                _projectionVolume = value;
                UpdateProjectionMatrix();
            }
            else
            {
                Logger.Warn("Wrong value for ProjectionVolume parameter. Expected: value"
                        + $" greater than {ProjectionVolumeMinValue}. Got: {value}." 
                        + " Projection volume was not changed.");
            }
        }
    }

    private void UpdateViewMatrix() => _view = Matrix4.LookAt(_pivot.Position,
            _pivot.Position + _pivot.ZAxisPositiveDirection * -1, _pivot.YAxisPositiveDirection);
    private void UpdateProjectionMatrix() => _projection = _projectionType == ProjectionTypeEnum.Perspective
            ? Matrix4.CreatePerspectiveFieldOfView(_fov, _viewportAspectRatio, _distanceToTheNearClipPlane,
                    _distanceToTheFarClipPlane)
            : Matrix4.CreateOrthographic(_projectionVolume, _projectionVolume, _distanceToTheNearClipPlane,
                    _distanceToTheFarClipPlane);

    private void UpdateViewportAspectRatio() => _viewportAspectRatio = _viewportWidth / (float)_viewportHeight;
        
    public Matrix4 ViewMatrix => _view;
    public Matrix4 ProjectionMatrix => _projection;
    public Vector3 ViewDirection => _pivot.ZAxisPositiveDirection * -1;
    public Vector3 RightDirection => _pivot.XAxisPositiveDirection;
    public Vector3 UpDirection => _pivot.YAxisPositiveDirection;
        
    public Vector3 Position
    {
        get => _pivot.Position;
        set
        {
            _pivot.Position = value;
            UpdateViewMatrix();
        }
    }

    public void LookAt(in Vector3 target)
    {
        var rotationAxis = Vector3.Normalize(target - Position) + ViewDirection;
        RotateViewDirection(MathHelper.Pi, rotationAxis);
        RotateViewDirection(MathHelper.Pi, ViewDirection);
    }
        
    // Перемещение камеры, посредством указания напрпавления перемещения в определенной системе координат.
    public void Move(in Pivot pivot, float deltaX = 0.0f, float deltaY = 0.0f, float deltaZ = 0.0f)
    {
        _pivot.Move(pivot, deltaX, deltaY, deltaZ);
        UpdateViewMatrix();
    }

    public void GlobalMove(float deltaX = 0.0f, float deltaY = 0.0f, float deltaZ = 0.0f)
    {
        _pivot.GlobalMove(deltaX, deltaY, deltaZ);
        UpdateViewMatrix();
    }

    public void MoveInViewDirection(float delta)
    {
        _pivot.LocalMove(deltaZ: -delta);
        UpdateViewMatrix();
    }
        
    public void MoveInRightDirection(float delta)
    {
        _pivot.LocalMove(delta);
        UpdateViewMatrix();
    }
        
    public void MoveInUpDirection(float delta)
    {
        _pivot.LocalMove(deltaY: delta);
        UpdateViewMatrix();
    }

    public void Rotate(float angle, in Vector3 rotationVectorStartPoint, in Vector3 rotationVectorEndPoint)
    {
        _pivot.Rotate(angle, rotationVectorStartPoint, rotationVectorEndPoint);
        UpdateViewMatrix();
    }

    public void GlobalRotate(float angle, in Vector3 vector)
    {
        _pivot.GlobalRotate(angle, vector);
        UpdateViewMatrix();
    }

    public void RotateViewDirection(float angle, in Vector3 vector)
    {
        _pivot.LocalRotate(angle, vector);
        UpdateViewMatrix();
    }
        
    public void InitializeVao()
    {
        var fov = new[] { Fov };
        var distanceToTheNearClipPlane = new[] { DistanceToTheNearClipPlane };
        var distanceToTheFarClipPlane = new[] { DistanceToTheFarClipPlane };
        var viewDirection = new[] { ViewDirection.X, ViewDirection.Y, ViewDirection.Z };
        var rightDirection = new[] { RightDirection.X, RightDirection.Y, RightDirection.Z };
        var upDirection = new[] { UpDirection.X, UpDirection.Y, UpDirection.Z };
        var position = new[] { Position.X, Position.Y, Position.Z };

        _vao = new VertexArrayObject();
        _vao.AddVertexBufferObject(fov, 1, BufferUsageHint.DynamicDraw);
        _fovVboIndex = _vao.VertexBufferObjectIndexes[^1];
        _vao.AddVertexBufferObject(distanceToTheNearClipPlane, 1, BufferUsageHint.DynamicDraw);
        _distanceToTheNearClipPlaneVboIndex = _vao.VertexBufferObjectIndexes[^1];
        _vao.AddVertexBufferObject(distanceToTheFarClipPlane, 1, BufferUsageHint.DynamicDraw);
        _distanceToTheFarClipPlaneVboIndex = _vao.VertexBufferObjectIndexes[^1];
        _vao.AddVertexBufferObject(viewDirection, 3, BufferUsageHint.StreamDraw);
        _viewDirectionVboIndex = _vao.VertexBufferObjectIndexes[^1];
        _vao.AddVertexBufferObject(rightDirection, 3, BufferUsageHint.StreamDraw);
        _rightDirectionVboIndex = _vao.VertexBufferObjectIndexes[^1];
        _vao.AddVertexBufferObject(upDirection, 3, BufferUsageHint.StreamDraw);
        _upDirectionVboIndex = _vao.VertexBufferObjectIndexes[^1];
        _vao.AddVertexBufferObject(position, 3, BufferUsageHint.StreamDraw);
        _positionVboIndex = _vao.VertexBufferObjectIndexes[^1];
    }
        
    private void UpdateAllVbo()
    {
        var fov = new[] { Fov };
        var distanceToTheNearClipPlane = new[] { DistanceToTheNearClipPlane };
        var distanceToTheFarClipPlane = new[] { DistanceToTheFarClipPlane };
        var viewDirection = new[] { ViewDirection.X, ViewDirection.Y, ViewDirection.Z };
        var rightDirection = new[] { RightDirection.X, RightDirection.Y, RightDirection.Z };
        var upDirection = new[] { UpDirection.X, UpDirection.Y, UpDirection.Z };
        var position = new[] { Position.X, Position.Y, Position.Z };
            
        _vao.UpdateVertexBufferObject(_fovVboIndex, fov);
        _vao.UpdateVertexBufferObject(_distanceToTheNearClipPlaneVboIndex, distanceToTheNearClipPlane);
        _vao.UpdateVertexBufferObject(_distanceToTheFarClipPlaneVboIndex, distanceToTheFarClipPlane);
        _vao.UpdateVertexBufferObject(_viewDirectionVboIndex, viewDirection);
        _vao.UpdateVertexBufferObject(_rightDirectionVboIndex, rightDirection);
        _vao.UpdateVertexBufferObject(_upDirectionVboIndex, upDirection);
        _vao.UpdateVertexBufferObject(_positionVboIndex, position);
    }

    public void Draw(in ShaderProgram shaderProgram, in Camera currentCamera)
    {
        if (IsVisibleInDebug)
        {
            UpdateAllVbo();
            shaderProgram.SetMatrix4("view", currentCamera.ViewMatrix, false);
            shaderProgram.SetMatrix4("projection", currentCamera.ProjectionMatrix, false);

            _vao.Draw(1, PrimitiveType.Points);
        }
    }
        
    public void DeleteVao() => _vao.Delete();

    public override string ToString() => string.Format(CultureInfo.InvariantCulture,
            $"Camera:\n\t{_pivot}\n\tViewport width: {_viewportWidth}\n\tViewport height: {_viewportHeight}\n\t"
            + $"Viewport aspect ratio: {_viewportAspectRatio}\n\tFOV: {_fov}\n\t"
            + $"Distance to the near clip plane: {_distanceToTheNearClipPlane}\n\t"
            + $"Distance to the far clip plane: {_distanceToTheFarClipPlane}\n");
}