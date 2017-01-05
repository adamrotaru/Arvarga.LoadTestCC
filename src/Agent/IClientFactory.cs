using System;
using LoadTestCC.Proto1.Iface;

namespace LoadTestCC.Proto1.Agent
{
    ///<summary>
    ///A load test client interface
    ///</summary>
    public interface IClientFactory
    {
        IClient CreateNew(string clientId);
    }
}
