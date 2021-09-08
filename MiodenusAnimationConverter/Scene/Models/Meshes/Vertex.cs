using System.Globalization;
using OpenTK.Mathematics;

namespace MiodenusAnimationConverter.Scene.Models.Meshes
{
    /// <summary>Структура, содержащая параметры вершины в трехмерном пространстве.</summary>
    public struct Vertex : IMovable, IRotatable, IScalable
    {
        /// <summary>Значение положения вершины в трехмерном пространстве.</summary>
        public readonly Vector3 Position;
        /// <summary>Значение нормали вершины.</summary><remarks>Используется в расчетах освещения.</remarks>
        public readonly Vector3 Normal;
        /// <summary>Значение цвета вершины.</summary>
        public readonly Color4 Color;
        /// <summary>Значение трансформации вершины.</summary>
        private Transformation _transformation;

        /// <inheritdoc cref="_transformation"/>
        public Transformation Transformation => _transformation;

        /// <summary>Конструктор <see cref="Vertex">вершины</see>.</summary>
        /// <param name="position">значение параметра <see cref="Position">положения</see>.</param>
        /// <param name="normal">значение параметра <see cref="Normal">нормали</see>.</param>
        /// <param name="color">значение параметра <see cref="Color">цвета</see>.</param>
        public Vertex(Vector3 position, Vector3 normal, Color4 color)
        {
            Position = position;
            Normal = normal;
            Color = color;
            _transformation = new Transformation(Vector3.Zero, Quaternion.Identity, Vector3.One);
        }

        /// <summary>Метод, сдвигающий вершину в трехмерном пространстве.</summary>
        /// <remarks>
        /// Сдвиг "применяется" к <see cref="Transformation">трансформации</see> вершины.
        /// Ее исходные параметры (<see cref="Position">положение</see>, <see cref="Normal">нормаль</see>,
        /// <see cref="Color">цвет</see>) не изменяются.
        /// </remarks>
        /// <param name="deltaX">значение сдвига вершины по оси OX.</param>
        /// <param name="deltaY">значение сдвига вершины по оси OY.</param>
        /// <param name="deltaZ">значение сдвига вершины по оси OZ.</param>
        public void Move(float deltaX, float deltaY, float deltaZ)
        {
            _transformation.Location.X += deltaX;
            _transformation.Location.Y += deltaY;
            _transformation.Location.Z += deltaZ;
        }

        /// <summary>
        /// Метод, поворачивающий вершину в трехмерном пространстве на угол <c>angle</c> вокруг вектора <c>vector</c>.
        /// </summary>
        /// <remarks>
        /// Поворот "применяется" к <see cref="Transformation">трансформации</see> вершины.
        /// Ее исходные параметры (<see cref="Position">положение</see>, <see cref="Normal">нормаль</see>,
        /// <see cref="Color">цвет</see>) не изменяются. Поворот осуществляется против часовой
        /// стрелки, если смотреть в направлении противоположном вектору, вокруг которого происходит вращение.
        /// </remarks>
        /// <param name="angle">значение угла поворота (в радианах).</param>
        /// <param name="vector">вектор, вокруг которого будет осуществлен поворот.</param>
        public void Rotate(float angle, Vector3 vector)
        {
            _transformation.Rotation *= Quaternion.FromAxisAngle(vector, angle);
        }

        /// <summary>Метод, выполняющий масштабирование вершины.</summary>
        /// <remarks>
        /// Масштабирование "применяется" к <see cref="Transformation">трансформации</see> вершины.
        /// Ее исходные параметры (<see cref="Position">положение</see>, <see cref="Normal">нормаль</see>,
        /// <see cref="Color">цвет</see>) не изменяются. Для большего понимания принципа работы масштабирования
        /// вершины стоит рассматривать его как состовляющую масштабирования всей 3D-модели.
        /// </remarks>
        /// <param name="scaleX">значение масштаба по оси OX.</param>
        /// <param name="scaleY">значение масштаба по оси OY.</param>
        /// <param name="scaleZ">значение масштаба по оси OZ.</param>
        public void Scale(float scaleX, float scaleY, float scaleZ)
        {
            _transformation.Scale.X *= scaleX;
            _transformation.Scale.Y *= scaleY;
            _transformation.Scale.Z *= scaleZ;
        }

        /// <summary>Метод, переводящий параметры вершины в строку.</summary>
        /// <returns>Строка, содержащая информацию о вершине.</returns>
        public override string ToString()
        {
            return string.Format(CultureInfo.InvariantCulture,
                    $"Vertex:\n\tPosition: ({Position.X}, {Position.Y}, {Position.Z})\n\t"
                    + $"Normal: ({Normal.X}, {Normal.Y}, {Normal.Z})\n\t"
                    + $"Color: ({Color.R}, {Color.G}, {Color.B}, {Color.A})\n\t"
                    + _transformation);
        }
    }
}