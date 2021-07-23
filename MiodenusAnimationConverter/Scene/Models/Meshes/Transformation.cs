using OpenTK.Mathematics;

namespace MiodenusAnimationConverter.Scene.Models.Meshes
{
    public struct Transformation
    {
        public const byte SizeInBytes = (3 + 4 + 3) * sizeof(float);
        public Vector3 Location;
        public Quaternion Rotation;
        public Vector3 Scale;

        public Transformation(Vector3 location, Quaternion rotation, Vector3 scale)
        {
            Location = location;
            Rotation = rotation;
            Scale = scale;
        }

        public void Reset()
        {
            Location = Vector3.Zero;
            Rotation = Quaternion.Identity;
            Scale = Vector3.One;
        }
    }
}