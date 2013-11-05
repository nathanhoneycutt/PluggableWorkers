using System;
using System.Linq;
using NUnit.Framework;
using Should;
using SpecsFor;
using SpecsFor.ShouldExtensions;
using StructureMap;

namespace PluggableWorkers.Tests
{
    public class PluggableWorkerHostSpecs
    {
        public class when_configured_for_valid_workers : SpecsFor<PluggableWorkerHost>
        {
            protected Container Container;
            private bool _result;

            protected override void InitializeClassUnderTest()
            {
                Container = new Container();

                SUT = PluggableWorkerHost.CreateHost(@"TestPluggableWorkerHostConfig.xml", Container);
            }

            protected override void When()
            {
                _result = SUT.Invoke();
            }

            [Test]
            public void then_workers_executed_without_failure()
            {
                _result.ShouldEqual(true);
            }

            [Test]
            public void then_all_workers_were_configured_from_the_config_file()
            {
                SUT.ObjectContainer.Model.InstancesOf<IDoWork>().Count().ShouldEqual(5);
            }

            [Test]
            public void then_each_setting_was_used_once()
            {
                SUT.ObjectContainer
                    .GetAllInstances<IDoWork>()
                    .Cast<TestWorker>()
                    .Select(w => w.WorkerSettings)
                    .ToArray()
                    .ShouldLookLike(new[]
                                        {
                                            new TestWorker.Settings { StringValue = "String Value 1", IntValue = 1, DateValue = new DateTime(2012,1,1) },
                                            new TestWorker.Settings { StringValue = "String Value 2", IntValue = 2, DateValue = new DateTime(2012,2,2) },
                                            new TestWorker.Settings { StringValue = "Testing constant \"today\"", IntValue = 0, DateValue = DateTime.Today },
                                            new TestWorker.Settings { StringValue = "Testing constant \"yesterday\"", IntValue = 0, DateValue = DateTime.Today.AddDays(-1) },
                                            new TestWorker.Settings { StringValue = "Testing constant \"tomorrow\"", IntValue = 0, DateValue = DateTime.Today.AddDays(1) },
                                        });
            }

            [Test]
            public void then_the_external_container_is_not_altered()
            {
                Container.Model.InstancesOf<IDoWork>().Count().ShouldEqual(0);
            }
        }

        public class when_configuring_worker_host_with_a_worker_that_does_not_exist : SpecsFor<PluggableWorkerHost>
        {
            protected override void InitializeClassUnderTest() {}

            [Test]
            public void then_there_is_an_exception_for_the_bad_worker()
            {
                Assert.Throws<ArgumentException>(() => PluggableWorkerHost.CreateHost("FailingPluggableWorkerHostConfig.xml"));
            }
        }

        public class when_configuring_worker_with_no_settings : SpecsFor<PluggableWorkerHost>
        {
            protected override void InitializeClassUnderTest()
            {
                SUT = PluggableWorkerHost.CreateHost(@"SettinglessConfig.xml");
            }

            [Test]
            public void then_the_worker_runs()
            {
                SUT.Invoke().ShouldBeTrue();
            }
        }

        public class when_configuring_worker_host_with_an_invalid_config_file : SpecsFor<PluggableWorkerHost>
        {
            protected override void InitializeClassUnderTest() {}

            [Test]
            public void then_there_is_an_exception_describing_the_correct_config_file_format()
            {
                Assert.Throws<Exception>(() => PluggableWorkerHost.CreateHost("InvalidPluggableWorkerHostConfig.xml"))
                      .Message.ShouldContainAll("<pluggableWorkers>", "<workers>", "<worker", "</workers>",
                                                "</pluggableWorkers>");
            }
        }
    }
}
