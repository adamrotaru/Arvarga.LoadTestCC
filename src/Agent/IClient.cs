using System;
using System.Threading.Tasks;
using LoadTestCC.Proto1.Iface;

namespace LoadTestCC.Proto1.Agent
{
    ///<summary>
    ///A load test client interface
    ///</summary>
    public interface IClient
    {
        string Id { get; }
        Task Run();
        void StopAsync();
        ClientsStat GetStat();
    }
}
