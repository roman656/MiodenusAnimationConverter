using OpenTK.Mathematics;

namespace MiodenusAnimationConverter.Loaders
{
    public interface IModelLoader
    {
        public Model Load(in string filename, Color4 modelColor, bool useCalculatedNormals);
    }
}