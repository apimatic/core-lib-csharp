using APIMatic.Core.Types.Sdk;

namespace APIMatic.Core.Utilities.Logger
{
    public class NullSdkLogger : ISdkLogger
    {
        public void LogRequest(CoreRequest request)
        {
            // Method intentionally left empty.
        }

        public void LogResponse(CoreResponse response)
        {
            // Method intentionally left empty.
        }
    }
}
