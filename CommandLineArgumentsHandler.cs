using System;
using System.Collections.Generic;
using MiodenusAnimationConverter.Exceptions;
using OpenTK.Graphics.ES30;
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
        
        public string[] InputKeys = {"--fps",
                                     "-f",
                                     "--bitrate",
                                     "-b",
                                     "--extension",
                                     "-e",
                                     "--help",
                                     "?",
                                     "-h",
                                     "--animation",
                                     "-a",
                                     "--models",
                                     "-m",
                                     "--output",
                                     "-o"
        };

        public Dictionary<string, bool> InputKeysCheck = new Dictionary<string, bool>(){
            ["fps"] = false,
            ["bitrate"] = false,
            ["extension"] = false,
            ["animation"] = false,
            ["models"] = false,
            ["output"] = false
        };

        public byte MinimumAmountOfArguments = 3;
        public string AnimationFile = "";
        public List<string> ModelFiles = new List<string>();
        public string VideoFile = "";
        public string Extension = "avi";
        public int Bitrate = 10000;
        public int Fps = 60;

        public MainSettings InitializeSettings()
        {
            MainSettings settings = new MainSettings(VideoFile, Extension, Bitrate, Fps, ModelFiles);

            Console.WriteLine($"settings:\n  VideoFile = {settings.VideoFile}\n  Extension = {settings.Extension}\n  Bitrate = {settings.Bitrate}\n  Fps = {settings.Fps}");

            for (int i = 0; i < settings.AnimationFile.Count; i++)
            {
                Console.WriteLine($"  ModelFiles[{i}] = {settings.AnimationFile[i]}");
            }
            
            Console.WriteLine("end settings");
            
            return settings;
        }
        
        public CommandLineArgumentsHandler(string[] arguments)
        {
            var argumentsList = new List<string>(arguments);
            
            //if (argumentsList.Count == 1 && argumentsList[0] == "-i")
            //{
                //System.Console.Out.WriteLine(info);
                //Environment.Exit(0);
            //}
            
            if (argumentsList.Count < MinimumAmountOfArguments)
            {
                throw new WrongCommandLineArgumentsException($"At least {MinimumAmountOfArguments} command line arguments should be used");
            }
            else
            {
                //System.Console.Out.WriteLine(argumentsList[0]);
                
                for ( ; argumentsList.Count!=0; )
                {
                        
                        //ModelFiles.Add(argumentsList[0]);
                        //argumentsList.Remove(argumentsList[0]);
                        foreach (string argument in argumentsList)
                        {
                            if (argument == InputKeys[6] || argument == InputKeys[7] || argument == InputKeys[8])
                            {
                                Console.WriteLine(info);
                                Environment.Exit(0);
                            }
                        }

                        if (argumentsList.Count < 2)
                        {
                            throw new WrongCommandLineArgumentsException($"Argument {argumentsList[0]} should not be empty"); 
                        }
                        else
                        {
                                if (argumentsList[0] == InputKeys[0]||argumentsList[0]==InputKeys[1])
                                {
                                    if (!InputKeysCheck["fps"])
                                    {
                                        if (!Int32.TryParse(argumentsList[1], out Fps))
                                        {
                                            throw new WrongCommandLineArgumentsException("Fps should be a number");
                                        }
                                        
                                        if (Fps < 1 || Fps > byte.MaxValue)
                                        {
                                            throw new WrongCommandLineArgumentsException("Fps should be greater or equal to 1 and less than 256");
                                        }
                                        
                                        InputKeysCheck["fps"] = true;
                                                                            
                                        argumentsList.Remove(argumentsList[1]);
                                        argumentsList.Remove(argumentsList[0]);
                                                                            
                                        System.Console.Out.WriteLine($"Fps = {Fps}");
                                    }
                                    else
                                    {
                                        throw new WrongCommandLineArgumentsException("Fps should be set only once");
                                    }
                                    
                                    
                                }
                                else if (argumentsList[0] == InputKeys[2]||argumentsList[1]==InputKeys[3])
                                {
                                    if (!InputKeysCheck["bitrate"])
                                    {
                                        if(!Int32.TryParse(argumentsList[1], out Bitrate))
                                        {
                                            throw new WrongCommandLineArgumentsException("Bitrate should be a number");
                                        }

                                        if (Bitrate < 1)
                                        {
                                            throw new WrongCommandLineArgumentsException("Bitrate should be greater or equal to 1");
                                        }
                                        
                                        InputKeysCheck["bitrate"] = true;
                                        
                                        argumentsList.Remove(argumentsList[1]);
                                        argumentsList.Remove(argumentsList[0]);
                                                                            
                                        System.Console.Out.WriteLine($"Bitrate = {Bitrate}");
                                    }
                                    else
                                    {
                                        throw new WrongCommandLineArgumentsException("Bitrate should be set only once");
                                    }
                                    
                                }
                                else if(argumentsList[0] == InputKeys[4]||argumentsList[0]==InputKeys[5])
                                {
                                    if (!InputKeysCheck["extension"])
                                    {
                                        Extension = argumentsList[1];
                                        
                                        InputKeysCheck["extension"] = true;
                                        
                                        argumentsList.Remove(argumentsList[1]);
                                        argumentsList.Remove(argumentsList[0]);
                                        
                                        System.Console.Out.WriteLine($"Extension = {Extension}");
                                    }
                                    else
                                    {
                                        throw new WrongCommandLineArgumentsException("Bitrate should be set only once");
                                    }
                                    
                                }
                                else if (argumentsList[0] == InputKeys[9] || argumentsList[0] == InputKeys[10])
                                {
                                    if (!InputKeysCheck["animation"])
                                    {
                                        AnimationFile = argumentsList[1];

                                        InputKeysCheck["animation"] = true;

                                        argumentsList.Remove(argumentsList[1]);
                                        argumentsList.Remove(argumentsList[0]);
                                        
                                        Console.WriteLine($"Path to animation file = {AnimationFile}");
                                    }
                                    else
                                    {
                                        throw new WrongCommandLineArgumentsException("Path to animation file should be set only once");
                                    }
                                }
                                else if (argumentsList[0] == InputKeys[13] || argumentsList[0] == InputKeys[14])
                                {
                                    if (!InputKeysCheck["output"])
                                    {
                                        VideoFile = argumentsList[1];

                                        InputKeysCheck["output"] = true;

                                        argumentsList.Remove(argumentsList[1]);
                                        argumentsList.Remove(argumentsList[0]);
                                        
                                        Console.WriteLine($"Path to output file = {VideoFile}");
                                    }
                                    else
                                    {
                                        throw new WrongCommandLineArgumentsException("Path to output file should be set only once");
                                    }
                                }
                                else if (argumentsList[0] == InputKeys[11] || argumentsList[0] == InputKeys[12])
                                {
                                    if (!InputKeysCheck["models"])
                                    {
                                        bool exit = false;
                                        
                                        int i = 0;
                                        foreach (string model in argumentsList)
                                        {
                                            for (int j = 0; j < InputKeys.Length;j++)
                                            {
                                                if (i != 0 && model == InputKeys[j])
                                                {
                                                    exit = true;
                                                    break;
                                                }
                                            }

                                            if (exit)
                                            {
                                                break;
                                            }

                                            if (i != 0)
                                            {
                                                ModelFiles.Add(model);
                                                Console.WriteLine($"Model = {ModelFiles[ModelFiles.Count-1]}");
                                                InputKeysCheck["models"] = true;
                                            }

                                            i++;
                                        }

                                        for (int k = i - 1; k >= 0; k--)
                                        {
                                            argumentsList.Remove(argumentsList[k]);
                                        }
                                    }
                                    else
                                    {
                                        throw new WrongCommandLineArgumentsException("Path (or paths) to model (or models) should be set only once");
                                    }
                                }
                        }
                }

                if (ModelFiles.Count < 1 || AnimationFile == "" || VideoFile == "")
                {
                    throw new WrongCommandLineArgumentsException($"All {MinimumAmountOfArguments} required arguments should be used: path to animation file (-a), path to model (or models) (-m) and path to output video (-o)");
                }
                
                VideoFile = ModelFiles[ModelFiles.Count - 1];
                ModelFiles.Remove(ModelFiles[ModelFiles.Count - 1]);
                
                //System.Console.Out.WriteLine($"Video file = {VideoFile}");
                
                //foreach (var animation in ModelFiles)
                //{
                    //System.Console.Out.WriteLine($"Animation file = {animation}");
                //}
            }
        }
    }
}