namespace Xlent.Lever.Libraries2.Core.Storage.Model
{
    /// <summary>
    /// A class that inherits this interface has a <see cref="Name"/> property that can be presented to the end user to represent an instance of the class.
    /// </summary>
    public interface INameProperty
    {
        /// <summary>
        /// A friendly name for the item, i.e. can be shown to the end user to represent the item.
        /// </summary>
        /// <remarks>
        /// Does not have to be unique among items.
        /// </remarks>
        string Name { get; }
    }
}
