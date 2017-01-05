using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using LoadTestCC.Proto1.Iface;
using LoadTestCC.Proto1.Agent;
using LoadTestCC.Proto1.RemoteAgent;

namespace LoadTestCC.Proto1.TestExe1Proc
{
    public class Program
    {
        public static void Main(string[] args)
        {
            string baseUrl = "http://localhost:5000";
            RemoteAgent.RemoteAgent agent = new RemoteAgent.RemoteAgent();
            agent.Init(baseUrl, new ClientFactory());
            agent.Start();
            Console.WriteLine($"Program:  agent started");

            Console.WriteLine($"Press Enter to exit");
            Console.ReadLine();

            agent.Stop();
        }
    }
}
