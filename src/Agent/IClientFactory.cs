using System;
using LoadTestCC.Iface;

namespace LoadTestCC.Agent
{
    ///<summary>
    ///A load test client interface
    ///</summary>
    public interface IClientFactory
    {
        IClient CreateNew(string clientId);
    }
}
