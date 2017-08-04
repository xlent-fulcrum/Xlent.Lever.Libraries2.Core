namespace Xlent.Lever.Libraries2.Core.Storage.Model
{
    /// <summary>
    /// Data that can be stored in a <see cref="IStorableByteArray{TId}"/>
    /// </summary>
    /// <typeparam name="TData">The type of the data.</typeparam>
    /// <typeparam name="TId">The type of the unique identifier for the <see cref="IStorableByteArray{TId}"/></typeparam>
    public interface IStorableAsByteArray<TData, TId> : IStorableByteArray<TId>
    {
        /// <summary>
        /// The Data that is stored as a byte array. This property is just a converter between the <see cref="IStorableByteArray{TId}.ByteArray"/> and this data type.
        /// </summary>
        TData Data { get; set; }
    }
}