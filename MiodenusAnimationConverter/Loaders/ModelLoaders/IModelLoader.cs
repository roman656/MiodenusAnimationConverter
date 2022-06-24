namespace MiodenusAnimationConverter.Loaders.ModelLoaders;

using Animation;
using Scene.Models;

public interface IModelLoader
{
    public Model Load(in ModelInfo modelInfo);
}