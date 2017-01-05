using System;
using System.Collections.Generic;
using System.Linq;using System.Threading.Tasks;
using LoadTestCC.Proto1.Iface;

namespace LoadTestCC.Proto1.Agent
{
    public class ClientInfo
    {
        public IClient _cli;
        // 1 created, 2 running, 3 cancelling
        public int _state;
    }

    /// <summary>
    /// Manages a number of clients, in one process locally.
    /// </summary>
    public class Agent: IAgent
    {
        public const int MaxClientCount = 1000;
        private const int ReportingPeriod = 3000;
        private const int ControlLoopPeriod = 200;
        private ICCAgent _cc;
        private string _id;
        private string _name;
        private string _ip;
        private DateTime _startTime;
        private IClientFactory _clientFactory;
        private int _targetClientCount;
        private Dictionary<string, ClientInfo> _clients = new Dictionary<string, ClientInfo>();
        private object _clientLock = new object();
        private static int _agentCounterInProc;
        private static int _clientCounter;
        private bool _stopped;
        private Task _controlLoopTask;

        public void Init(ICCAgent cc, IClientFactory clientFactory)
        {
            DateTime now = DateTime.Now;
            _cc = cc;
            // Create name, include hostname, process Id
            // Note: ExpandEnvironmentVariables("%COMPUTERNAME%") is Windows-specific!  TODO
            _name = $"A_{System.Environment.ExpandEnvironmentVariables("%COMPUTERNAME%")}_{System.Diagnostics.Process.GetCurrentProcess().Id}_{(++_agentCounterInProc).ToString()}";
            _ip = "ipTODO";
            _startTime = now;
            _clientFactory = clientFactory;
            SayHello();
        }

        private void SayHello()
        {
            _id = _cc.Hello(_name, _ip, _startTime.ToString(), MaxClientCount);
            Console.WriteLine($"Agent {_id}:  Hello({_name}, {_ip}, {_startTime}, {MaxClientCount}) --> {_id}");
        }

        public void Start()
        {
            _targetClientCount = 0;

            _controlLoopTask = Task.Run(AgentControlLoop);
        }

        public void Stop()
        {
            _stopped = true;
            _controlLoopTask.Wait();
        }

        private void SayBye()
        {
            _cc.Bye(_id);
            Console.WriteLine($"Agent {_id}:  Bye({_id})");
        }

        private int AskSync()
        {
            string respStr = _cc.Sync(_id, GetActiveClientCount(), _targetClientCount);
            int resp = Int32.Parse(respStr);
            Console.WriteLine($"Agent {_id}: Snyc({GetActiveClientCount()}, {_targetClientCount}) --> {resp}");
            return resp;
        }

        private void TellStat()
        {
            // process stats
            IEnumerable<IClient> clients = _clients.Values.Where(cli => cli._state <= 2).Select(ci => ci._cli);
            IEnumerable<ClientsStat> stats = clients.Select(cli => cli.GetStat());
            ClientsStat stat = ClientsStat.Aggregate(stats);
            string statStr = stat.ToString();
            Console.WriteLine($"Agent {_id}: Stat({GetActiveClientCount()}, '{statStr}')");
            _cc.AgentStat(_id, GetActiveClientCount(), statStr);
        }

        private int GetActiveClientCount()
        {
            lock (_clientLock)
            {
                return _clients.Values.Where(c => c._state <= 2).Count();
            }
        }

        private async Task AgentControlLoop()
        {
            DateTime lastReportTime = DateTime.Now.Subtract(new TimeSpan(24, 0, 0));
            while (true)
            {
                if (_stopped)
                    break;

                DateTime now = DateTime.Now;
                if (now.Subtract(lastReportTime).TotalMilliseconds > ReportingPeriod)
                {
                    // sync now
                    lastReportTime = now;
                    _targetClientCount = AskSync();
                    await ManageClients();
                    TellStat();
                }
                else
                {
                     await ManageClients();
                }

                await Task.Delay(ControlLoopPeriod);
            }

            TellStat();            
            SayBye();
            await CancelClients();
        }

        private async Task ManageClients()
        {
            int n = GetActiveClientCount();
            if (n == _targetClientCount)
            {
                // right number of clients
                return;
            }
            if (n < _targetClientCount)
            {
                // too few, start
                while (true)
                {
                    n = GetActiveClientCount();
                    if (n >= _targetClientCount)
                        break;
                    Console.WriteLine($"Agent {_id}: Too few clients, {n} vs. {_targetClientCount}");
                    await CreateClient();
                }
            }
            // too many, kill
            while (true)
            {
                n = GetActiveClientCount();
                if (n <= _targetClientCount)
                    break;
                Console.WriteLine($"Agent {_id}: Too many clients, {n} vs. {_targetClientCount}");
                await CancelClient();
            }
        }

        private async Task CreateClient()
        {
            IClient cli;
            lock (_clientLock)
            {
                string newId = (++_clientCounter).ToString();
                cli = _clientFactory.CreateNew(newId);
                _clients.Add(newId, new ClientInfo{ _cli = cli, _state = 1 });
            }
            await Task.Factory.StartNew(async () => await RunClient(cli));
            lock (_clientLock)
            {
                _clients[cli.Id]._state = 2;
            }
            //Console.WriteLine($"Agent {_id}: client {cli.Id} started");
        }

        private async Task CancelClient()
        {
            IClient cli = null;
            lock (_clientLock)
            {
                foreach(ClientInfo ci in _clients.Values)
                {
                    if (ci._state == 2)
                    {
                        cli = ci._cli;
                        break;
                    }
                }
                if (cli == null)
                {
                    // no active client found
                    return;
                }
                // cancel the cli
                cli.StopAsync();
                _clients[cli.Id]._state = 3;
            }
            //Console.WriteLine($"Agent {_id}: client {cli.Id} cancelled");
        }

        private async Task CancelClients()
        {
            lock (_clientLock)
            {
                foreach(string cli in _clients.Keys)
                {
                    if (_clients[cli]._state == 2)
                    {
                        _clients[cli]._cli.StopAsync();
                        _clients[cli]._state = 3;
                    }
                }
            }
        }

        private async Task RunClient(IClient cli)
        {
            Console.WriteLine($"Agent {_id}: client {cli.Id} started, {GetActiveClientCount()}");
            await cli.Run();
            Console.WriteLine($"Agent {_id}: client {cli.Id} finished");
            lock (_clientLock)
            {
                if (_clients.ContainsKey(cli.Id))
                {
                    _clients.Remove(cli.Id);
                    //Console.WriteLine($"Agent {_id}: client {cli.Id} removed, {GetClientCount()}");
                }
            }
        }
    }
}
