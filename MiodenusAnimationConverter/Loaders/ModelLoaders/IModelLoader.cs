using MiodenusAnimationConverter.Scene.Models;
using OpenTK.Mathematics;

namespace MiodenusAnimationConverter.Loaders.ModelLoaders
{
    public interface IModelLoader
    {
        public Model Load(in string filename, Color4 modelColor, bool useCalculatedNormals);
    }
}