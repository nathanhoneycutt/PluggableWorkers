using System;

namespace PluggableWorkers.Tests
{
    public class TestWorker : IDoWork
    {
        public readonly Settings WorkerSettings;

        public TestWorker(Settings settings)
        {
            WorkerSettings = settings;
        }

        public bool Invoke()
        {
            return (WorkerSettings.IntValue == WorkerSettings.DateValue.Month) || (WorkerSettings.IntValue == 0);
        }

        public class Settings
        {
            public string StringValue { get; set; }
            public int IntValue { get; set; }
            public DateTime DateValue { get; set; }
        }
    }
}
