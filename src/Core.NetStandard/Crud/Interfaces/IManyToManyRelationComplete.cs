using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Xlent.Lever.Libraries2.Core.Storage.Model
{
    /// <summary>
    /// Functionality for persisting many-to-many relations.
    /// </summary>
    public interface IManyToManyRelationComplete<TManyToManyModel, TReferenceModel1, TReferenceModel2, TId>
        : ICrud<TManyToManyModel, TId>,
            IManyToManyRelation<TReferenceModel1, TReferenceModel2, TId>,
            IManyToManyBiased1Complete<TManyToManyModel, TReferenceModel2, TId>,
            IManyToManyBiased2Complete<TManyToManyModel, TReferenceModel1, TId>
    {
    }
}
