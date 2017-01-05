using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using LoadTestCC.Proto1.Iface;
using LoadTestCC.Proto1.Agent;
using LoadTestCC.Proto1.RemoteAgent;

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
        private bool _stopped;

        public async Task Run()
        {
            //Console.WriteLine($"Client {Id}: started");
            for (int i = 0; i < 30; ++i)
            {
                if (_stopped)
                    break;
                await Task.Delay(1000);
            }
            //Console.WriteLine($"Client {Id}: finished");
        }
        
        public void StopAsync()
        {
            _stopped = true;
        }

        public ClientsStat GetStat()
        {
            ClientsStat cs = new ClientsStat(new List<StatParamDesc> {
                new StatParamDesc("CliCountCurr", StatParamAggregationStrategy.Sum, 1, ""),
            });
            return cs;
        }
    }
}
