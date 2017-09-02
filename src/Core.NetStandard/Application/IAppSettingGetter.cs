namespace Xlent.Lever.Libraries2.Core.Application
{
    /// <summary>
    /// The implementor of this interface can get settings for the application.
    /// </summary>
    public interface IAppSettingGetter
    {
        /// <summary>
        /// Get one app setting. Returns null if the setting was not found.
        /// </summary>
        /// <param name="name">The name of the app setting.</param>
        /// <returns>The value or null.</returns>
        string GetAppSetting(string name);
    }
}