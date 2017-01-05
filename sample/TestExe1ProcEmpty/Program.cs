using LoadTestCC.Proto1.Iface;
using LoadTestCC.Proto1.Agent;
using System;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace LoadTestCC.Proto1.TestExe1Proc
{
    public class Program
    {
        public static void Main(string[] args)
        {
            // Create the CC
            ICCControl cc = new CommCont.CommCont();
            cc.Start();
            Console.WriteLine("Program:  CC created");
            cc.SetTarget(1);

            // Create some clients
            List<IAgent> agents = new List<IAgent>();
            for (int i = 0; i < 3; ++i)
            {
                IAgent a1 = new Agent.Agent();
                a1.Init(cc, new ClientFactory());
                a1.Start();
                agents.Add(a1);
                Console.WriteLine($"Program:  agent {agents.Count} started");
            }

            cc.SetTarget(10);
            Console.WriteLine($"CC Status {cc.GetStatus()}");

            for (int i = 0; i < 50; ++i)
            {
                System.Threading.Thread.Sleep(1000);
                Console.WriteLine($"CC Status {cc.GetStatus()}");
            }

            Console.WriteLine($"CC Stopping agents...");
            foreach(IAgent a in agents)
            {
                a.Stop();
            }
            Console.WriteLine($"CC Agents stopped");
            Console.WriteLine($"CC Status {cc.GetStatus()}");

            cc.Stop();
            Console.WriteLine($"CC Stopped");
        }
    }
}
