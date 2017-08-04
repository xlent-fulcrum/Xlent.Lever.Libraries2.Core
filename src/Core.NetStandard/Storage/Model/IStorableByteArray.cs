namespace Xlent.Lever.Libraries2.Core.Storage.Model
{
    /// <summary>
    /// Contains a byte array
    /// </summary>
    public interface IStorableByteArray<TId> : IStorableItem<TId>
    {
        /// <summary>
        /// The content as a byte array
        /// </summary>
        byte[] ByteArray { get; set; }
    }
}