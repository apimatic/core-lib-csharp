using System.Collections.Generic;

namespace APIMatic.Core.Utilities.Logger.Configuration
{
    public interface IHttpLoggingConfiguration
    {
        bool Body { get; }

        bool Headers { get; }

        IReadOnlyCollection<string> HeadersToInclude { get; }

        IReadOnlyCollection<string> HeadersToExclude { get; }

        IReadOnlyCollection<string> HeadersToUnmask { get; }
    }
}
