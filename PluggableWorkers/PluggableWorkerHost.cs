using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Xml.Linq;
using StructureMap;

namespace PluggableWorkers
{
    public class PluggableWorkerHost
    {
        internal readonly IContainer ObjectContainer;
        private readonly string _configFileName;
        private List<WorkerDefinition> _workersToRun;
        private XDocument _settingsFile;

        private PluggableWorkerHost(string configFileName, IContainer objectContainer)
        {
            _configFileName = configFileName;
            ObjectContainer = objectContainer;

            PopulateWorkersToRun();
            InitializeObjectFactory();
        }

        public static PluggableWorkerHost CreateHost(string configFileName = null, IContainer container = null)
        {
            var myContainer = container != null ? container.GetNestedContainer() : ObjectFactory.Container;

            myContainer.Configure(cfg => cfg.Scan(scan =>
                                                      {
                                                          scan.TheCallingAssembly();
                                                          scan.WithDefaultConventions();
                                                      }));

            return new PluggableWorkerHost(
                (configFileName ??
                 Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "PluggableWorkerHost.config"))
                , myContainer);
        }

        private void PopulateWorkersToRun()
        {
            _settingsFile = XDocument.Load(_configFileName);

            IEnumerable<XElement> settingsElements;

            try
            {
                settingsElements = _settingsFile.Element("pluggableWorkers").Element("workers").Elements("worker");
            }
            catch (Exception ex)
            {
                throw new Exception(
                    "Configuration file is not properly formatted.  The correct format is <pluggableWorkers><workers><worker/><worker/></workers></pluggableWorkers>.",
                    ex);
            }
            
            _workersToRun = settingsElements
                .Select(n =>
                            {
                                var workerType = "<unknown>";
                                try
                                {
                                    workerType = n.Attribute("type").Value;
                                    return new WorkerDefinition
                                               {
                                                   WorkerType = Type.GetType(workerType, true),
                                                   Parameters = n.Elements().ToDictionary(e => e.Name.ToString(), e => e.Value)
                                               };
                                }
                                catch (Exception ex)
                                {
                                    throw new ArgumentException(String.Format("Unable to get type for worker '{0}'", workerType), ex);
                                }
                            })
                .ToList();
        }

        private void InitializeObjectFactory()
        {
            ObjectContainer.Configure(cfg => _workersToRun.ForEach(worker =>
                                                {
                                                    var workerType = worker.WorkerType;
                                                    var parameters = worker.Parameters;
                                                    var workerName = Guid.NewGuid().ToString();

                                                    cfg.For(workerType).Use(workerType).Named(workerName);
                                                    cfg.For(typeof(IDoWork))
                                                        .Use(ctx =>
                                                                {
                                                                    var typeName = workerType.FullName + "+" + "Settings";
                                                                    var settingsType = workerType.Assembly.GetType(typeName, false);
                                                                    if (settingsType == null)
                                                                    {
                                                                        typeName = workerType.FullName + "+" + workerType.Name + "Settings";
                                                                        settingsType = workerType.Assembly.GetType(typeName, false);
                                                                    }
                                                                    if (settingsType != null)
                                                                    {
                                                                        ctx.RegisterDefault(settingsType, ctx.GetInstance<SettingsFactory>()
                                                                            .GetSettingsFor(settingsType, parameters));
                                                                    }

                                                                    return ctx.GetInstance(workerType, workerName);
                                                                });
                                              
                                                }));
        }

        public bool Invoke()
        {
            return ObjectContainer.GetAllInstances<IDoWork>().All(m => m.Invoke());
        }
    }
}