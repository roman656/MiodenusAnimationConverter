using System;
using OpenTK.Mathematics;

namespace MiodenusAnimationConverter
{
    public struct Vertex
    {
        public const byte SizeInBytes = (4 + 4) * 4;
        public Vector4 Position;
        public Color4 Color;
        
        public Vertex(Vector4 position, Color4 color)
        {
            Position = position;
            Color = color;
        }
    }
}