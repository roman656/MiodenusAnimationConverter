namespace MiodenusAnimationConverter.Loaders.AnimationLoaders
{
    public interface IAnimationLoader
    {
        public Animation.Animation Load(in string filename);
    }
}