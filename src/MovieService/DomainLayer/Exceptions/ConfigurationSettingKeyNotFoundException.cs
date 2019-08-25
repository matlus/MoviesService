using System;
using System.Diagnostics.CodeAnalysis;

namespace MovieService.DomainLayer.Exceptions
{

    [Serializable]
    [ExcludeFromCodeCoverage]
    public sealed class ConfigurationSettingKeyNotFoundException : MovieServiceTechnicalBaseException
    {
        public override string Reason => "Configuration Setting Key Not Found";
        public ConfigurationSettingKeyNotFoundException() { }
        public ConfigurationSettingKeyNotFoundException(string message) : base(message) { }
        public ConfigurationSettingKeyNotFoundException(string message, Exception inner) : base(message, inner) { }
        private ConfigurationSettingKeyNotFoundException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
    }
}
