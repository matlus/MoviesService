using Microsoft.Extensions.Configuration;

namespace MovieService.DomainLayer.Configuration
{
    internal sealed class ConfigurationProvider : ConfigurationProviderBase
    {
        private readonly IConfigurationRoot _configurationRoot;
        public ConfigurationProvider()
        {
            var configurationBuilder = new ConfigurationBuilder();
            configurationBuilder.AddJsonFile("appSettings.json");
            _configurationRoot = configurationBuilder.Build();
        }

        internal ConfigurationProvider(IConfigurationRoot configurationRoot)
        {
            _configurationRoot = configurationRoot;
        }

        protected override string RetrieveConfigurationAppSettings(string appSettingKey)
        {
            return _configurationRoot["AppSettings:" + appSettingKey];
        }
    }
}
