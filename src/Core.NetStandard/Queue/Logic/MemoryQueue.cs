using System;
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
        private static readonly string Namespace = typeof(MemoryQueue<T>).Namespace;
        private readonly object _lockObject = new object();
        private readonly Queue<T> _queue;

        /// <summary>
        /// Constructor
        /// </summary>
        public MemoryQueue(string name)
        {
            InternalContract.RequireNotNullOrWhitespace(name, nameof(name));
            Name = name;
            _queue = new Queue<T>();
        }

        /// <inheritdoc />
        public string Name { get; }
    }

    public partial class MemoryQueue<T> : IWritableQueue<T>
    {

        /// <inheritdoc />
        public async Task AddMessageAsync(T message, TimeSpan? timeSpanToWait = null)
        {
            lock (_lockObject)
            {
                FulcrumAssert.IsNotNull(_queue, $"{Namespace}: 9BD616FD-3867-453C-93C6-13B4767A6FE5",
                    $"Expected the queue ({Name}) to exist. Did you forget to call MaybeCreateAndConnect()?");
                _queue.Enqueue(message);
            }
            await Task.Yield();
        }

        /// <inheritdoc />
        public async Task ClearAsync()
        {
            lock (_lockObject)
            {
                if (_queue == null) return;
                _queue.Clear();
            }
            await Task.Yield();
        }
    }

    public partial class MemoryQueue<T> : IReadableQueue<T>
    {

        /// <inheritdoc />
        public Task<T> GetOneMessageNoBlockAsync()
        {
            lock (_lockObject)
            {
                if (!_queue.Any()) return null;
                return Task.FromResult(_queue.Dequeue());
            }
        }
    }

    public partial class MemoryQueue<T> : IPeekableQueue<T>
    {

        /// <inheritdoc />
        public Task<T> PeekNoBlockAsync()
        {
            lock (_lockObject)
            {
                return Task.FromResult(_queue.FirstOrDefault());
            }
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
