using MiodenusAnimationConverter.Animation;
using MiodenusAnimationConverter.Scene.Models;

namespace MiodenusAnimationConverter.Loaders.ModelLoaders
{
    public interface IModelLoader
    {
        public Model Load(in ModelInfo modelInfo);
    }
}