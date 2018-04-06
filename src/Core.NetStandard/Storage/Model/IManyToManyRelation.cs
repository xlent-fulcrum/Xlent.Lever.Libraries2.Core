using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Xlent.Lever.Libraries2.Core.Storage.Model
{
    /// <summary>
    /// Functionality for persisting many-to-many relations.
    /// </summary>
    public interface IManyToManyRelation<TReferenceModel1, TReferenceModel2, in TId> : IManyToManyBiased1<TReferenceModel2, TId>, IManyToManyBiased2<TReferenceModel1, TId>
    {
    }
}
