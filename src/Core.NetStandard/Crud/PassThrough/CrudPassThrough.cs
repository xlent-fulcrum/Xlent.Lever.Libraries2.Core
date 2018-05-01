using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Xlent.Lever.Libraries2.Core.Crud.Interfaces;
using Xlent.Lever.Libraries2.Core.Storage.Model;

namespace Xlent.Lever.Libraries2.Core.Crud.PassThrough
{
    /// <inheritdoc cref="CrudPassThrough{TModelCreate,TModel,TId}" />
    public class CrudPassThrough<TModel, TId> : CrudPassThrough<TModel, TModel, TId>, ICrud<TModel, TId>
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="nextLevel">The crud class to pass things down to.</param>
        public CrudPassThrough(ICrud<TModel, TId> nextLevel)
        :base(nextLevel)
        {
        }
    }

    /// <inheritdoc />
    public class CrudPassThrough<TModelCreate, TModel, TId> : ICrud<TModelCreate, TModel, TId> where TModel : TModelCreate
    {
        private readonly ICrud<TModelCreate, TModel, TId> _nextLevel;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="nextLevel">The crud class to pass things down to.</param>
        public CrudPassThrough(ICrud<TModelCreate, TModel, TId> nextLevel)
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
        public virtual Task<IEnumerable<TModel>> ReadAllAsync(int limit = int.MaxValue, CancellationToken token = default(CancellationToken))
        {
            return _nextLevel.ReadAllAsync(limit, token);
        }

        /// <inheritdoc />
        public virtual Task<PageEnvelope<TModel>> ReadAllWithPagingAsync(int offset, int? limit = null, CancellationToken token = default(CancellationToken))
        {
            return _nextLevel.ReadAllWithPagingAsync(offset, limit, token);
        }

        /// <inheritdoc />
        public virtual Task<TModel> ReadAsync(TId id, CancellationToken token = default(CancellationToken))
        {
            return _nextLevel.ReadAsync(id, token);
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
    }
}
