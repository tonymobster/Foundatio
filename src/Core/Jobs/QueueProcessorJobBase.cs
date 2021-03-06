﻿using System;
using System.Threading;
using System.Threading.Tasks;
using Foundatio.Queues;
using Foundatio.Utility;
using NLog.Fluent;

namespace Foundatio.Jobs {
    public abstract class QueueProcessorJobBase<T> : JobBase, IQueueProcessorJob where T : class {
        protected readonly IQueue<T> _queue;

        public QueueProcessorJobBase(IQueue<T> queue) {
            _queue = queue;
        }

        protected override Task<JobResult> RunInternalAsync(CancellationToken token) {
            QueueEntry<T> queueEntry = null;
            try {
                queueEntry = _queue.Dequeue();
            } catch (Exception ex) {
                if (!(ex is TimeoutException)) {
                    Log.Error().Exception(ex).Message("Error trying to dequeue message: {0}", ex.Message).Write();
                    return Task.FromResult(JobResult.FromException(ex));
                }
            }

            if (queueEntry == null)
                return Task.FromResult(JobResult.Success);

            var lockValue = GetQueueItemLock(queueEntry);
            if (lockValue == null) {
                Log.Warn().Message("Unable to acquire lock for queue entry '{0}'.", queueEntry.Id).Write();
                return Task.FromResult(JobResult.FailedWithMessage("Unable to acquire lock for queue entry."));
            }
            Log.Trace().Message("Processing queue entry '{0}'.", queueEntry.Id).Write();

            using (lockValue)
                return ProcessQueueItem(queueEntry);
        }

        protected virtual IDisposable GetQueueItemLock(QueueEntry<T> queueEntry) {
            return Disposable.Empty;
        }

        public void RunUntilEmpty(CancellationToken cancellationToken = default(CancellationToken)) {
            do {
                while (_queue.GetQueueCount() > 0)
                    Run();

                Thread.Sleep(100);
            } while (_queue.GetQueueCount() != 0);
        }

        protected abstract Task<JobResult> ProcessQueueItem(QueueEntry<T> queueEntry);
    }

    public interface IQueueProcessorJob : IDisposable {
        void RunUntilEmpty(CancellationToken cancellationToken = default(CancellationToken));
    }
}
