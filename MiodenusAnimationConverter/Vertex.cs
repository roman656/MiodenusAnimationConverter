using OpenTK.Mathematics;

namespace MiodenusAnimationConverter
{
    public struct Vertex
    {
        public const int Size = (4 + 4) * 4;    // Размер структуры в байтах.
        
        private readonly Vector4 _position;
        private readonly Color4 _color;
        
        public Vertex(Vector4 position, Color4 color)
        {
            _position = position;
            _color = color;
        }
    }
}