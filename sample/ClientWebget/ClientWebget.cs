using LoadTestCC.Proto1.Iface;
using LoadTestCC.Proto1.Agent;
using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Net.Http;
using System.Diagnostics;

namespace LoadTestCC.Proto1.Sample.ClientWebget
{
    public class ClientWebgetFactory : IClientFactory
    {
        public IClient CreateNew(string clientId)
        {
            return new ClientWebget{ Id = clientId };
        }
    }

    /// TODO response time statistics
    public class ClientWebget : IClient
    {
        public static readonly string Url = "http://google.com";
        public static readonly int Iterations = 5;
        public static readonly int PauseTime = 3000;
        private HttpClient _httpClient = new HttpClient();
        private int _callCount;
        private double _durTotal;
        private double _durMax = 0;
        private double _durMin = 1000000;

        public string Id { get; set; }

        public async Task Run()
        {
            // TODO stat
            //Console.WriteLine($"Client {Id}: started");
            for (int i = 0; i < Iterations; ++i)
            {
                await GetWebPage(Url);
                await Task.Delay(PauseTime);
            }
            //Console.WriteLine($"Client {Id}: finished");
        }
        
        public void StopAsync()
        {
            // TODO
        }

        public ClientsStat GetStat()
        {
            ClientsStat cs = new ClientsStat(new List<StatParamDesc> {
                new StatParamDesc("CliCount", StatParamAggregationStrategy.Sum, 1, ""),
                new StatParamDesc("CallCountTotal", StatParamAggregationStrategy.Sum, _callCount, ""),
                new StatParamDesc("DurAvg", StatParamAggregationStrategy.Average, _callCount == 0 ? 0 : _durTotal / (double)_callCount, "ms"),
                new StatParamDesc("DurMax", StatParamAggregationStrategy.Average, _durMax, "ms"),
                new StatParamDesc("DurMin", StatParamAggregationStrategy.Average, _durMin, "ms"),
            });
            return cs;
        }

        private async Task GetWebPage(string url)
        {
            Stopwatch sw = new Stopwatch();
            sw.Start();
            HttpResponseMessage resp = await _httpClient.GetAsync(url);
            sw.Stop();
            double dur = sw.ElapsedMilliseconds;
            ++_callCount;
            _durTotal += dur;
            if (dur > _durMax) _durMax = dur;
            if (dur < _durMin) _durMin = dur;
            System.Console.WriteLine($"Got web page in {dur} ms, {url} {resp.StatusCode}");
        }
    }
}
