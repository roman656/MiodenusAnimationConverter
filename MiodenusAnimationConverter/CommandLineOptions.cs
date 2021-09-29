using System.Collections.Generic;
using CommandLine;
using CommandLine.Text;

namespace MiodenusAnimationConverter
{
    public class CommandLineOptions
    {
        private int _frameNumberToView;
        private int _frameNumberToGetImage;
        public bool WasFrameNumberToViewOptionGot;
        public bool WasFrameNumberToGetImageOptionGot;
        
        [Option('a', "animation", Required = true, HelpText = "Path to animation file.")]
        public string AnimationFilePath { get; set; }

        [Option('q', "quiet", HelpText = "Suppresses summary messages.")]
        public bool UseQuietMode { get; set; }

        [Option('v', "view", SetName = "ViewMode", HelpText = "Frame number that will be uploaded for viewing.")]
        public int FrameNumberToView
        {
            get => _frameNumberToView;
            set
            {
                WasFrameNumberToViewOptionGot = true;
                _frameNumberToView = value;
            }
        }

        [Option('f', "frame", SetName = "GetImageMode", HelpText = "Frame number that will be saved as an image file.")]
        public int FrameNumberToGetImage
        {
            get => _frameNumberToGetImage;
            set
            {
                WasFrameNumberToGetImageOptionGot = true;
                _frameNumberToGetImage = value;
            }
        }
        
        [Usage]
        public static IEnumerable<Example> Examples => new List<Example>
        {
            new ("Simple run", new CommandLineOptions
            {
                AnimationFilePath = "path/to/animation.maf"
            }),
            new ("Run quietly", new CommandLineOptions
            { 
                AnimationFilePath = "path/to/animation.maf",
                UseQuietMode = true
            })
        };

        public bool IsValid => AnimationFilePath != string.Empty && FrameNumberToView >= 0
                && (!WasFrameNumberToGetImageOptionGot || FrameNumberToGetImage > 0);
    }
}