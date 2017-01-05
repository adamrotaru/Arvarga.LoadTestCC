using System;

namespace LoadTestCC.Sample.TestAgentExeWebget
{
    public class Program
    {
        public static void Main(string[] args)
        {
            string ccUrl;
            if (args.Length < 1)
            {
                ccUrl = RemoteAgent.RemoteAgent.DefaultUrl;
                Console.WriteLine($"Missing argument, using default: {ccUrl}");
            }
            else
            {
                ccUrl = args[0];
            }
            Console.WriteLine($"CC Url: {ccUrl}");

            RemoteAgent.RemoteAgent agent = new RemoteAgent.RemoteAgent();
            agent.Init(ccUrl, new ClientWebget.ClientWebgetFactory());
            agent.Start();
            Console.WriteLine($"Program:  agent started");

            Console.WriteLine($"Press Enter to exit");
            Console.ReadLine();

            agent.Stop();
        }
    }
}
