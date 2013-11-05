using NUnit.Framework;
using Should;
using SpecsFor;

namespace PluggableWorkers.Tests
{
    /// <remarks>
    /// This test fixture isn't really useful on its own, it more serves as a demonstration of how to test pluggable
    /// workers.
    /// </remarks>
    public class SampleWorkerSpecs : SpecsFor<SampleWorker>
    {
        private bool _results;

        protected override void InitializeClassUnderTest()
        {
            var settings = new SampleWorker.Settings
                               {
                                   TestString = "Some string",
                                   TestInts = new[] {1, 2, 3}
                               };

            SUT = new SampleWorker(settings);
        }

        protected override void When()
        {
            _results = SUT.Invoke();
        }

        [Test]
        public void then_it_should_execute()
        {
            _results.ShouldEqual(true);
        }
    }
}
