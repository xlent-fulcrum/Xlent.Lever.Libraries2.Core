using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Xlent.Lever.Libraries2.Core.Crud.Interfaces;
using Xlent.Lever.Libraries2.Core.Storage.Model;

namespace Xlent.Lever.Libraries2.Core.Crud.Helpers
{
    /// <inheritdoc cref="ManyToOneCompleteBase{TModelCreate,TModel,TId}" />
    public abstract class ManyToOneCompleteBase< TModel, TId> : ManyToOneCompleteBase<TModel, TModel, TId>,
        IManyToOneComplete<TModel, TId>
    {
    }

    /// <summary>
    /// Abstract base class that has a default implementation for <see cref="ReadChildrenAsync"/>
    /// and <see cref="DeleteChildrenAsync"/>.
    /// </summary>
    public abstract class ManyToOneCompleteBase<TModelCreate, TModel, TId> : CrudBase<TModelCreate, TModel, TId>, IManyToOneComplete<TModelCreate, TModel, TId>
        where TModel : TModelCreate
    {
        /// <inheritdoc />
        public abstract Task<PageEnvelope<TModel>> ReadChildrenWithPagingAsync(TId parentId, int offset,
        int? limit = null,
        CancellationToken token = default(CancellationToken));

        /// <inheritdoc />
        public virtual Task<IEnumerable<TModel>> ReadChildrenAsync(TId parentId, int limit = Int32.MaxValue, CancellationToken token = default(CancellationToken))
        {
            return StorageHelper.ReadPagesAsync((offset, ct) => ReadChildrenWithPagingAsync(parentId, offset, null, ct), limit, token);
        }

        /// <inheritdoc />
        public abstract Task DeleteChildrenAsync(TId parentId, CancellationToken token = default(CancellationToken));
    }
}
