﻿using System;
using System.Diagnostics.CodeAnalysis;

namespace MovieService.DomainLayer.Exceptions
{

    [Serializable]
    [ExcludeFromCodeCoverage]
    public sealed class ConfigurationSettingMissingException : MovieServiceTechnicalBaseException
    {
        public ConfigurationSettingMissingException() { }
        public ConfigurationSettingMissingException(string message) : base(message) { }
        public ConfigurationSettingMissingException(string message, Exception inner) : base(message, inner) { }
        private ConfigurationSettingMissingException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
    }}