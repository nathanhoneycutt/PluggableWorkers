using System;

namespace PluggableWorkers.Tests
{
    public class SettinglessTestWorker : IDoWork
    {
        public bool Invoke()
        {
            Console.WriteLine("Managed to create the worker without having to specify settings.");
            return true;
        }
    }
}
