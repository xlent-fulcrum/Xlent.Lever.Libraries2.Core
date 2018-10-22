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
    internal class MessageWithActivationTime<T>
    {
        public MessageWithActivationTime(T message, TimeSpan? timeSpanToWait)
        {
            Message = message;
            PostponeUntil = timeSpanToWait == null
                ? (DateTimeOffset?)null
                : DateTimeOffset.Now.Add(timeSpanToWait.Value);
        }

        public bool IsActivationTime => PostponeUntil == null || DateTimeOffset.Now > PostponeUntil;
        public T Message { get; set; }
        public DateTimeOffset? PostponeUntil { get; set; }
    }

    /// <summary>
    /// A generic interface for adding strings to a queue.
    /// </summary>
    public partial class MemoryQueue<T> : IBaseQueue, ICompleteQueue<T>
    {
        private readonly ConcurrentQueue<MessageWithActivationTime<T>> _queue;
        private readonly FailSafeQueueItemActionDelegate _failSafeQueueItemAction;
        private readonly bool _actionsCanExecuteInParallel;
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
        public MemoryQueue(string name) : this(name, null, false)
        {
        }

        /// <summary>
        /// Constructor
        /// </summary>
        public MemoryQueue(string name, FailSafeQueueItemActionDelegate failSafeQueueItemAction) : this(name, failSafeQueueItemAction, false)
        {
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <remarks>If <paramref name="actionsCanExecuteInParallel"/> is true, then you guarantee that it is possible to run many <paramref name="failSafeQueueItemAction"/> in parallel without interference.
        /// This will mean that the actions are run in parallel and the processing of the queue is faster.
        /// </remarks>
        public MemoryQueue(string name, FailSafeQueueItemActionDelegate failSafeQueueItemAction, bool actionsCanExecuteInParallel)
        {
            InternalContract.RequireNotNullOrWhiteSpace(name, nameof(name));
            Name = name;
            _queue = new ConcurrentQueue<MessageWithActivationTime<T>>();
            _failSafeQueueItemAction = failSafeQueueItemAction;
            _actionsCanExecuteInParallel = actionsCanExecuteInParallel;
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
        public Task AddMessageAsync(T message, TimeSpan? timeSpanToWait = null)
        {
            AddMessage(message, timeSpanToWait);
            return Task.CompletedTask;
        }

        /// <inheritdoc cref="AddMessageAsync" />
        public void AddMessage(T message, TimeSpan? timeSpanToWait = null)
        {
            FulcrumAssert.IsNotNull(_queue, null, $"Expected the queue ({Name}) to exist.");
            var messageWithExpiration = new MessageWithActivationTime<T>(message, timeSpanToWait);
            _queue.Enqueue(messageWithExpiration);
            StartBackgroundWorkerIfNeeded();
        }

        /// <inheritdoc />
        public async Task ClearAsync()
        {
            if (_queue == null) return;
            while (_queue.TryDequeue(out MessageWithActivationTime<T> _))
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
                    ThreadHelper.FireAndForgetIgnoreContext(async () => await BackgroundWorker().ConfigureAwait(false));
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
                    var queueStaysEmpty = !await ItemsWereAddedWithinTimespanAsync(TimeSpan.FromSeconds(1.0));
                    if (queueStaysEmpty)
                    {
                        // Give up
                        lock (_queue)
                        {
                            if (!_queue.IsEmpty) continue;
                            _hasBackgroundWorker = false;
                            successfulExecution = true;
                        }
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
            while (!_queue.IsEmpty)
            {
                var message = await GetOneMessageNoBlockAsync();
                if (!_queue.IsEmpty && Equals(message, default(T)))
                {
                    // There seems to be postponed items on the queue. Make a pause.
                    await Task.Delay(TimeSpan.FromSeconds(1.0));
                }
                else
                {
                    if (!_actionsCanExecuteInParallel)
                    {
                        // We need to await earlier actions before we can start the next action
                        await Task.WhenAll(taskList);
                        taskList.Clear();
                    }
                    var task = _failSafeQueueItemAction(message);
                    taskList.Add(task);
                }
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
        public async Task<T> GetOneMessageNoBlockAsync()
        {
            var triedItems = new List<MessageWithActivationTime<T>>();
            while (true)
            {
                if (!_queue.TryDequeue(out var messageWithActivationTime)) return default(T);
                if (triedItems.Contains(messageWithActivationTime))
                {
                    // We have looped through all items in the queue
                    _queue.Enqueue(messageWithActivationTime);
                    return default(T);
                }
                if (messageWithActivationTime.IsActivationTime)
                {
                    // We have found the first message in the queue that should be activated now
                    return await Task.FromResult(messageWithActivationTime.Message);
                }
                _queue.Enqueue(messageWithActivationTime);
                triedItems.Add(messageWithActivationTime);
            }
        }
    }

    public partial class MemoryQueue<T> : IPeekableQueue<T>
    {
        /// <inheritdoc />
        public Task<T> PeekNoBlockAsync()
        {
            if (!_queue.TryPeek(out var item)) return Task.FromResult(default(T));
            return item.IsActivationTime ? Task.FromResult(item.Message) : Task.FromResult(default(T));
        }
    }


    public partial class MemoryQueue<T> : IResourceHealth
    {
        /// <inheritdoc />
        public async Task<HealthResponse> GetResourceHealthAsync(Tenant tenant)
        {
            return await Task.FromResult(new HealthResponse("MemoryQueue"));
        }
    }
}
