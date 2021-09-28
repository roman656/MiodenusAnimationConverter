using System.IO;
using MiodenusAnimationConverter.Exceptions;
using Newtonsoft.Json;

namespace MiodenusAnimationConverter.Loaders.AnimationLoaders
{
    public class LoaderMaf : IAnimationLoader
    {
        public Animation.Animation Load(in string filename)
        {
            CheckAnimationFile(filename);
            
            var content = File.ReadAllText(filename);
            var animation = JsonConvert.DeserializeObject<Animation.MAFStructure.Animation>(content);
            
            return new Animation.Animation(animation);
        }
        
        private static void CheckAnimationFile(in string filename)
        {
            var modelFileInfo = new FileInfo(filename);
            
            if (!modelFileInfo.Exists)
            {
                throw new FileNotFoundException($"File {filename} not found.");
            }

            if (modelFileInfo.Length <= 0)
            {
                throw new EmptyFileException($"File {filename} is empty.");
            }
        }
    }
}