namespace Xlent.Lever.Libraries2.Core.Misc.Models
{
    /// <summary>
    /// Copy an object with deep copying
    /// </summary>
    public interface IDeepCopy<T>
    {
        /// <summary>
        /// Copy the values of <paramref name="source"/> into the current object.
        /// </summary>
        /// <returns></returns>
        void DeepCopy(T source);

        /// <summary>
        /// Create a new a new object that is a copy of the current object.
        /// </summary>
        T DeepCopy();
    }
}
