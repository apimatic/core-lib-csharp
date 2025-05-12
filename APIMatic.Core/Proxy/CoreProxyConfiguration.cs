namespace APIMatic.Core.Proxy
{
    public class CoreProxyConfiguration
    {
        public string Address { get; }
        public int Port { get; }
        public string User { get; }
        public string Pass { get; }
        public bool Tunnel { get; }

        // Constructor for direct initialization
        public CoreProxyConfiguration(string address, int port, string user, string pass, bool tunnel)
        {
            Address = address;
            Port = port;
            User = user;
            Pass = pass;
            Tunnel = tunnel;
        }

        // Copy constructor (optional, for backward compatibility)
        public CoreProxyConfiguration(CoreProxyConfiguration proxy)
        {
            Address = proxy?.Address;
            Port = proxy?.Port ?? 0;
            User = proxy?.User;
            Pass = proxy?.Pass;
            Tunnel = proxy?.Tunnel ?? false;
        }
    }
}
