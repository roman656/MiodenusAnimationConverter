using System;
using System.Globalization;
using OpenTK.Mathematics;

namespace MiodenusAnimationConverter.Scene.Models.Meshes
{
    /// <summary>Класс, представляющий собой полигональную сетку.</summary>
    public class Mesh : IMovable, IRotatable, IScalable
    {
        /// <summary>Массив <see cref="Triangle">полигонов</see>, образующих полигональную сетку.</summary>
        public readonly Triangle[] Triangles;
        /// <summary>Трансформация полигональной сетки.</summary>
        /// <remarks>Используется для перевода полигональной сетки в глобальную систему координат.</remarks>
        private Transformation _transformation;

        /// <summary>Конструктор полигональной сетки.</summary>
        /// <param name="triangles">массив <see cref="Triangle">полигонов</see>.</param>
        public Mesh(in Triangle[] triangles)
        {
            Triangles = new Triangle[triangles.Length];
            Array.Copy(triangles, Triangles, triangles.Length);
            _transformation = new Transformation(Vector3.Zero, Quaternion.Identity, Vector3.One);
        }

        /// <summary>Конструктор полигональной сетки.</summary>
        /// <param name="triangles">массив <see cref="Triangle">полигонов</see>.</param>
        /// <param name="transformation"><see cref="Transformation">трансформация</see> полигональной сетки.</param>
        public Mesh(in Triangle[] triangles, Transformation transformation) : this(triangles)
        {
            _transformation = transformation;
        }

        /// <inheritdoc cref="_transformation"/>
        public Transformation Transformation => _transformation;

        /// <summary>
        /// Устанавливает новый цвет всем <see cref="Triangle">полигонам</see>, входящим в состав сетки.
        /// </summary>
        public Color4 Color
        {
            set
            {
                var trianglesAmount = Triangles.Length;
                
                for (var i = 0; i < trianglesAmount; i++)
                {
                    Triangles[i].Color = value;
                }
            }
        }

        /// <summary>Метод, сдвигающий полигональную сетку в трехмерном пространстве.</summary>
        /// <remarks>
        /// Сдвиг "применяется" к <see cref="Transformation">трансформации</see>.
        /// <see cref="Triangles">Полигоны</see> сетки не изменяются.
        /// </remarks>
        /// <param name="deltaX">значение сдвига по оси OX.</param>
        /// <param name="deltaY">значение сдвига по оси OY.</param>
        /// <param name="deltaZ">значение сдвига по оси OZ.</param>
        public void Move(float deltaX, float deltaY, float deltaZ)
        {
            _transformation.Location.X += deltaX;
            _transformation.Location.Y += deltaY;
            _transformation.Location.Z += deltaZ;
        }

        /// <summary>
        /// Метод, поворачивающий полигональную сетку в трехмерном пространстве на угол <c>angle</c>
        /// вокруг вектора <c>vector</c>.
        /// </summary>
        /// <remarks>
        /// Поворот "применяется" к <see cref="Transformation">трансформации</see>.
        /// <see cref="Triangles">Полигоны</see> сетки не изменяются. Поворот осуществляется против часовой
        /// стрелки, если смотреть в направлении противоположном вектору, вокруг которого происходит вращение.
        /// </remarks>
        /// <param name="angle">значение угла поворота (в радианах).</param>
        /// <param name="vector">вектор, вокруг которого будет осуществлен поворот.</param>
        public void Rotate(float angle, Vector3 vector)
        {
            _transformation.Rotation *= Quaternion.FromAxisAngle(vector, angle);
        }

        /// <summary>Метод, выполняющий масштабирование полигональной сетки.</summary>
        /// <remarks>
        /// Масштабирование "применяется" к <see cref="Transformation">трансформации</see>.
        /// <see cref="Triangles">Полигоны</see> сетки не изменяются.
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

        /// <summary>Метод, переводящий параметры полигональной сетки в строку.</summary>
        /// <remarks>
        /// Так как число <see cref="Triangles">полигонов</see> сетки может быть достаточно большим - было
        /// принято решение выводить только их количество. В случае необходимости получения подробной информации
        /// о любом полигоне, находящемся в составе сетки, можно обратиться к соответствующему
        /// <see cref="Triangles">полю</see> класса.
        /// </remarks>
        /// <returns>Строка, содержащая информацию о сетке.</returns>
        public override string ToString()
        {
            return string.Format(CultureInfo.InvariantCulture, $"Mesh:\n\tTriangles amount: {Triangles.Length}\n\t"
                    + Transformation);
        }
    }
}