using System;
using System.Collections.Generic;
using System.Linq;
using CommandLine;
using CommandLine.Text;

namespace MiodenusAnimationConverter
{
    public static class Program
    {
        public const string Version = "1.0";
        public const string Year = "2021";
        public static int ExitCode;

        public static int Main(string[] args)
        {
            var parser = new Parser(with =>
            {
                with.HelpWriter = null;
                with.IgnoreUnknownArguments = false;
            });
            var parserResult = parser.ParseArguments<CommandLineOptions>(args);
            
            parserResult.WithParsed(Run).WithNotParsed(errs => DisplayHelp(parserResult, errs));
            
            return ExitCode;
        }
        
        private static void Run(CommandLineOptions options)
        {
            if (options.IsValid)
            {
                var mainController = new MainController(options);
            }
            else
            {
                ExitCode = 1;
            }
        }

        private static void DisplayHelp<T>(ParserResult<T> result, IEnumerable<Error> errs)
        {
            var errors = errs.ToList();
            var helpText = HelpText.AutoBuild(result, help =>
            {
                help.AddNewLineBetweenHelpSections = true;
                help.AdditionalNewLineAfterOption = false;
                help.Heading = $"MiodenusAnimationConverter {Version}";
                help.Copyright = $"Copyright (C) {Year} roman656, PoorMercymain";
                return HelpText.DefaultParsingErrorsHandler(result, help);
            }, example => example);
            
            if (!errors.IsVersion() && !errors.IsHelp())
            {
                ExitCode = 1;
                Console.Error.WriteLine(helpText);
            }
            else
            {
                Console.WriteLine(helpText);
            }
        }
    }
}