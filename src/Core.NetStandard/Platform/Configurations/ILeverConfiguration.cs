namespace Xlent.Lever.Libraries2.Core.Platform.Configurations
{
    /// <summary>
    /// Interface for retrieving configuration data.
    /// </summary>
    public interface ILeverConfiguration
    {
        /// <summary>
        /// Gets a value from the configuration and verifies that it is not null.
        /// </summary>
        T MandatoryValue<T>(object key);

        /// <summary>
        /// Gets a value from the configuration.
        /// </summary>
        T Value<T>(object key);
    }
}