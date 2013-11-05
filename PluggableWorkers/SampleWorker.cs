using System;
using System.Linq;

namespace PluggableWorkers
{
    public class SampleWorker : IDoWork
    {
        //NOTE: This test worker is not intended to do anything, and should not be configured in a live environment.  It is included
        //only to demonstrate how workers and their settings need to be organized for the pluggable worker host.
        private readonly Settings _settings;

        public SampleWorker(Settings settings)
        {
            _settings = settings;
        }

        public bool Invoke()
        {
            Console.WriteLine("SampleWorker.Invoke()");
            Console.WriteLine("\t Test String: {0}", _settings.TestString);
            Console.WriteLine("\t Test Ints ({0})", _settings.TestInts.Length);
            _settings.TestInts.ToList().ForEach(x => Console.WriteLine("\t\t{0}", x));

            return true;
        }

        public class Settings
        {
            public string TestString { get; set; }
            public int[] TestInts { get; set; }
        }
    }
}