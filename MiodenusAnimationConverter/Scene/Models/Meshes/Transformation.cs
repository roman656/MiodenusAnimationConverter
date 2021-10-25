using System.Globalization;
using OpenTK.Mathematics;

namespace MiodenusAnimationConverter.Scene.Models.Meshes
{
    /// <summary>Структура, содержащая параметры трансформации <see cref="Mesh">полигональной сетки</see>.</summary>
    /// <remarks>
    /// Используется для перевода всех <see cref="Vertex">вершин</see> полигональной сетки
    /// в глобальную систему координат.
    /// </remarks>
    public struct Transformation
    {
        /// <summary>
        /// Значение сдвига относительно локальной системы координат трансформируемой <see cref="Vertex">вершины</see>.
        /// </summary>
        public Vector3 Location;
        /// <summary>
        /// Значение поворота относительно локальной системы координат трансформируемой
        /// <see cref="Vertex">вершины</see>.
        /// </summary>
        public Quaternion Rotation;
        /// <summary>
        /// Значение масштаба трансформируемой <see cref="Vertex">вершины</see> относительно локальной системы
        /// координат.
        /// </summary>
        public Vector3 Scale;

        /// <summary>Конструктор структуры трансформации.</summary>
        /// <param name="location">значение параметра <see cref="Location">сдвига</see>.</param>
        /// <param name="rotation">значение параметра <see cref="Rotation">поворота</see>.</param>
        /// <param name="scale">значение параметра <see cref="Scale">масштаба</see>.</param>
        public Transformation(Vector3 location, Quaternion rotation, Vector3 scale)
        {
            Location = location;
            Rotation = rotation;
            Scale = scale;
        }

        /// <summary>Метод, устанавливающий параметры трансформации в значения по умолчанию.</summary>
        public void Reset()
        {
            Location = Vector3.Zero;
            Rotation = Quaternion.Identity;
            Scale = Vector3.One;
        }
        
        /// <summary>
        /// Метод, устанавливающий параметр <see cref="Location">сдвига</see> трансформируемой
        /// <see cref="Vertex">вершины</see> в значение по умолчанию.
        /// </summary>
        public void ResetLocation()
        {
            Location = Vector3.Zero;
        }
        
        /// <summary>
        /// Метод, устанавливающий параметр <see cref="Rotation">поворота</see> трансформируемой
        /// <see cref="Vertex">вершины</see> в значение по умолчанию.
        /// </summary>
        public void ResetRotation()
        {
            Rotation = Quaternion.Identity;
        }
        
        /// <summary>
        /// Метод, устанавливающий параметр <see cref="Scale">масштаба</see> трансформируемой
        /// <see cref="Vertex">вершины</see> в значение по умолчанию.
        /// </summary>
        public void ResetScale()
        {
            Scale = Vector3.One;
        }

        /// <summary>Метод, переводящий параметры трансформации в строку.</summary>
        /// <returns>Строка, содержащая информацию о трансформации.</returns>
        public override string ToString()
        {
            return string.Format(CultureInfo.InvariantCulture,
                    $"Transformation: [ Location: ({Location.X}; {Location.Y}; {Location.Z}) "
                    + $"| Rotation: ({Rotation.X}; {Rotation.Y}; {Rotation.Z}; {Rotation.W}) "
                    + $"| Scale: ({Scale.X}; {Scale.Y}; {Scale.Z}) ]");
        }
    }
}