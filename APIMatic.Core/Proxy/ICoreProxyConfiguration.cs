namespace APIMatic.Core.Proxy
{
    public interface ICoreProxyConfiguration
    {
        string Address { get; }
        int Port { get; }
        string User { get; }
        string Pass { get; }
        bool Tunnel { get; }
    }
}
