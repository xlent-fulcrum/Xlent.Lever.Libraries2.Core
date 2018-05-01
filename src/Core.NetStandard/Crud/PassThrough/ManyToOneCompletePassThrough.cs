using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Xlent.Lever.Libraries2.Core.Crud.Interfaces;
using Xlent.Lever.Libraries2.Core.Storage.Model;

namespace Xlent.Lever.Libraries2.Core.Crud.PassThrough
{
    /// <inheritdoc cref="IManyToOneRelationComplete{TManyModelCreate, TManyModel,TId}" />
    public class ManyToOneCompletePassThrough<TModel, TId> : 
        ManyToOneCompletePassThrough<TModel, TModel, TId>,
        IManyToOneRelationComplete<TModel, TId>
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="nextLevel">The crud class to pass things down to.</param>
        public ManyToOneCompletePassThrough(IManyToOneRelationComplete<TModel, TId> nextLevel)
            : base(nextLevel)
        {
        }
    }

    /// <inheritdoc cref="IManyToOneRelationComplete{TManyModelCreate, TManyModel,TId}" />
    public class ManyToOneCompletePassThrough<TModelCreate, TModel, TId> : CrudPassThrough<TModelCreate, TModel, TId>, IManyToOneRelationComplete<TModelCreate, TModel, TId> where TModel : TModelCreate
    {
        private readonly IManyToOneRelationComplete<TModelCreate, TModel, TId> _nextLevel;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="nextLevel">The crud class to pass things down to.</param>
        public ManyToOneCompletePassThrough(IManyToOneRelationComplete<TModelCreate, TModel, TId> nextLevel)
            : base(nextLevel)
        {
            _nextLevel = nextLevel;
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
        public virtual Task<PageEnvelope<TModel>> ReadChildrenWithPagingAsync(TId parentId, int offset, int? limit = null,
            CancellationToken token = default(CancellationToken))
        {
            return _nextLevel.ReadChildrenWithPagingAsync(parentId, offset, limit, token);
        }

        /// <inheritdoc />
        public virtual Task<IEnumerable<TModel>> ReadChildrenAsync(TId parentId, int limit = Int32.MaxValue, CancellationToken token = default(CancellationToken))
        {
            return _nextLevel.ReadChildrenAsync(parentId, limit, token);
        }

        /// <inheritdoc />
        public virtual Task DeleteChildrenAsync(TId parentId, CancellationToken token = default(CancellationToken))
        {
            return _nextLevel.DeleteChildrenAsync(parentId, token);
        }
    }
}
