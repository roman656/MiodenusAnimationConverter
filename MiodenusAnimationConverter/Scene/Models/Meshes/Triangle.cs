using System;
using OpenTK.Mathematics;

namespace MiodenusAnimationConverter.Scene.Models.Meshes
{
    public struct Triangle : IMovable, IRotatable, IScalable
    {
        public const byte VertexesAmount = 3;
        public readonly Vertex[] Vertexes;

        public Triangle(in Vertex[] vertexes, Vector3 normal)
        {
            CheckArgument(vertexes);

            Vertexes = new Vertex[VertexesAmount];
            
            for (byte i = 0; i < VertexesAmount; i++)
            {
                Vertexes[i] = new Vertex(vertexes[i].Position, normal, vertexes[i].Color);
            }
        }

        public static Vector3 CalculateNormal(in Vertex[] vertexes)
        {
            CheckArgument(vertexes);

            var AB = new Vector3(vertexes[1].Position - vertexes[0].Position);
            var AC = new Vector3(vertexes[2].Position - vertexes[0].Position);

            return Vector3.Cross(AB, AC).Normalized();
        }

        private static void CheckArgument(in Vertex[] vertexes)
        {
            if (vertexes == null)
            {
                throw new ArgumentException("Argument \"vertexes\" can not be null.");
            }
            
            if (vertexes.Length != VertexesAmount)
            {
                throw new ArgumentException($"Triangle vertexes amount must be {VertexesAmount}.");
            }
        }

        public void Move(float deltaX, float deltaY, float deltaZ)
        {
            foreach (var vertex in Vertexes)
            {
                vertex.Move(deltaX, deltaY, deltaZ);
            }
        }

        public void Rotate(float angle, Vector3 vector)
        {
            foreach (var vertex in Vertexes)
            {
                vertex.Rotate(angle, vector);
            }
        }

        public void Scale(float scaleX, float scaleY, float scaleZ)
        {
            foreach (var vertex in Vertexes)
            {
                vertex.Scale(scaleX, scaleY, scaleZ);
            }
        }

    }
}