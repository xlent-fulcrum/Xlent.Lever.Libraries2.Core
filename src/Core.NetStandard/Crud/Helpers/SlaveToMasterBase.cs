using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Xlent.Lever.Libraries2.Core.Crud.Interfaces;
using Xlent.Lever.Libraries2.Core.Crud.Model;
using Xlent.Lever.Libraries2.Core.Storage.Model;

namespace Xlent.Lever.Libraries2.Core.Crud.Helpers
{
    /// <inheritdoc cref="SlaveToMasterCompleteBase{TModelCreate,TModel,TId}" />
    public abstract class SlaveToMasterBase<TModel, TId> :
        SlaveToMasterBase<TModel, TModel, TId>,
        ISlaveToMaster<TModel, TId>
    {
    }

    /// <summary>
    /// Abstract base class that has a default implementation for <see cref="ReadChildrenAsync"/>
    /// and <see cref="DeleteChildrenAsync"/>.
    /// </summary>
    public abstract class SlaveToMasterBase<TModelCreate, TModel, TId> :
        ISlaveToMaster<TModelCreate, TModel, TId>
        where TModel : TModelCreate
    {
        /// <inheritdoc />
        public abstract Task<PageEnvelope<TModel>> ReadChildrenWithPagingAsync(TId parentId, int offset, int? limit = null,
            CancellationToken token = default(CancellationToken));

        /// <inheritdoc />
        public Task<IEnumerable<TModel>> ReadChildrenAsync(TId parentId, int limit = int.MaxValue, CancellationToken token = default(CancellationToken))
        {
            return StorageHelper.ReadPagesAsync((offset, ct) => ReadChildrenWithPagingAsync(parentId, offset, null, ct), limit, token);
        }

        /// <inheritdoc />
        public abstract Task<SlaveToMasterId<TId>> CreateAsync(TId masterId, TModelCreate item,
            CancellationToken token = default(CancellationToken));

        /// <inheritdoc />
        public abstract Task<TModel> CreateAndReturnAsync(TId masterId, TModelCreate item,
            CancellationToken token = default(CancellationToken));

        /// <inheritdoc />
        public abstract Task CreateWithSpecifiedIdAsync(SlaveToMasterId<TId> id, TModelCreate item,
            CancellationToken token = default(CancellationToken));

        /// <inheritdoc />
        public abstract Task<TModel> CreateWithSpecifiedIdAndReturnAsync(SlaveToMasterId<TId> id, TModelCreate item,
            CancellationToken token = default(CancellationToken));

        /// <inheritdoc />
        public abstract Task DeleteChildrenAsync(TId masterId, CancellationToken token = default(CancellationToken));
    }
}
