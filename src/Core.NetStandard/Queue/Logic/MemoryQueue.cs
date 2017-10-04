using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xlent.Lever.Libraries2.Core.Application;
using Xlent.Lever.Libraries2.Core.Assert;
using Xlent.Lever.Libraries2.Core.Health.Model;
using Xlent.Lever.Libraries2.Core.MultiTenant.Model;
using Xlent.Lever.Libraries2.Core.Queue.Model;
using Xlent.Lever.Libraries2.Core.Threads;

// ReSharper disable RedundantExtendsListEntry

namespace Xlent.Lever.Libraries2.Core.Queue.Logic
{
    /// <summary>
    /// A generic interface for adding strings to a queue.
    /// </summary>
    public partial class MemoryQueue<T> : IBaseQueue
    {
        private readonly ConcurrentQueue<T> _queue;
        private readonly FailSafeQueueItemActionDelegate _failSafeQueueItemAction;
        private bool _hasBackgroundWorker;

        /// <summary>
        /// Delegate for the action to take on every queue item.
        /// The delegate must never fail.
        /// </summary>
        /// <param name="item">A queue item.</param>
        public delegate Task FailSafeQueueItemActionDelegate(T item);

        /// <summary>
        /// Constructor
        /// </summary>
        public MemoryQueue(string name)
        {
            InternalContract.RequireNotNullOrWhitespace(name, nameof(name));
            Name = name;
            _queue = new ConcurrentQueue<T>();
        }

        /// <summary>
        /// Constructor
        /// </summary>
        public MemoryQueue(string name, FailSafeQueueItemActionDelegate failSafeQueueItemAction) : this(name)
        {
            _failSafeQueueItemAction = failSafeQueueItemAction;
        }

        /// <summary>
        /// This is a property specifically for unit testing.
        /// </summary>
        // ReSharper disable once InconsistentNaming
        public bool OnlyForUnitTest_HasBackgroundWorkerForLogging
        {
            get
            {
                FulcrumAssert.IsTrue(FulcrumApplication.IsInDevelopment, null,
                    "This property must only be used in unit tests.");
                return _hasBackgroundWorker;
            }
        }

        /// <inheritdoc />
        public string Name { get; }
    }

    public partial class MemoryQueue<T> : IWritableQueue<T>
    {
        /// <inheritdoc />
        public async Task AddMessageAsync(T message, TimeSpan? timeSpanToWait = null)
        {
            FulcrumAssert.IsNotNull(_queue, null, $"Expected the queue ({Name}) to exist.");
            _queue.Enqueue(message);
            StartBackgroundWorkerIfNeeded();
            await Task.Yield();
        }

        /// <inheritdoc />
        public async Task ClearAsync()
        {
            if (_queue == null) return;
            while (_queue.TryDequeue(out T _))
            {
            }
            await Task.Yield();
        }

        /// <summary>
        /// Starts a background worker if needed. The background worker will take log messages from the queue until it is empty.
        /// </summary>
        private void StartBackgroundWorkerIfNeeded()
        {
            if (_failSafeQueueItemAction == null) return;
            try
            {
                lock (_queue)
                {
                    if (_queue.IsEmpty || _hasBackgroundWorker) return;
                    _hasBackgroundWorker = true;
                    ThreadHelper.FireAndForget(async () => await BackgroundWorker().ConfigureAwait(false));
                }
            }
            catch (Exception e)
            {
                _hasBackgroundWorker = false;
                Logging.Log.LogError("Could not start background worker for logging", e);
            }
        }

        private async Task BackgroundWorker()
        {
            var successfulExecution = false;
            try
            {
                while (true)
                {
                    await CallCallbackUntilQueueIsEmpty();
                    if (await ItemsWereAddedWithinTimespanAsync(TimeSpan.FromSeconds(1.0))) continue;
                    lock (_queue)
                    {
                        if (!_queue.IsEmpty) continue;
                        _hasBackgroundWorker = false;
                        successfulExecution = true;
                        return;
                    }
                }
            }
            finally
            {
                if (!successfulExecution) _hasBackgroundWorker = false;
            }
        }

        private async Task CallCallbackUntilQueueIsEmpty()
        {
            var taskList = new List<Task>();
            while (_queue.TryDequeue(out T item))
            {
                var task = _failSafeQueueItemAction(item);
                taskList.Add(task);
            }
            await Task.WhenAll(taskList);
        }

        private async Task<bool> ItemsWereAddedWithinTimespanAsync(TimeSpan timeSpan)
        {
            var deadline = DateTimeOffset.Now.Add(timeSpan);
            while (DateTimeOffset.Now < deadline)
            {
                if (!_queue.IsEmpty) return true;
                await Task.Delay(TimeSpan.FromMilliseconds(10));
            }
            return false;
        }
    }

    public partial class MemoryQueue<T> : IReadableQueue<T>
    {
        /// <inheritdoc />
        public Task<T> GetOneMessageNoBlockAsync()
        {
            return !_queue.TryDequeue(out T item) ? Task.FromResult(default(T)) : Task.FromResult(item);
        }
    }

    public partial class MemoryQueue<T> : IPeekableQueue<T>
    {
        /// <inheritdoc />
        public Task<T> PeekNoBlockAsync()
        {
            return !_queue.TryPeek(out T item) ? Task.FromResult(default(T)) : Task.FromResult(item);
        }
    }


    public partial class MemoryQueue<T> : IResourceHealth
    {
        /// <inheritdoc />
        public async Task<HealthResponse> GetResourceHealthAsync(ITenant tenant)
        {
            return await Task.FromResult(new HealthResponse("MemoryQueue"));
        }
    }
}
