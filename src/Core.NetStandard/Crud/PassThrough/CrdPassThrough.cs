using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Xlent.Lever.Libraries2.Core.Crud.Interfaces;
using Xlent.Lever.Libraries2.Core.Storage.Model;

namespace Xlent.Lever.Libraries2.Core.Crud.PassThrough
{
    /// <inheritdoc cref="CrdPassThrough{TModelCreate,TModel,TId}" />
    public class CrdPassThrough<TModel, TId> : CrdPassThrough<TModel, TModel, TId>, ICrd<TModel, TId>
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="nextLevel">The crud class to pass things down to.</param>
        public CrdPassThrough(ICrd<TModel, TId> nextLevel)
        :base(nextLevel)
        {
        }
    }

    /// <inheritdoc cref="ReadPassThrough{TModel,TId}" />
    public class CrdPassThrough<TModelCreate, TModel, TId> : ReadPassThrough<TModel, TId>, ICrd<TModelCreate, TModel, TId> where TModel : TModelCreate
    {
        private readonly ICrd<TModelCreate, TModel, TId> _nextLevel;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="nextLevel">The crud class to pass things down to.</param>
        public CrdPassThrough(ICrd<TModelCreate, TModel, TId> nextLevel)
            : base(nextLevel)
        {
            _nextLevel = nextLevel;
        }

        /// <inheritdoc />
        public virtual Task<TModel> CreateAndReturnAsync(TModelCreate item, CancellationToken token = default(CancellationToken))
        {
            return _nextLevel.CreateAndReturnAsync(item, token);
        }

        /// <inheritdoc />
        public virtual Task<TId> CreateAsync(TModelCreate item, CancellationToken token = default(CancellationToken))
        {
            return _nextLevel.CreateAsync(item, token);
        }

        /// <inheritdoc />
        public virtual Task CreateWithSpecifiedIdAsync(TId id, TModelCreate item, CancellationToken token = default(CancellationToken))
        {
            return _nextLevel.CreateWithSpecifiedIdAsync(id, item, token);
        }

        /// <inheritdoc />
        public virtual Task<TModel> CreateWithSpecifiedIdAndReturnAsync(TId id, TModelCreate item,
            CancellationToken token = default(CancellationToken))
        {
            return _nextLevel.CreateWithSpecifiedIdAndReturnAsync(id, item, token);
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
