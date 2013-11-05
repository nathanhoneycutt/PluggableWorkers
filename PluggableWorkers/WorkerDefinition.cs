using System;
using System.Collections.Generic;

namespace PluggableWorkers
{
    public class WorkerDefinition
    {
        public Type WorkerType { get; set; }
        public Dictionary<string, string> Parameters { get; set; }
    }
}