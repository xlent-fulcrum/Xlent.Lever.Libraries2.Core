using System.Threading;
using System.Threading.Tasks;
using Xlent.Lever.Libraries2.Core.Crud.Interfaces;

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

    /// <inheritdoc cref="ICrud{TModel,TId}" />
    public class CrudPassThrough<TModelCreate, TModel, TId> : RudPassThrough<TModel, TId>, ICrud<TModelCreate, TModel, TId> where TModel : TModelCreate
    {
        private readonly ICrud<TModelCreate, TModel, TId> _nextLevel;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="nextLevel">The crud class to pass things down to.</param>
        public CrudPassThrough(ICrud<TModelCreate, TModel, TId> nextLevel)
        :base(nextLevel)
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
    }
}
