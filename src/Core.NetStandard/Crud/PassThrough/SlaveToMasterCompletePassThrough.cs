using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Xlent.Lever.Libraries2.Core.Crud.Interfaces;
using Xlent.Lever.Libraries2.Core.Crud.Model;
using Xlent.Lever.Libraries2.Core.Storage.Model;

namespace Xlent.Lever.Libraries2.Core.Crud.PassThrough
{
    /// <inheritdoc cref="ManyToOneCompletePassThrough{TManyModelCreate,TManyModel,TId}" />
    public class SlaveToMasterCompletePassThrough<TModel, TId> :
        SlaveToMasterCompletePassThrough<TModel, TModel, TId>,
        ISlaveToMaster<TModel, TId>
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="nextLevel">The crud class to pass things down to.</param>
        public SlaveToMasterCompletePassThrough(ISlaveToMasterComplete<TModel, TId> nextLevel)
            : base(nextLevel)
        {
        }
    }

    /// <inheritdoc cref="IManyToOneComplete{TManyModelCreate,TManyModel,TId}" />
    public class SlaveToMasterCompletePassThrough<TModelCreate, TModel, TId> : RudPassThrough<TModel, SlaveToMasterId<TId>>, ISlaveToMasterComplete<TModelCreate, TModel, TId> where TModel : TModelCreate
    {
        private readonly ISlaveToMasterComplete<TModelCreate, TModel, TId> _nextLevel;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="nextLevel">The crud class to pass things down to.</param>
        public SlaveToMasterCompletePassThrough(ISlaveToMasterComplete<TModelCreate, TModel, TId> nextLevel)
        :base(nextLevel)
        {
            _nextLevel = nextLevel;
        }

        /// <inheritdoc />
        public virtual Task<PageEnvelope<TModel>> ReadChildrenWithPagingAsync(TId parentId, int offset, int? limit = null,
            CancellationToken token = default(CancellationToken))
        {
            return _nextLevel.ReadChildrenWithPagingAsync(parentId, offset, limit, token);
        }

        /// <inheritdoc />
        public virtual Task<IEnumerable<TModel>> ReadChildrenAsync(TId parentId, int limit = int.MaxValue, CancellationToken token = default(CancellationToken))
        {
            return _nextLevel.ReadChildrenAsync(parentId, limit, token);
        }

        /// <inheritdoc />
        public virtual Task<SlaveToMasterId<TId>> CreateAsync(TId masterId, TModelCreate item, CancellationToken token = default(CancellationToken))
        {
            return _nextLevel.CreateAsync(masterId, item, token);
        }

        /// <inheritdoc />
        public virtual Task<TModel> CreateAndReturnAsync(TId masterId, TModelCreate item, CancellationToken token = default(CancellationToken))
        {
            return _nextLevel.CreateAndReturnAsync(masterId, item, token);
        }

        /// <inheritdoc />
        public virtual Task CreateWithSpecifiedIdAsync(SlaveToMasterId<TId> id, TModelCreate item,
            CancellationToken token = default(CancellationToken))
        {
            return _nextLevel.CreateWithSpecifiedIdAsync(id, item, token);
        }

        /// <inheritdoc />
        public virtual Task<TModel> CreateWithSpecifiedIdAndReturnAsync(SlaveToMasterId<TId> id, TModelCreate item,
            CancellationToken token = default(CancellationToken))
        {
            return _nextLevel.CreateWithSpecifiedIdAndReturnAsync(id, item, token);
        }

        /// <inheritdoc />
        public virtual Task DeleteChildrenAsync(TId parentId, CancellationToken token = default(CancellationToken))
        {
            return _nextLevel.DeleteChildrenAsync(parentId, token);
        }
    }
}
