using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Xlent.Lever.Libraries2.Core.Crud.Interfaces;
using Xlent.Lever.Libraries2.Core.Storage.Model;

namespace Xlent.Lever.Libraries2.Core.Crud.PassThrough
{
    /// <inheritdoc />
    public class RudPassThrough<TModel, TId> : ReadPassThrough<TModel, TId>, IRud<TModel, TId>
    {
        private readonly IRud<TModel, TId> _nextLevel;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="nextLevel">The crud class to pass things down to.</param>
        public RudPassThrough(IRud<TModel, TId> nextLevel)
            : base(nextLevel)
        {
            _nextLevel = nextLevel;
        }

        /// <inheritdoc />
        public virtual Task DeleteAllAsync(CancellationToken token = default(CancellationToken))
        {
            return _nextLevel.DeleteAllAsync(token);
        }

        /// <inheritdoc />
        public virtual Task DeleteAsync(TId id, CancellationToken token = default(CancellationToken))
        {
            return _nextLevel.DeleteAsync(id, token);
        }

        /// <inheritdoc />
        public virtual Task<TModel> UpdateAndReturnAsync(TId id, TModel item, CancellationToken token = default(CancellationToken))
        {
            return _nextLevel.UpdateAndReturnAsync(id, item, token);
        }

        /// <inheritdoc />
        public virtual Task UpdateAsync(TId id, TModel item, CancellationToken token = default(CancellationToken))
        {
            return _nextLevel.UpdateAsync(id, item, token);
        }

        /// <inheritdoc />
        public virtual Task<Lock> ClaimLockAsync(TId id, CancellationToken token = default(CancellationToken))
        {
            return _nextLevel.ClaimLockAsync(id, token);
        }

        /// <inheritdoc />
        public virtual Task ReleaseLockAsync(Lock @lock, CancellationToken token = default(CancellationToken))
        {
            return _nextLevel.ReleaseLockAsync(@lock, token);
        }
    }
}
