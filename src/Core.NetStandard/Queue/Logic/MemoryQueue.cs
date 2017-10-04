using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xlent.Lever.Libraries2.Core.Assert;
using Xlent.Lever.Libraries2.Core.Health.Model;
using Xlent.Lever.Libraries2.Core.MultiTenant.Model;
using Xlent.Lever.Libraries2.Core.Queue.Model;

// ReSharper disable RedundantExtendsListEntry

namespace Xlent.Lever.Libraries2.Core.Queue.Logic
{
    /// <summary>
    /// A generic interface for adding strings to a queue.
    /// </summary>
    public partial class MemoryQueue<T> : IBaseQueue
    {
        private readonly ConcurrentQueue<T> _queue;
        //private Func<Task<T>> _callback;

        /// <summary>
        /// Constructor
        /// </summary>
        public MemoryQueue(string name)
        {
            InternalContract.RequireNotNullOrWhitespace(name, nameof(name));
            Name = name;
            _queue = new ConcurrentQueue<T>();
        }

        ///// <summary>
        ///// Constructor
        ///// </summary>
        //public MemoryQueue(string name, Func<Task<T>> callback) :this(name)
        //{
        //    _callback = callback;
        //}

        /// <inheritdoc />
        public string Name { get; }
    }

    public partial class MemoryQueue<T> : IWritableQueue<T>
    {
        /// <inheritdoc />
        public async Task AddMessageAsync(T message, TimeSpan? timeSpanToWait = null)
        {
            FulcrumAssert.IsNotNull(_queue, null, $"Expected the queue ({Name}) to exist. Did you forget to call MaybeCreateAndConnect()?");
            _queue.Enqueue(message);
            //if (_callback != null)
            //{
                
            //}
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
    }

    public partial class MemoryQueue<T> : IReadableQueue<T>
    {
        /// <inheritdoc />
        public Task<T> GetOneMessageNoBlockAsync()
        {
            return !_queue.TryDequeue(out T item) ? Task.FromResult(default(T)): Task.FromResult(item);
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
