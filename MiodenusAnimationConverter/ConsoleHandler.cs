using System;
using MiodenusAnimationConverter.Exceptions;
using OpenTK.Graphics.OpenGL;

namespace MiodenusAnimationConverter
{
    public class ConsoleHandler
    {
        public string[] InputKeys = {"fps",
                                     "bitrate",
                                     "extention"
        };

        public string AnimationFile = "";
        public string VideoFile = "";
        public string Extention = "";
        public int Bitrate = 0;
        public int Fps = 0;
        
        public ConsoleHandler(){}

        public void InitializeProgram(string[] arguments)
        {
            if (arguments.Length < 2)
            {
                throw new CommandLineArgumentsException("At least 2 command line arguments should be used");
            }
            else
            {
                AnimationFile = arguments[0];
                VideoFile = arguments[1];
            }

            if (arguments.Length > 2)
            {
                if (arguments[2] != InputKeys[2])
                {
                    throw new CommandLineArgumentsException("The third argument should be extention");
                }
                
                if(arguments.Length < 4)
                {
                    throw new CommandLineArgumentsException("Extention of video can not be empty");
                }
                else
                {
                    Extention = arguments[3];
                }

                if (arguments.Length > 4)
                {
                    if (arguments[4] != InputKeys[1])
                    {
                        throw new CommandLineArgumentsException("The fourth argument should be bitrate");
                    }

                    if (arguments.Length < 6)
                    {
                        throw new CommandLineArgumentsException("Bitrate can not be empty");
                    }
                    else
                    {
                        if (!Int32.TryParse(arguments[5], out Bitrate))
                        {
                            throw new CommandLineArgumentsException("Bitrate should be a number");
                        }
                    }

                    if (arguments.Length > 6)
                    {
                        if (arguments[6] != InputKeys[0])
                        {
                            throw new CommandLineArgumentsException("The fifth argument should be fps");
                        }

                        if (arguments.Length < 8)
                        {
                            throw new CommandLineArgumentsException("Fps can not be empty");
                        }
                        else
                        {
                            if (!Int32.TryParse(arguments[7], out Fps))
                            {
                                throw new CommandLineArgumentsException("Fps should be a number");
                            }
                        }
                    }
                }
            }
            else
            {
                Extention = "avi";
                Bitrate = 10000;
                Fps = 60;
            }
        }
    }
}