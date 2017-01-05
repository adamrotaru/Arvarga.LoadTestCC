using LoadTestCC.Proto1.Iface;
using LoadTestCC.Proto1.Agent;
using System;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace LoadTestCC.Proto1.TestExe1Proc
{
    public class ClientFactory : IClientFactory
    {
        public IClient CreateNew(string clientId)
        {
            return new Client{ Id = clientId };
        }
    }

    public class Client : IClient
    {
        public string Id { get; set; }

        public async Task Run()
        {
            //Console.WriteLine($"Client {Id}: started");
            await Task.Delay(10000);
            //Console.WriteLine($"Client {Id}: finished");
        }
        
        public void StopAsync()
        {}

        public ClientsStat GetStat()
        {
            ClientsStat cs = new ClientsStat(new List<StatParamDesc> {
                new StatParamDesc("CliCountCurr", StatParamAggregationStrategy.Sum, 1, ""),
            });
            return cs;
        }
    }
}
