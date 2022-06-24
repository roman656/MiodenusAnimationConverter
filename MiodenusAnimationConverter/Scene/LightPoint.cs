namespace MiodenusAnimationConverter.Scene;

using Shaders;
using OpenTK.Mathematics;

public class LightPoint
{
    private static int _lightPointsAmount;
    private readonly int _index;
        
    private bool _isActive = true;
    public Vector3 Position;
    private Color4 _color;

    private bool _useAttenuation = true;
    private float _constantFactor = 1.0f;
    private float _linearFactor = 0.09f;
    private float _quadraticFactor = 0.032f;
        
    private Color4 _ambientComponent;
    private float _ambientStrength = 0.3f;
    private float _diffuseStrength = 1.0f;
    private float _specularStrength = 0.8f;

    public LightPoint(Vector3 position, Color4 color)
    {
        _index = _lightPointsAmount;
        _lightPointsAmount++;
        Position = position;
        Color = color;
    }

    private void UpdateAmbientComponent()
    {
        _ambientComponent.R = _ambientStrength * _color.R;
        _ambientComponent.G = _ambientStrength * _color.G;
        _ambientComponent.B = _ambientStrength * _color.B;
        _ambientComponent.A = _ambientStrength * _color.A;
    }

    public Color4 Color
    {
        get => _color;

        set
        {
            _color = value;
            UpdateAmbientComponent();
        }
    }

    public float AmbientStrength
    {
        get => _ambientStrength;

        set
        {
            _ambientStrength = value;
            UpdateAmbientComponent();
        }
    }

    public void Enable() => _isActive = true;
    public void Disable() => _isActive = false;
    public void EnableAttenuation() => _useAttenuation = true;
    public void DisableAttenuation() => _useAttenuation = false;

    public void Move(float deltaX, float deltaY, float deltaZ)
    {
        Position.X += deltaX;
        Position.Y += deltaY;
        Position.Z += deltaZ;
    }

    public void Rotate(float angle, Vector3 vector)
    {
        Position = Quaternion.FromAxisAngle(vector, angle) * Position;
    }

    public void SetTo(ShaderProgram program)
    {
        program.SetBool($"light_points[{_index}].is_active", _isActive);
        program.SetBool($"light_points[{_index}].use_attenuation", _useAttenuation);
        program.SetVector3($"light_points[{_index}].position", Position);
        program.SetColor4($"light_points[{_index}].color", _color);
        program.SetColor4($"light_points[{_index}].ambient_component", _ambientComponent);
        program.SetFloat($"light_points[{_index}].diffuse_strength", _diffuseStrength);
        program.SetFloat($"light_points[{_index}].specular_strength", _specularStrength);
        program.SetFloat($"light_points[{_index}].constant_factor", _constantFactor);
        program.SetFloat($"light_points[{_index}].linear_factor", _linearFactor);
        program.SetFloat($"light_points[{_index}].quadratic_factor", _quadraticFactor);
    }
}