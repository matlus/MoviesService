using MovieService.DomainLayer.Exceptions;

namespace MovieService.DomainLayer.Configuration
{
    internal abstract class ConfigurationProviderBase
    {
        public string GetImdbBaseUrl()
        {
            var imdbBaseUrl =  RetrieveConfigurationAppSettingThrowIfMissing("ImdbBaseUrl");
            return (imdbBaseUrl.EndsWith("/")) ? imdbBaseUrl : imdbBaseUrl + "/";
        }

        private string RetrieveConfigurationAppSettingThrowIfMissing(string appSettingKey)
        {
            var settingValue = RetrieveConfigurationAppSettings(appSettingKey);

            if (settingValue == null)
            {
                throw new ConfigurationSettingKeyNotFoundException($"The configuration file's, AppSettings section does not contain the key: {appSettingKey}, or the AppSettings section itself is missing.");
            }
            else if (settingValue.Length == 0)
            {
                throw new ConfigurationSettingMissingException($"The configuration file's, AppSettings section does not contain a value for the key: {appSettingKey}");
            }

            return settingValue;
        }

        protected abstract string RetrieveConfigurationAppSettings(string appSettingKey);
    }
}
