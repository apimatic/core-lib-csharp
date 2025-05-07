
namespace APIMatic.Core.Proxy
{
    public class CoreProxyConfiguration : ICoreProxyConfiguration
    {
        public string Address { get; }
        public int Port { get; }
        public string User { get; }
        public string Pass { get; }
        public bool Tunnel { get; }

        public CoreProxyConfiguration(ICoreProxyConfiguration proxy)
        {
            Address = proxy?.Address;
            Port = proxy?.Port ?? 0;
            User = proxy?.User;
            Pass = proxy?.Pass;
            Tunnel = proxy?.Tunnel ?? false;
        }
    }
}
