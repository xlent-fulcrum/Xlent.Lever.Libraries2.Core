namespace Xlent.Lever.Libraries2.Core.Context
{
    /// <summary>
    /// Interface for accessing data.
    /// </summary>
    public interface IValueProvider
    {
        /// <summary>
        /// Get the data with name <paramref key="key"/>.
        /// </summary>
        /// <param name="key"></param>
        /// <typeparam name="T">The type of the data.</typeparam>
        /// <returns>The found value or null if not found.</returns>
        T GetValue<T>(string key);

        /// <summary>
        /// Create/update the data with name <paramref name="key"/> to <paramref name="value"/>, which must not be null.
        /// </summary>
        void SetValue<T>(string key, T value);
    }
}