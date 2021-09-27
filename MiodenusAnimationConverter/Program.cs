using CommandLine;

namespace MiodenusAnimationConverter
{
    public static class Program
    {
        public static int Main(string[] args)
        {
            return Parser.Default.ParseArguments<CommandLineOptions>(args).MapResult(RunAndReturnExitCode, _ => 1);
        }
        
        private static int RunAndReturnExitCode(CommandLineOptions options)
        {
            var mainController = new MainController(options);
            return mainController.ExitCode;
        }
    }
}