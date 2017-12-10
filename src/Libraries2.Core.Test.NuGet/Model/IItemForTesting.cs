﻿namespace Xlent.Lever.Libraries2.Core.Test.NuGet.Model
{
    /// <summary>
    /// Methods needed for automatic testing of persistant storage implementations.
    /// </summary>
    /// <typeparam name="T">The type for the items that can be stored.</typeparam>
    public interface IItemForTesting
    {
        /// <summary>
        /// Fills all mandatory fields  with valid data.
        /// </summary>
        /// <param name="typeOfTestData">Decides what kind of data to fill with, <see cref="TypeOfTestDataEnum"/>.</param>
        /// <returns>The item itself ("this").</returns>
        void InitializeWithDataForTesting(TypeOfTestDataEnum typeOfTestData);

        /// <summary>
        /// Changes the information in a way that would make the item not equal to the state before the changes. 
        /// </summary>
        /// <returns>The item itself ("this").</returns>
        void ChangeDataToNotEqualForTesting();
    }
}
