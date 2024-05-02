using Microsoft.Extensions.Logging;

namespace APIMatic.Core.Utilities.Logger.Configuration
{
    public interface ISdkLoggingConfiguration
    {
        ILogger Logger { get; }
        
        LogLevel? LogLevel { get; }
        
        bool IsConfigured { get; }
        
        bool MaskSensitiveHeaders { get; }
        
        IRequestLoggingConfiguration RequestLoggingConfiguration { get; }
        
        IResponseLoggingConfiguration ResponseLoggingConfiguration { get; }
    }
}
