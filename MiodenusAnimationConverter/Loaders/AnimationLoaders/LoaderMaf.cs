using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using MiodenusAnimationConverter.Exceptions;
using Newtonsoft.Json;
using NLog;

namespace MiodenusAnimationConverter.Loaders.AnimationLoaders
{
    public class LoaderMaf : IAnimationLoader
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();
        
        public Animation.Animation Load(in string filename)
        {
            var animation = LoadMaf(filename);
            var loadedFiles = new List<string> { filename };
            var filesToLoad = new List<string>();

            filesToLoad.AddRange(animation.AnimationInfo.Include);

            while (filesToLoad.Count > 0)
            {
                var file = filesToLoad.First();
                filesToLoad.RemoveAt(0);
                
                if (!loadedFiles.Contains(file))
                {
                    loadedFiles.Add(file);
                    
                    try
                    {
                        var temp = LoadMaf(file);

                        foreach (var action in temp.Actions)
                        {
                            animation.Actions.Add(action);
                        }
                        
                        filesToLoad.AddRange(temp.AnimationInfo.Include);
                    }
                    catch (Exception exception)
                    {
                        Logger.Warn(exception.Message);
                    }
                }
            }

            return new Animation.Animation(animation);
        }

        private static Animation.MafStructure.Animation LoadMaf(in string filename)
        {
            CheckFile(filename);
            return JsonConvert.DeserializeObject<Animation.MafStructure.Animation>(File.ReadAllText(filename));
        }
        
        private static void CheckFile(in string filename)
        {
            var fileInfo = new FileInfo(filename);
            
            if (!fileInfo.Exists)
            {
                throw new FileNotFoundException($"File {filename} not found.");
            }

            if (fileInfo.Length <= 0)
            {
                throw new EmptyFileException($"File {filename} is empty.");
            }
        }
    }
}