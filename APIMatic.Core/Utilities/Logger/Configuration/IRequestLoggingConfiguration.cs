namespace APIMatic.Core.Utilities.Logger.Configuration
{
    public interface IRequestLoggingConfiguration : IHttpLoggingConfiguration
    {
        bool IncludeQueryInPath { get; }
    }
}
