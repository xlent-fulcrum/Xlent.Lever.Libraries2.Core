namespace Xlent.Lever.Libraries2.Core.Misc.Models
{
    /// <summary>
    /// Copy an object with deep copying
    /// </summary>
    public interface IDeepCopy<out T>
    {
        /// <summary>
        /// Return a new copy.
        /// </summary>
        /// <returns></returns>
        T DeepCopy();
    }
}
