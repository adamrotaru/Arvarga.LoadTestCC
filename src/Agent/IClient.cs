using System;
using System.Threading.Tasks;
using LoadTestCC.Iface;

namespace LoadTestCC.Agent
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
