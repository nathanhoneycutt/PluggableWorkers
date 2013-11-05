using CommandLine;
using CommandLine.Text;

namespace PluggableWorkers.ConsoleRunner
{
    public class CommandLineOptions
    {
        [Option('c', "configFile", Required = false, HelpText = "Configuration file to load if not the standard one.")]
        public string ConfigruationFile { get; set; }

        [HelpOption]
        public string GetUsage()
        {
            var help = new HelpText
                           {
                                Heading = "Pluggable Workers Runner",
                                Copyright = new CopyrightInfo("Lazy Developers Group, LLC", 2012),
                                AddDashesToOption = true
                           };
            
            help.AddPreOptionsLine("This application supports the following command line arguments.");
            help.AddPostOptionsLine("Short parameters must be prefixed with a single dash, long names by two dashes.");
            help.AddPostOptionsLine("Parameters values of more than one word must be enclosed in quotes.");
            help.AddPostOptionsLine("\n\n");
            help.AddOptions(this);

            return help;
        }
    }
}