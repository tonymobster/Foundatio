using System;
using System.Threading.Tasks;
using Foundatio.Queues;
using Xunit;

namespace Foundatio.Tests.Queue {
    public class InMemoryQueueTests : QueueTestBase {
        private IQueue<SimpleWorkItem> _queue;

        protected override IQueue<SimpleWorkItem> GetQueue(int retries = 1, TimeSpan? workItemTimeout = null, TimeSpan? retryDelay = null, int deadLetterMaxItems = 100) {
            if (_queue == null)
                _queue = new InMemoryQueue<SimpleWorkItem>(retries, retryDelay, workItemTimeout: workItemTimeout);

            return _queue;
        }

        [Fact]
        public override void CanQueueAndDequeueWorkItem() {
            base.CanQueueAndDequeueWorkItem();
        }

        [Fact]
        public override void CanQueueAndDequeueMultipleWorkItems() {
            base.CanQueueAndDequeueMultipleWorkItems();
        }

        [Fact]
        public override void WillWaitForItem() {
            base.WillWaitForItem();
        }

        [Fact]
        public override void DequeueWaitWillGetSignaled() {
            base.DequeueWaitWillGetSignaled();
        }

        [Fact]
        public override void CanUseQueueWorker() {
            base.CanUseQueueWorker();
        }

        [Fact]
        public override Task CanHandleErrorInWorker() {
            return base.CanHandleErrorInWorker();
        }

        [Fact]
        public override void WorkItemsWillTimeout() {
            base.WorkItemsWillTimeout();
        }

        [Fact]
        public override void WorkItemsWillGetMovedToDeadletter() {
            base.WorkItemsWillGetMovedToDeadletter();
        }

        [Fact]
        public override void CanAutoCompleteWorker() {
            base.CanAutoCompleteWorker();
        }

        [Fact]
        public override void CanHaveMultipleQueueInstances() {
            base.CanHaveMultipleQueueInstances();
        }

        [Fact]
        public override void CanDelayRetry() {
            base.CanDelayRetry();
        }
    }
}