using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Xlent.Lever.Libraries2.Core.Assert;
using Xlent.Lever.Libraries2.Core.Crud.Interfaces;
using Xlent.Lever.Libraries2.Core.Crud.Model;
using Xlent.Lever.Libraries2.Core.Storage.Logic;
using Xlent.Lever.Libraries2.Core.Storage.Model;

namespace Xlent.Lever.Libraries2.Core.Crud.Helpers
{
    /// <inheritdoc cref="SlaveToMasterCrudBase{TModelCreate,TModel,TId}" />
    public abstract class SlaveToMasterCrudBase<TModel, TId> :
        SlaveToMasterCrudBase<TModel, TModel, TId>,
        ISlaveToMasterCrud<TModel, TId>
    {
    }

    /// <summary>
    /// Abstract base class that has a default implementation for <see cref="CreateAndReturnAsync"/>
    /// and <see cref="CreateWithSpecifiedIdAndReturnAsync"/>.
    /// </summary>
    public abstract class SlaveToMasterCrudBase<TModelCreate, TModel, TId> :
        SlaveToMasterRudBase<TModel, TId>,
        ISlaveToMasterCrud<TModelCreate, TModel, TId>
        where TModel : TModelCreate
    {
        /// <inheritdoc />
        public abstract Task<SlaveToMasterId<TId>> CreateAsync(TId masterId, TModelCreate item,
        CancellationToken token = default(CancellationToken));

        /// <inheritdoc />
        public async Task<TModel> CreateAndReturnAsync(TId masterId, TModelCreate item, CancellationToken token = default(CancellationToken))
        {
            InternalContract.RequireNotDefaultValue(masterId, nameof(masterId));
            InternalContract.RequireNotNull(item, nameof(item));
            InternalContract.RequireValidated(item, nameof(item));
            var id = await CreateAsync(masterId, item, token);
            return await ReadAsync(id, token);
        }

        /// <inheritdoc />
        public abstract Task CreateWithSpecifiedIdAsync(SlaveToMasterId<TId> id, TModelCreate item,
            CancellationToken token = default(CancellationToken));

        /// <inheritdoc />
        public virtual async Task<TModel> CreateWithSpecifiedIdAndReturnAsync(SlaveToMasterId<TId> id, TModelCreate item,
            CancellationToken token = default(CancellationToken))
        {
            InternalContract.RequireNotNull(id, nameof(id));
            InternalContract.RequireValidated(id, nameof(id));
            InternalContract.RequireNotNull(item, nameof(item));
            InternalContract.RequireValidated(item, nameof(item));
            await CreateWithSpecifiedIdAsync(id, item, token);
            return await ReadAsync(id, token);
        }
    }
}
