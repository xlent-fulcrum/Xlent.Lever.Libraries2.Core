using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Xlent.Lever.Libraries2.Core.Crud.Interfaces;
using Xlent.Lever.Libraries2.Core.Storage.Model;

namespace Xlent.Lever.Libraries2.Core.Crud.PassThrough
{
    /// <inheritdoc />
    public class ReadPassThrough<TModel, TId> : IRead<TModel, TId>
    {
        private readonly IRead<TModel, TId> _nextLevel;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="nextLevel">The crud class to pass things down to.</param>
        public ReadPassThrough(IRead<TModel, TId> nextLevel)
        {
            _nextLevel = nextLevel;
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
    }
}
