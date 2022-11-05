namespace PU_Test.Model
{
    internal class ProxyConfig
    {
        public bool ProxyEnable = false;

        public ProxyConfig(bool proxyEnable, string proxyServer,bool usehttp=false)
        {
            ProxyEnable = proxyEnable;
            ProxyServer = proxyServer;
            UseHttp = usehttp;

        }

        public string ProxyServer { get; set; }

        public string ProxyPort { get; set; }

        public bool UseHttp { get; set; }


    }
}
