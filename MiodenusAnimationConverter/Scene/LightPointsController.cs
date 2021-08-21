using OpenTK.Mathematics;

namespace MiodenusAnimationConverter.Scene
{
    public class LightPointsController
    {
        private const ushort LightPointsTotalAmount = 2;
        private LightPoint[] _lightPoints = new LightPoint[LightPointsTotalAmount];
        private int _lightPointsAvaliable = LightPointsTotalAmount;

        public LightPointsController()
        {
            for (var i = 0; i < LightPointsTotalAmount; i++)
            {
                _lightPoints[i] = new LightPoint(new Vector3(), Color4.White);
                _lightPoints[i].Disable();
            }
        }

        public int LightPointsAvaliable => _lightPointsAvaliable;

        public LightPoint AddLightPoint(Vector3 position, Color4 color)
        {
            LightPoint result = null;
            
            if (_lightPointsAvaliable > 0)
            {
                _lightPoints[_lightPointsAvaliable - 1].Position = position;
                _lightPoints[_lightPointsAvaliable - 1].Color = color;
                _lightPoints[_lightPointsAvaliable - 1].Enable();
                result = _lightPoints[_lightPointsAvaliable - 1];
                _lightPointsAvaliable--;
            }

            return result;
        }
    }
}