namespace Xlent.Lever.Libraries2.Core.Decoupling
{
    /// <summary>
    /// Can create items of the specified type.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IFactory<out T>
    {
        /// <summary>
        /// Creates an item instance.
        /// </summary>
        T CreateItemFromFactory();
    }
}
