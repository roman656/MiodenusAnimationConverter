using CommandLine;

namespace MiodenusAnimationConverter
{
    public class CommandLineOptions
    {
        [Option('a', "animation", Required = true, HelpText = "Path to animation file.")]
        public string AnimationFilePath { get; set; }

        [Option('q', "quiet", Required = false, HelpText = "Suppresses summary messages.")]
        public bool UseQuietMode { get; set; }
        
        [Option('v', "view", Required = false, Default = -1, HelpText = "Frame number that will be uploaded for viewing.")]
        public int FrameNumberToView { get; set; }
        
        [Option('f', "frame", Required = false, Default = -1, HelpText = "Frame number that will be saved as an image file.")]
        public int FrameNumberToGetImage { get; set; }
    }
}