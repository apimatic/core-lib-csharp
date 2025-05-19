namespace APIMatic.Core.Proxy
{
    public class CoreProxyConfiguration
    {
        public string Address { get; }
        public int Port { get; }
        public string User { get; }
        public string Pass { get; }
        public bool Tunnel { get; }

        public CoreProxyConfiguration(string address, int port, string user, string pass, bool tunnel)
        {
            Address = address;
            Port = port;
            User = user;
            Pass = pass;
            Tunnel = tunnel;
        }

        public override string ToString()
        {
            return $"CoreProxyConfiguration: " +
                   $"Address={Address ?? "null"}, " +
                   $"Port={Port}, " +
                   $"User={(string.IsNullOrEmpty(User) ? "null" : User)}, " +
                   $"Pass={(string.IsNullOrEmpty(Pass) ? "null" : "****")}, " +
                   $"Tunnel={Tunnel}";
        }

    }
}
