namespace MiodenusAnimationConverter.Loaders
{
    public interface IModelLoader
    {
        public Model Load(in string filename);
    }
}