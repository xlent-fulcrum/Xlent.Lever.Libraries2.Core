using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Xlent.Lever.Libraries2.Core.Crud.Interfaces;
using Xlent.Lever.Libraries2.Core.Crud.Model;
using Xlent.Lever.Libraries2.Core.Error.Logic;
using Xlent.Lever.Libraries2.Core.Storage.Model;

namespace Xlent.Lever.Libraries2.Core.Crud.Helpers
{
    /// <summary>
    /// Abstract base class that has a default implementation for <see cref="ReadAllAsync"/>.
    /// </summary>
    public abstract class SlaveToMasterReadBase<TModel, TId> :
        SlaveToMasterBase<TModel,TId>,
        ISlaveToMasterRead<TModel, TId>
    {
        /// <inheritdoc />
        public abstract Task<TModel> ReadAsync(SlaveToMasterId<TId> id,
            CancellationToken token = default(CancellationToken));

        /// <inheritdoc cref="IRead{TModel,TId}.ReadAllWithPagingAsync" />
        public abstract Task<PageEnvelope<TModel>> ReadAllWithPagingAsync(int offset, int? limit = null,
            CancellationToken token = default(CancellationToken));

        /// <inheritdoc cref="IRead{TModel,TId}.ReadAllAsync" />
        public virtual Task<IEnumerable<TModel>> ReadAllAsync(int limit = int.MaxValue, CancellationToken token = default(CancellationToken))
        {
            return StorageHelper.ReadPagesAsync((offset, ct) => ReadAllWithPagingAsync(offset, null, ct), limit, token);
        }
    }
}
