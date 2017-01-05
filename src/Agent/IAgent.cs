using System;
using LoadTestCC.Iface;

namespace LoadTestCC.Agent
{
    /// <summary>
    /// Agent interface
    /// </summary>
    public interface IAgent
    {
        void Init(ICCAgent cc, IClientFactory clientFactory);
        void Start();

        void Stop();
    }
}