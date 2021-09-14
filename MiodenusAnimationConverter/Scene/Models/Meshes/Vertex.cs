using System.Globalization;
using OpenTK.Mathematics;

namespace MiodenusAnimationConverter.Scene.Models.Meshes
{
    /// <summary>Структура, содержащая параметры вершины в трехмерном пространстве.</summary>
    public struct Vertex
    {
        /// <summary>Значение положения вершины в трехмерном пространстве.</summary>
        public readonly Vector3 Position;
        /// <summary>Значение нормали вершины.</summary><remarks>Используется в расчетах освещения.</remarks>
        public readonly Vector3 Normal;
        /// <summary>Значение цвета вершины.</summary>
        public Color4 Color;

        /// <summary>Конструктор вершины.</summary>
        /// <param name="position">значение параметра <see cref="Position">положения</see>.</param>
        /// <param name="normal">значение параметра <see cref="Normal">нормали</see>.</param>
        /// <param name="color">значение параметра <see cref="Color">цвета</see>.</param>
        public Vertex(Vector3 position, Vector3 normal, Color4 color)
        {
            Position = position;
            Normal = normal;
            Color = color;
        }

        /// <summary>Метод, переводящий параметры вершины в строку.</summary>
        /// <returns>Строка, содержащая информацию о вершине.</returns>
        public override string ToString()
        {
            return string.Format(CultureInfo.InvariantCulture,
                    $"Vertex:\n\tPosition: ({Position.X}, {Position.Y}, {Position.Z})\n\t"
                    + $"Normal: ({Normal.X}, {Normal.Y}, {Normal.Z})\n\t"
                    + $"Color: ({Color.R}, {Color.G}, {Color.B}, {Color.A})\n\t");
        }
    }
}