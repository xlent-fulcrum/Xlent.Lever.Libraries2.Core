using System.Configuration;

namespace Xlent.Lever.Libraries2.Core.Application
{
    /// <summary>
    /// Implements <see cref="IAppSettingGetter"/> by using <see cref="ConfigurationManager"/>.
    /// </summary>
    public class ConfigurationManagerAppSettings : IAppSettingGetter
    {
        /// <inheritdoc />
        public string GetAppSetting(string name)
        {
            return ConfigurationManager.AppSettings[name];
        }
    }
}