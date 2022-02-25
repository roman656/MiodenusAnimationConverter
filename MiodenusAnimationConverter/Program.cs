using System;
using System.Collections.Generic;
using System.Linq;
using CommandLine;
using CommandLine.Text;

namespace MiodenusAnimationConverter
{
    public static class Program
    {
        public static ExitCodeEnum ExitCode = ExitCodeEnum.Success;
        
        public static int Main(string[] args)
        {
            var parser = new Parser(with => with.HelpWriter = null);
            var parserResult = parser.ParseArguments<CommandLineOptions>(args);
            
            parserResult.WithParsed(_ => Run(parserResult))
                        .WithNotParsed(errs => DisplayHelp(parserResult, errs));
            
            return (byte)ExitCode;
        }
        
        private static void Run(ParserResult<CommandLineOptions> result)
        {
            if (result.Value.IsValid)
            {
                var mainController = new MainController(result.Value);
            }
            else
            {
                var helpText = GenerateHelpText(result);
                
                /* TODO: убрать костыль. */
                helpText.Copyright += "\n\nERROR(S):\n  Incorrect value for some options.\n";
                ExitCode = ExitCodeEnum.WrongCommandLineOptionValue;
                Console.Error.WriteLine(helpText);
            }
        }

        private static void DisplayHelp(ParserResult<CommandLineOptions> result, IEnumerable<Error> errs)
        {
            var errors = errs.ToList();
            var helpText = GenerateHelpText(result);

            if (errors.IsHelp())
            {
                Console.WriteLine(helpText);
            }
            else if (errors.IsVersion())
            {
                Console.WriteLine(helpText.Heading);
            }
            else
            {
                ExitCode = ExitCodeEnum.WrongCommandLineOptionValue;
                Console.Error.WriteLine(helpText);
            }
        }

        private static HelpText GenerateHelpText(ParserResult<CommandLineOptions> result)
        {
            return HelpText.AutoBuild(result, help =>
            {
                help.AddNewLineBetweenHelpSections = true;
                help.AdditionalNewLineAfterOption = false;
                help.Heading = $"MiodenusAnimationConverter {Config.Version}";
                help.Copyright = $"Copyright (C) {Config.Year} {Config.Authors}";
                return HelpText.DefaultParsingErrorsHandler(result, help);
            }, example => example);
        }
    }
}