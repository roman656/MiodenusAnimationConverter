using System;
using System.Collections.Generic;
using MiodenusAnimationConverter.Exceptions;
using OpenTK.Graphics.OpenGL;

namespace MiodenusAnimationConverter
{
    public class CommandLineArgumentsHandler
    {
        public string[] InputKeys = {"-fps",
                                     "-bitrate",
                                     "-extension"
        };

        public List<string> AnimationFile = new List<string>();
        public string VideoFile = "";
        public string Extension = "avi";
        public int Bitrate = 10000;
        public int Fps = 60;

        public CommandLineArgumentsHandler(string[] arguments)
        {
            
            var argumentsList = new List<string>(arguments);

            if (argumentsList.Count < 2)
            {
                throw new WrongCommandLineArgumentsException("At least 2 command line arguments should be used");
            }
            else
            {
                System.Console.Out.WriteLine(argumentsList[0]);
                
                for (int i = 0; argumentsList.Count!=0; )
                {
                    foreach (string key in InputKeys)
                    {
                        if (argumentsList[i] != InputKeys[0] && argumentsList[i] != InputKeys[1] && argumentsList[i] != InputKeys[2])
                        {
                            AnimationFile.Add(argumentsList[i]);
                            argumentsList.Remove(argumentsList[i]);
                            break;
                        }
                        else
                        {
                            if (argumentsList.Count < 2)
                            {
                                throw new WrongCommandLineArgumentsException($"Argument {argumentsList[0]} should not be empty"); 
                            }
                            else
                            {
                                if (argumentsList[0] == "-fps")
                                {
                                    if (!Int32.TryParse(argumentsList[1], out Fps))
                                    {
                                        throw new WrongCommandLineArgumentsException("Fps should be a number");
                                    }
                                    
                                    argumentsList.Remove(argumentsList[1]);
                                    argumentsList.Remove(argumentsList[0]);
                                    
                                    System.Console.Out.WriteLine($"Fps = {Fps}");
                                    
                                    break;
                                }
                                if (argumentsList[0] == "-bitrate")
                                {
                                    if(!Int32.TryParse(argumentsList[1], out Bitrate))
                                    {
                                        throw new WrongCommandLineArgumentsException("Bitrate should be a number");
                                    }
                                    
                                    argumentsList.Remove(argumentsList[1]);
                                    argumentsList.Remove(argumentsList[0]);
                                    
                                    System.Console.Out.WriteLine($"Bitrate = {Bitrate}");
                                    
                                    break;
                                }
                                if(argumentsList[0] == "-extension")
                                {
                                    Extension = argumentsList[1];
                                    
                                    argumentsList.Remove(argumentsList[1]);
                                    argumentsList.Remove(argumentsList[0]);

                                    System.Console.Out.WriteLine($"Extension = {Extension}");
                                    
                                    break;
                                }
                            }
                        }

                        
                    }
                }
                VideoFile = AnimationFile[AnimationFile.Count - 1];
                AnimationFile.Remove(AnimationFile[AnimationFile.Count - 1]);
                System.Console.Out.WriteLine($"Video file = {VideoFile}");
                foreach (var animation in AnimationFile)
                {
                    System.Console.Out.WriteLine($"Animation file = {animation}");
                }
            }
        }
    }
}