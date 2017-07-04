using System;

namespace Xlent.Lever.Libraries2.Core.Storage.Model
{
    /// <summary>
    /// Properties for a data "row" that has timestamps for creation and updates.
    /// </summary>
    public interface ITimeStamped
    {
        /// <summary>
        /// The time when a "row" was created.
        /// </summary>
        DateTimeOffset CreatedAt { get; set; }

        /// <summary>
        /// The time when a "row" was last updated.
        /// </summary>
        DateTimeOffset UpdatedAt { get; set; }
    }
}
