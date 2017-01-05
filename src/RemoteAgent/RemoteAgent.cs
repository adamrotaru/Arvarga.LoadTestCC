using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using LoadTestCC.Iface;
using LoadTestCC.Agent;

namespace LoadTestCC.RemoteAgent
{
    /// <summary>
    /// </summary>
    public class RemoteAgent
    {
        public static readonly string DefaultUrl = "http://localhost:5000";
        private Agent.Agent _wrappedAgent;
        private RemoteCC _remoteCC;

        /// <summary><summary>
        /// <paramref name="ccUrl">Base URL of the CC webapp</paramref>
        public void Init(string ccUrl, IClientFactory clientFactory)
        {
            _remoteCC = new RemoteCC(ccUrl);
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
