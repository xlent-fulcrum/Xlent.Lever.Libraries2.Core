using Xlent.Lever.Libraries2.Core.Assert;
using Xlent.Lever.Libraries2.Core.Storage.Model;

namespace Xlent.Lever.Libraries2.Core.Test.NuGet.Model
{
    /// <summary>
    /// Methods needed for automatic testing of persistant storage implementations.
    /// </summary>
    /// <seealso cref="CrdTestdTest{TId}"/>
    /// <typeparam name="TStorableItem">The type for the items that can be stored.</typeparam>
    /// <typeparam name="TId">The type for the <see cref="IStorableItem{TId}.Id"/></typeparam>
    public interface IStorableItemForTesting<out TStorableItem, TId>
        where TStorableItem : IStorableItem<TId>, IValidatable
    {
        /// <summary>
        /// Fills all mandatory fields  with valid data.
        /// </summary>
        /// <param name="typeOfTestData">Decides what kind of data to fill with, <see cref="TypeOfTestDataEnum"/>.</param>
        /// <returns>The item itself ("this").</returns>
        TStorableItem InitializeWithDataForTesting(TypeOfTestDataEnum typeOfTestData);

        /// <summary>
        /// Changes the information in a way that would make the item not equal to the state before the changes. 
        /// </summary>
        /// <returns>The item itself ("this").</returns>
        TStorableItem ChangeDataToNotEqualForTesting();
    }
}
