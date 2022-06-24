namespace MiodenusAnimationConverter.Scene;

using Shaders;
using OpenTK.Mathematics;

public class LightPointsController
{
    private const ushort LightPointsTotalAmount = 8;
    private LightPoint[] _lightPoints = new LightPoint[LightPointsTotalAmount];
    private int _lightPointsAvailable = LightPointsTotalAmount;

    public LightPointsController()
    {
        for (var i = 0; i < LightPointsTotalAmount; i++)
        {
            _lightPoints[i] = new LightPoint(new Vector3(), Color4.White);
            _lightPoints[i].Disable();
        }
    }

    public int LightPointsAvailable => _lightPointsAvailable;

    public LightPoint AddLightPoint(Vector3 position, Color4 color)
    {
        LightPoint result = null;
            
        if (_lightPointsAvailable > 0)
        {
            _lightPoints[_lightPointsAvailable - 1].Position = position;
            _lightPoints[_lightPointsAvailable - 1].Color = color;
            _lightPoints[_lightPointsAvailable - 1].Enable();
            result = _lightPoints[_lightPointsAvailable - 1];
            _lightPointsAvailable--;
        }

        return result;
    }

    public void SetLightPointsTo(ShaderProgram program)
    {
        for (var i = 0; i < LightPointsTotalAmount; i++)
        {
            _lightPoints[i].SetTo(program);
        }
    }
}