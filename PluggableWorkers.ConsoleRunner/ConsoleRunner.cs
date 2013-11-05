using System;
using StructureMap;

namespace PluggableWorkers.ConsoleRunner
{
    /// <remarks>
    /// A barebones way to run PluggableWorkers. You could use this if your needs are pretty basic, but
    /// a better use would be as an example of how to configure the PluggableWorkersHost.
    /// </remarks>
    public class ConsoleRunner
    {
        public static void Main(string[] args)
        {
            string configFileName = null;
            var commandLineOptions = new CommandLineOptions();

            if (!CommandLine.Parser.Default.ParseArguments(args, commandLineOptions))
                return;

            if (!String.IsNullOrEmpty(commandLineOptions.ConfigruationFile))
                configFileName = commandLineOptions.ConfigruationFile;

            var host = PluggableWorkerHost.CreateHost(configFileName, ObjectFactory.Container);
            host.Invoke();
        }
    }
}
