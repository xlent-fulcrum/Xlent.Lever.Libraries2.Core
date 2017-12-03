namespace Xlent.Lever.Libraries2.Core.Storage.Model
{
    /// <summary>
    /// Properties required to be a storable class
    /// </summary>
    /// <typeparam name="TId">The type for the property <see cref="Id"/>.</typeparam>
    public interface IIdentifiable<TId>
    {
        /// <summary>
        /// A unique identifier for the item.
        /// </summary>
        TId Id { get; set; }
    }
}
