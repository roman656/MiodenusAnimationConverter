using System;
using MiodenusAnimationConverter.Exceptions;
using OpenTK.Graphics.OpenGL;

namespace MiodenusAnimationConverter
{
    public class ProgramInitializer
    {
        public string[] InputKeys = {"fps",
                                     "bitrate",
                                     "extension"
        };

        public string AnimationFile = "";
        public string VideoFile = "";
        public string Extension = "avi";
        public int Bitrate = 10000;
        public int Fps = 60;

        private void InitializeInOut(string[] arguments)
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
        }

        private void InitializeExtension(string[] arguments)
        {
            if (arguments[2] != InputKeys[2])
            {
                throw new CommandLineArgumentsException("The third argument should be extension");
            }
                
            if(arguments.Length < 4)
            {
                throw new CommandLineArgumentsException("Extension of video can not be empty");
            }
            else
            {
                Extension = arguments[3];
            }
        }
        
        private void InitializeBitrate(string[] arguments)
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
        }
        
        private void InitializeFps(string[] arguments)
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
        
        public ProgramInitializer(string[] arguments)
        {
            InitializeInOut(arguments);
            
            if (arguments.Length > 2)
            {
                InitializeExtension(arguments);

                if (arguments.Length > 4)
                {
                    InitializeBitrate(arguments);

                    if (arguments.Length > 6)
                    {
                        InitializeFps(arguments);
                    }
                }
            }
        }
    }
}