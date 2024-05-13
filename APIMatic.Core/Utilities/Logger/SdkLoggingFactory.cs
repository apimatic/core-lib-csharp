using APIMatic.Core.Utilities.Logger.Configuration;

namespace APIMatic.Core.Utilities.Logger
{
    internal static class SdkLoggerFactory
    {
        public static ISdkLogger Create(SdkLoggingConfiguration sdkLoggingConfiguration) =>
            sdkLoggingConfiguration == null
                ? (ISdkLogger)new NullSdkLogger()
                : new SdkLogger(sdkLoggingConfiguration);
    }
}
