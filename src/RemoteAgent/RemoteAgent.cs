using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using LoadTestCC.Proto1.Iface;
using LoadTestCC.Proto1.Agent;

namespace LoadTestCC.Proto1.RemoteAgent
{
    /// <summary>
    /// </summary>
    public class RemoteAgent
    {
        private Agent.Agent _wrappedAgent;
        private RemoteCC _remoteCC;

        public void Init(string baseUrl, IClientFactory clientFactory)
        {
            _remoteCC = new RemoteCC(baseUrl);
            _wrappedAgent = new Agent.Agent();
            _wrappedAgent.Init(_remoteCC, clientFactory);
        }

        public void Start()
        {
            _wrappedAgent.Start();
        }

        public void Stop()
        {
            _wrappedAgent.Stop();
        }
    }
}
