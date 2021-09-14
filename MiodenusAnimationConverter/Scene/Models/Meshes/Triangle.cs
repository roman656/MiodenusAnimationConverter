using MiodenusAnimationConverter.Exceptions;
using OpenTK.Mathematics;

namespace MiodenusAnimationConverter.Scene.Models.Meshes
{
    /// <summary>Полигон, состоящий из 3-х <see cref="Vertex">вершин</see>.</summary>
    public readonly struct Triangle
    {
        /// <summary>Число <see cref="Vertex">вершин</see> в треугольном полигоне.</summary>
        public const byte VertexesAmount = 3;
        /// <summary>Массив <see cref="Vertex">вершин</see>, образующих данный полигон.</summary>
        public readonly Vertex[] Vertexes;

        /// <summary>Конструктор полигона, состоящего из 3-х <see cref="Vertex">вершин</see>.</summary>
        /// <param name="vertexes">массив, содержащий 3 вершины поверхности (полигона).</param>
        /// <param name="normal">вектор нормали поверхности, образуемой тремя вершинами.</param>
        public Triangle(in Vertex[] vertexes, Vector3 normal)
        {
            Vertexes = new Vertex[VertexesAmount];
            
            for (var i = 0; i < VertexesAmount; i++)
            {
                Vertexes[i] = new Vertex(vertexes[i].Position, normal, vertexes[i].Color);
            }
        }

        /// <summary>
        /// Устанавливает новый цвет всем <see cref="Vertex">вершинам</see>, входящим в состав полигона.
        /// </summary>
        public Color4 Color
        {
            set
            {
                for (var i = 0; i < VertexesAmount; i++)
                {
                    Vertexes[i].Color = value;
                }
            }
        }

        /// <summary>Метод, вычисляющий вектор нормали поверхности, образуемой тремя вершинами.</summary>
        /// <remarks>
        /// Если смотреть на поверхность, образованную тремя вершинами, в направлении противоположном вектору нормали,
        /// то порядковые номера вершин будут расти в направлении против часовой стрелки.
        /// </remarks>
        /// <param name="vertexes">массив, содержащий 3 вершины поверхности.</param>
        /// <returns>Вектор нормали к поверхности.</returns>
        /// <exception cref="TopologyException">
        /// не возможно определить направление вектора нормали, если все 3 вершины лежат на одной прямой.
        /// </exception>
        public static Vector3 CalculateNormal(in Vertex[] vertexes)
        {
            var AB = new Vector3(vertexes[1].Position - vertexes[0].Position);
            var AC = new Vector3(vertexes[2].Position - vertexes[0].Position);

            var crossProduct = Vector3.Cross(AB, AC);

            if (crossProduct.Length <= 0.0f)
            {
                throw new TopologyException("It is impossible to determine the direction of the normal vector." 
                        + "All vertexes of the surface lie on the same straight line.");
            }

            return crossProduct.Normalized();
        }
    }
}