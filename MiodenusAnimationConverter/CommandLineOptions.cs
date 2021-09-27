using CommandLine;

namespace MiodenusAnimationConverter
{
    public class CommandLineOptions
    {
        [Option('a', "animation", Required = true, HelpText = "Path to animation file.")]
        public string AnimationFilePath { get; set; }

        [Option('q', "quiet", Required = false, HelpText = "Suppresses summary messages.")]
        public bool UseQuietMode { get; set; }
        
        [Option('v', "view", Required = false, HelpText = "Suppresses summary messages.")]
        public bool UseViewMode { get; set; }
        
        [Option('f', "frame", Required = false, HelpText = "Suppresses summary messages.")]
        public int FrameIndex { get; set; }
    }
}