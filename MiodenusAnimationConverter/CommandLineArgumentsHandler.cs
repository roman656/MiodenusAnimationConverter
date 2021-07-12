using System;
using System.Collections.Generic;
using MiodenusAnimationConverter.Exceptions;
using OpenTK.Graphics.OpenGL;

namespace MiodenusAnimationConverter
{
    public class CommandLineArgumentsHandler
    {
        public string info =
            "This is Miodenus - a program which will make a video from your animation file. You can use special" +
            " command line keys to change some settings of it. The key -fps will set target value for fps in the" +
            " output video file. The key -bitrate will set value of bitrate for the output video. Key -extension" +
            " will set extension of the output video file.\nThe first argument (or arguments) should be path" +
            " (or paths) to input file (or files), the second should be a path to the output video destination," +
            " order of other arguments is not important.";
        
        public string[] InputKeys = {"-fps",
                                     "-bitrate",
                                     "-extension"
        };

        public bool[] InputKeysCheck = {false,
                                        false,
                                        false
        };

        public List<string> AnimationFile = new List<string>();
        public string VideoFile = "";
        public string Extension = "avi";
        public int Bitrate = 10000;
        public int Fps = 60;

        private Video InitializeVideo()
        {
            Video video = new Video(Extension, 60, (uint)Bitrate, VideoFile, (byte)Fps);
            
            return video;
        }
        
        public CommandLineArgumentsHandler(string[] arguments)
        {
            var argumentsList = new List<string>(arguments);
            
            if (argumentsList.Count == 1 && argumentsList[0] == "-i")
            {
                System.Console.Out.WriteLine(info);
                Environment.Exit(0);
            }
            
            if (argumentsList.Count < 2)
            {
                throw new WrongCommandLineArgumentsException("At least 2 command line arguments should be used");
            }
            else
            {
                System.Console.Out.WriteLine(argumentsList[0]);
                
                for ( ; argumentsList.Count!=0; )
                {
                    if (argumentsList[0] != InputKeys[0] && argumentsList[0] != InputKeys[1] && argumentsList[0] != InputKeys[2])
                        {
                            
                            AnimationFile.Add(argumentsList[0]);
                            argumentsList.Remove(argumentsList[0]);
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
                                    if (!InputKeysCheck[0])
                                    {
                                        if (!Int32.TryParse(argumentsList[1], out Fps))
                                        {
                                            throw new WrongCommandLineArgumentsException("Fps should be a number");
                                        }
                                        
                                        if (Fps < 1 || Fps > 255)
                                        {
                                            throw new WrongCommandLineArgumentsException("Fps should be greater or equal to 1 and less than 256");
                                        }
                                        
                                        InputKeysCheck[0] = true;
                                                                            
                                        argumentsList.Remove(argumentsList[1]);
                                        argumentsList.Remove(argumentsList[0]);
                                                                            
                                        System.Console.Out.WriteLine($"Fps = {Fps}");
                                    }
                                    else
                                    {
                                        throw new WrongCommandLineArgumentsException("Fps should be set only once");
                                    }
                                    
                                    
                                }
                                else if (argumentsList[0] == "-bitrate")
                                {
                                    if (!InputKeysCheck[1])
                                    {
                                        if(!Int32.TryParse(argumentsList[1], out Bitrate))
                                        {
                                            throw new WrongCommandLineArgumentsException("Bitrate should be a number");
                                        }

                                        if (Bitrate < 1)
                                        {
                                            throw new WrongCommandLineArgumentsException("Bitrate should be greater or equal to 1");
                                        }
                                        
                                        InputKeysCheck[1] = true;
                                        
                                        argumentsList.Remove(argumentsList[1]);
                                        argumentsList.Remove(argumentsList[0]);
                                                                            
                                        System.Console.Out.WriteLine($"Bitrate = {Bitrate}");
                                    }
                                    else
                                    {
                                        throw new WrongCommandLineArgumentsException("Bitrate should be set only once");
                                    }
                                    
                                }
                                else if(argumentsList[0] == "-extension")
                                {
                                    if (!InputKeysCheck[2])
                                    {
                                        Extension = argumentsList[1];
                                        
                                        InputKeysCheck[2] = true;
                                        
                                        argumentsList.Remove(argumentsList[1]);
                                        argumentsList.Remove(argumentsList[0]);
                                        
                                        System.Console.Out.WriteLine($"Extension = {Extension}");
                                    }
                                    else
                                    {
                                        throw new WrongCommandLineArgumentsException("Bitrate should be set only once");
                                    }
                                    
                                }
                            }
                        }
                }

                if (AnimationFile.Count < 2)
                {
                    throw new WrongCommandLineArgumentsException("The first 2 command line arguments should be: 1. Path for input file 2. Path for output file");
                }
                
                VideoFile = AnimationFile[AnimationFile.Count - 1];
                AnimationFile.Remove(AnimationFile[AnimationFile.Count - 1]);
                
                System.Console.Out.WriteLine($"Video file = {VideoFile}");
                
                foreach (var animation in AnimationFile)
                {
                    System.Console.Out.WriteLine($"Animation file = {animation}");
                }
            }

            InitializeVideo();
        }
    }
}