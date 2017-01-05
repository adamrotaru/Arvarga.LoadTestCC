using System;
using LoadTestCC.Proto1.Iface;

namespace LoadTestCC.Proto1.Agent
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