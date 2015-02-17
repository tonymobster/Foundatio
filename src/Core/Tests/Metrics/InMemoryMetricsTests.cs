using System.Diagnostics;
using Foundatio.Metrics;
using Xunit;

namespace Foundatio.Tests.Metrics {
    public class InMemoryMetricsTests {
        [Fact]
        public void CanIncrementCounter() {
            var metrics = new InMemoryMetricsClient();
            metrics.Counter("c1");
            Assert.Equal(1, metrics.GetCount("c1"));

            metrics.Counter("c1", 5);
            Assert.Equal(6, metrics.GetCount("c1"));

            var counter = metrics.Counters["c1"];
            var hitsPerSec = counter.Value / counter.GetElapsedTime().TotalSeconds;
            //Debug.WriteLine(hitsPerSec);
            Assert.InRange(hitsPerSec, 500, 2000);

            metrics.Gauge("g1", 2.534);
            Assert.Equal(2.534, metrics.GetGaugeValue("g1"));

            metrics.Timer("t1", 50788);

            metrics.DisplayStats();
            metrics.DisplayStats();
        }
    }
}