﻿using System;
using System.Threading;
using System.Threading.Tasks;
using Foundatio.Extensions;
using Foundatio.Jobs;
using Foundatio.Metrics;
using Foundatio.Queues;
using Foundatio.ServiceProviders;
using Xunit;

namespace Foundatio.Tests.Jobs {
    public class JobTests {
        [Fact]
        public void CanRunJobs() {
            var job = new HelloWorldJob();
            Assert.Equal(0, job.RunCount);
            job.Run();
            Assert.Equal(1, job.RunCount);

            job.RunContinuous(iterationLimit: 2);
            Assert.Equal(3, job.RunCount);

            job.RunContinuous(token: new CancellationTokenSource(TimeSpan.FromMilliseconds(10)).Token);
            Assert.True(job.RunCount > 10);

            var jobInstance = JobRunner.CreateJobInstance(typeof(HelloWorldJob).AssemblyQualifiedName);
            Assert.NotNull(jobInstance);
            Assert.Equal(0, ((HelloWorldJob)jobInstance).RunCount);
            Assert.Equal(JobResult.Success, jobInstance.Run());
            Assert.Equal(1, ((HelloWorldJob)jobInstance).RunCount);
        }

        [Fact]
        public void CanRunJobsWithLocks() {
            var job = new WithLockingJob();
            Assert.Equal(0, job.RunCount);
            job.Run();
            Assert.Equal(1, job.RunCount);

            job.RunContinuous(iterationLimit: 2);
            Assert.Equal(3, job.RunCount);

            Task.Run(() => job.Run());
            Task.Run(() => job.Run());
            Thread.Sleep(200);
            Assert.Equal(4, job.RunCount);
        }

        [Fact]
        public void CanBootstrapJobs() {
            ServiceProvider.SetServiceProvider(typeof(JobTests));
            Assert.NotNull(ServiceProvider.Current);
            Assert.Equal(ServiceProvider.Current.GetType(), typeof(MyBootstrappedServiceProvider));

            ServiceProvider.SetServiceProvider(typeof(MyBootstrappedServiceProvider));
            Assert.NotNull(ServiceProvider.Current);
            Assert.Equal(ServiceProvider.Current.GetType(), typeof(MyBootstrappedServiceProvider));

            var job = ServiceProvider.Current.GetService<WithDependencyJob>();
            Assert.NotNull(job);
            Assert.NotNull(job.Dependency);
            Assert.Equal(5, job.Dependency.MyProperty);

            var jobInstance = JobRunner.CreateJobInstance("Foundatio.Tests.Jobs.HelloWorldJob,Foundatio.Tests");
            Assert.NotNull(job);
            Assert.NotNull(job.Dependency);
            Assert.Equal(5, job.Dependency.MyProperty);

            jobInstance = JobRunner.CreateJobInstance("Foundatio.Tests.Jobs.HelloWorldJob,Foundatio.Tests", "Foundatio.Tests.Jobs.MyBootstrappedServiceProvider,Foundatio.Tests");
            Assert.NotNull(job);
            Assert.NotNull(job.Dependency);
            Assert.Equal(5, job.Dependency.MyProperty);

            var result = jobInstance.Run();
            Assert.Equal(true, result.IsSuccess);
            Assert.True(jobInstance is HelloWorldJob);
        }

        [Fact]
        public void CanRunQueueJob() {
            const int workItemCount = 1000;
            var metrics = new InMemoryMetricsClient();
            var queue = new InMemoryQueue<SampleQueueWorkItem>(0, TimeSpan.Zero, metrics: metrics);

            for (int i = 0; i < workItemCount; i++)
                queue.Enqueue(new SampleQueueWorkItem { Created = DateTime.Now, Path = "somepath" + i });

            var job = new SampleQueueJob(queue, metrics);
            job.RunUntilEmpty();
            metrics.DisplayStats();

            Assert.Equal(0, queue.GetQueueCount());
        }
    }
}
