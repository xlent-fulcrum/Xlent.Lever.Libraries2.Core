using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Xlent.Lever.Libraries2.Core.Assert;
using Xlent.Lever.Libraries2.Core.Crud.Interfaces;
using Xlent.Lever.Libraries2.Core.Storage.Logic;
using Xlent.Lever.Libraries2.Core.Storage.Model;

namespace Xlent.Lever.Libraries2.Core.Crud.Helpers
{
    /// <inheritdoc cref="ManyToOneCrudBase{TModelCreate, TModel,TId}" />
    public abstract class ManyToOneCrudBase<TModel, TId> :
        ManyToOneCrudBase<TModel, TModel, TId>,
        IManyToOneCrd<TModel, TId>
    {
    }

    /// <summary>
    /// Abstract base class that has a default implementation for <see cref="CreateAndReturnAsync"/> and
    /// <see cref="CreateWithSpecifiedIdAndReturnAsync"/>.
    /// </summary>
    public abstract class ManyToOneCrudBase<TModelCreate, TModel, TId> :
        ManyToOneRudBase<TModel, TId>,
        IManyToOneCrud<TModelCreate, TModel, TId>
        where TModel : TModelCreate
    {
        /// <inheritdoc />
        public abstract Task<TId> CreateAsync(TModelCreate item,
        CancellationToken token = default(CancellationToken));

        /// <inheritdoc />
        public virtual async Task<TModel> CreateAndReturnAsync(TModelCreate item, CancellationToken token = default(CancellationToken))
        {
            InternalContract.RequireNotNull(item, nameof(item));
            InternalContract.RequireValidated(item, nameof(item));
            var id = await CreateAsync(item, token);
            return await ReadAsync(id, token);
        }

        /// <inheritdoc />
        public abstract Task CreateWithSpecifiedIdAsync(TId id, TModelCreate item,
            CancellationToken token = default(CancellationToken));

        /// <inheritdoc />
        public virtual async Task<TModel> CreateWithSpecifiedIdAndReturnAsync(TId id, TModelCreate item,
            CancellationToken token = default(CancellationToken))
        {
            InternalContract.RequireNotDefaultValue(id, nameof(id));
            InternalContract.RequireNotNull(item, nameof(item));
            InternalContract.RequireValidated(item, nameof(item));
            await CreateWithSpecifiedIdAsync(id, item, token);
            return await ReadAsync(id, token);
        }
    }
}
