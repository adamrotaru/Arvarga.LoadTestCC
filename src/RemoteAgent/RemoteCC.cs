using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using LoadTestCC.Proto1.Iface;

namespace LoadTestCC.Proto1.RemoteAgent
{
    public class RemoteCC : ICCAgent
    {
        private string _baseUrl;
        private HttpClient _httpClient;

        public RemoteCC(string baseUrl)
        {
            _baseUrl = baseUrl;
            _httpClient = new HttpClient();
        }

        public string Hello(string agentName, string agentIp, string startTime, int maxClientCount)
        {
            string url = $"{_baseUrl}/ltcc/hello/{agentName}/{agentIp}/{System.Text.Encodings.Web.UrlEncoder.Default.Encode(startTime)}/{maxClientCount}";
            HttpResponseMessage resp = _httpClient.GetAsync(url).Result;
            string body = resp.Content.ReadAsStringAsync().Result;
            //Console.WriteLine($"resp '{body}'");
            return body;
        }
        
        public string Sync(string agentId, int clientCount, int targetClientCount)
        {
            string url = $"{_baseUrl}/ltcc/sync/{agentId}/{clientCount}/{targetClientCount}";
            HttpResponseMessage resp = _httpClient.GetAsync(url).Result;
            string body = resp.Content.ReadAsStringAsync().Result;
            //System.Console.WriteLine($"resp '{body}'");
            return body;
        }

        public void AgentStat(string agentId, int clientCount, string statString)
        {
            if (String.IsNullOrEmpty(statString)) statString = "x";
            string url = $"{_baseUrl}/ltcc/stat/{agentId}/{clientCount}/{statString}";
            HttpResponseMessage resp = _httpClient.GetAsync(url).Result;
            string body = resp.Content.ReadAsStringAsync().Result;
            //System.Console.WriteLine($"resp '{body}'");
        }

        public void Bye(string agentId)
        {
            string url = $"{_baseUrl}/ltcc/bye/{agentId}";
            HttpResponseMessage resp = _httpClient.GetAsync(url).Result;
            string body = resp.Content.ReadAsStringAsync().Result;
            //System.Console.WriteLine($"resp '{body}'");
        }
    }
}
