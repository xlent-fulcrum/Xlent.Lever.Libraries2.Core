using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Xlent.Lever.Libraries2.Core.Crud.Interfaces;
using Xlent.Lever.Libraries2.Core.Storage.Model;

namespace Xlent.Lever.Libraries2.Core.Crud.Helpers
{
    /// <summary>
    /// Abstract base class that has a default implementation for <see cref="ReadAllAsync"/>.
    /// </summary>
    public abstract class ManyToOneReadBase<TModel, TId> :
        ManyToOneBase<TModel, TId>,
        IManyToOneRead<TModel, TId>
    {
        /// <inheritdoc />
        public abstract Task<TModel> ReadAsync(TId id, CancellationToken token = default(CancellationToken));

        /// <inheritdoc />
        public abstract Task<PageEnvelope<TModel>> ReadAllWithPagingAsync(int offset, int? limit = null,
            CancellationToken token = default(CancellationToken));

        /// <inheritdoc />
        public virtual async Task<IEnumerable<TModel>> ReadAllAsync(int limit = Int32.MaxValue, CancellationToken token = default(CancellationToken))
        {
            return await StorageHelper.ReadPagesAsync((offset, ct) => ReadAllWithPagingAsync(offset, null, ct), limit, token);
        }
    }
}
