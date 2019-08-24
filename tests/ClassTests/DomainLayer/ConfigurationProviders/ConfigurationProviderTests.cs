using Microsoft.Extensions.Configuration;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MovieService.DomainLayer.Exceptions;
using System.Collections.Generic;
using TestingCommon.TestingHelpers;
using ConfigurationProvider = MovieService.DomainLayer.Configuration.ConfigurationProvider;

namespace ClassTests.DomainLayer.ConfigurationProviders
{
    [TestClass]
    public class UnitTest1
    {
        private const string ImdbBaseUrlKey = "ImdbBaseUrl";

        private static IConfigurationRoot GetConfigurationRoot(string configurationSectionName, string key, string value)
        {
            var configurationBuilder = new ConfigurationBuilder();
            var initialData = new Dictionary<string, string>
            {
                { configurationSectionName + ":" + key, value }
            };

            configurationBuilder.AddInMemoryCollection(initialData);
            return configurationBuilder.Build();
        }

        private static IConfigurationRoot AddAppSettingInConfigFile(string key, string value)
        {
            return GetConfigurationRoot("AppSettings", key, value);
        }

        [TestMethod]
        [TestCategory("Class Integration Test")]
        public void ConfigurationProvider_GetImdbBaseUrl_WhenAppSettingsSectionIsMissing_ShouldThrow()
        {
            var configurationRoot = GetConfigurationRoot("SomeIrrelevantConfigSectionName", ImdbBaseUrlKey, "irrelevantValue");
            var configurationProviderUnderTest = new ConfigurationProvider(configurationRoot);

            try
            {
                // Act
                configurationProviderUnderTest.GetImdbBaseUrl();
                Assert.Fail("We were expecting an exception of type: ConfigurationSettingKeyNotFoundException to be thrown, but no exception was thrown");
            }
            catch (ConfigurationSettingKeyNotFoundException e)
            {
                // Assert
                AssertEx.AssertExceptionMessageContains(new[] { "AppSettings section itself is missing", "does not contain the key", ImdbBaseUrlKey }, e);
            }
        }

        [TestMethod]
        [TestCategory("Class Integration Test")]
        public void ConfigurationProvider_GetImdbBaseUrl_WhenImdbBaseUrlSettingsExistsAndEndsWithForwardSlah_ShouldReturnValueEndingInForwardSlash()
        {
            // Arrange
            var expectedImdbBaseUrlValue = "http://goingtonowhere.com/";
            var configurationRoot = AddAppSettingInConfigFile(ImdbBaseUrlKey, expectedImdbBaseUrlValue);
            var configurationProviderUnderTest = new ConfigurationProvider(configurationRoot);

            // Act
            var actualImdnBaseUrl = configurationProviderUnderTest.GetImdbBaseUrl();

            // Assert
            Assert.AreEqual(expectedImdbBaseUrlValue, actualImdnBaseUrl, $"We we expecting the actualImdnBaseUrl to be '{expectedImdbBaseUrlValue}', but found that is it '{actualImdnBaseUrl}' instead");
        }

        [TestMethod]
        [TestCategory("Class Integration Test")]
        public void ConfigurationProvider_GetImdbBaseUrl_WhenImdbBaseUrlSettingsExistsButWithoutEndingForwardSlah_ShouldReturnValueEndingInForwardSlash()
        {
            // Arrange
            var imdbBaseUrlValue = "http://goingtonowhere.com";
            var expectedImdbBaseUrlValue = imdbBaseUrlValue + "/";
            var configurationRoot = AddAppSettingInConfigFile(ImdbBaseUrlKey, imdbBaseUrlValue);
            var configurationProviderUnderTest = new ConfigurationProvider(configurationRoot);

            // Act
            var actualImdnBaseUrl = configurationProviderUnderTest.GetImdbBaseUrl();

            // Assert
            Assert.AreEqual(expectedImdbBaseUrlValue, actualImdnBaseUrl, $"We we expecting the actualImdnBaseUrl to be '{expectedImdbBaseUrlValue}', but found that is it '{actualImdnBaseUrl}' instead");
        }

        [TestMethod]
        [TestCategory("Class Integration Test")]
        public void ConfigurationProvider_GetImdbBaseUrl_WhenImdbBaseUrlKeyExistsButValueIsEmpty_ShouldThrow()
        {
            // Arrange
            var emptyImdbBaseUrlValue = string.Empty;
            var configurationRoot = AddAppSettingInConfigFile(ImdbBaseUrlKey, emptyImdbBaseUrlValue);
            var configurationProviderUnderTest = new ConfigurationProvider(configurationRoot);

            try
            {
                // Act
                configurationProviderUnderTest.GetImdbBaseUrl();
                Assert.Fail("We were expecting an exception of type: ConfigurationSettingMissingException to be thrown, but no exception was thrown");
            }
            catch (ConfigurationSettingMissingException e)
            {
                // Assert
                AssertEx.AssertExceptionMessageContains(new[] { "does not contain a value", "key", ImdbBaseUrlKey }, e);
            }
        }

        [TestMethod]
        [TestCategory("Class Integration Test")]
        public void ConfigurationProvider_GetImdbBaseUrl_WhenImdbBaseUrlKeyAndValueAreNonExistant_ShouldThrow()
        {
            // Arrange
            var configurationRoot = AddAppSettingInConfigFile(null, null);
            var configurationProviderUnderTest = new ConfigurationProvider(configurationRoot);

            try
            {
                // Act
                configurationProviderUnderTest.GetImdbBaseUrl();
                Assert.Fail("We were expecting an exception of type: ConfigurationSettingKeyNotFoundException to be thrown, but no exception was thrown");
            }
            catch (ConfigurationSettingKeyNotFoundException e)
            {
                // Assert
                AssertEx.AssertExceptionMessageContains(new[] { "does not contain the key", ImdbBaseUrlKey }, e);
            }
        }
    }
}
