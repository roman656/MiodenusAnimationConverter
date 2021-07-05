using System;
using OpenTK.Mathematics;

namespace MiodenusAnimationConverter
{
    public struct Triangle
    {
        public const byte VertexesAmount = 3;
        
        public readonly Vector3 Normal;
        public readonly Vertex[] Vertexes;

        public Triangle(Vector3 normal, Vertex[] vertexes)
        {
            if (vertexes.Length != VertexesAmount)
            {
                throw new ArgumentException($"Triangle vertexes amount must be {VertexesAmount}.");
            }
            
            Normal = normal;
            Vertexes = vertexes;
        }
    }
}