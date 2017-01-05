using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LoadTestCC.Proto1.Iface;

namespace LoadTestCC.Proto1.CommCont
{
    internal class AgentInfo
    {
        public string _id;
        public string _name;
        public string _ip;
        public string _startTime;
        public int _clientCount;
        public int _targetClientCount;
        // 1 active  2 timedout/closed
        public int _state;
        public DateTime _lastSeen;
        public int _maxClientCount;
        public ClientsStat _stat;
    }

    /// <summary>Command and Control class for load test</summary>
    public class CommCont : ICCAgent, ICCControl
    {
        private Dictionary<string, AgentInfo> _agents = new Dictionary<string, AgentInfo>();
        private object _agentsLock = new object();
        private long _agentIdCounter = 0;
        private long _targetCliCount;
        private long _targetTargetCliCount;
        private DateTime _targetSetTime;
        private long _targetPrev;
        private const int HousekeepingPeriod = 1000;
        private const int ClientTimeoutPeriod = 60000;
        private const int TimedoutClientPurgePeriod = 600000;
        public const int ClientCreateRate = 3;
        private bool _stopped;
        private Task _loopTask;

        public CommCont()
        {
        }
                
        public void Start()
        {
            SetTarget(0);
            _loopTask = Task.Run(HousekeepingLoop);
            PrintShortStat();
        }
        
        public void Stop()
        {
            _stopped = true;
            _loopTask.Wait();
        }

        public void PrintShortStat()
        {
            Console.WriteLine($"CC:  agents: {ActiveAgentCount}/{_agents.Count}  clients target: {_targetCliCount}/{_targetTargetCliCount}  totAct: {TotalClientCountActual}");
        }

        /*public void PrintLongStat()
        {
            PrintShortStat();
            DateTime now = DateTime.Now;
            lock (_agentsLock)
            {
                foreach(AgentInfo a in _agents.Values)
                {
                    Console.WriteLine($"  {a._id}: act {a._clientCount} trg {a._targetClientCount}  last {(int)now.Subtract(a._lastSeen).TotalSeconds} s  stat {a._stat}  name {a._name}");
                }
            }
        }*/

        public int ActiveAgentCount 
        {
            get 
            { 
                lock(_agentsLock) 
                { 
                    return _agents.Values.Where(a => a._state == 1).Count();
                } 
            }
        }

        public int TotalClientCountActual
        {
            get 
            { 
                lock(_agentsLock) 
                { 
                    int s = 0;
                    foreach(AgentInfo a in _agents.Values) 
                        if (a._state == 1)
                            s += a._clientCount;
                    return s; 
                } 
            }
        }

        public void SetTarget(long newTargetCount)
        {
            if (_targetTargetCliCount != newTargetCount)
            {
                _targetPrev = _targetTargetCliCount;
                _targetSetTime = DateTime.Now;
                _targetTargetCliCount = newTargetCount;
                TargetChanged();
            }
        }

        public string GetStatus()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine($"agents: {ActiveAgentCount}/{_agents.Count}  clients target: {_targetCliCount}/{_targetTargetCliCount}  totAct: {TotalClientCountActual}");
            DateTime now = DateTime.Now;
            lock (_agentsLock)
            {
                List<ClientsStat> stats = new List<ClientsStat>();
                foreach(AgentInfo a in _agents.Values)
                {
                    sb.AppendLine($"  {a._id}: act {a._clientCount} trg {a._targetClientCount}  st {a._state}  last {(int)now.Subtract(a._lastSeen).TotalSeconds} s  stat ({a._stat?.ToNiceString()})  name {a._name}");
                    if (a._state == 1)
                    {
                        stats.Add(a._stat);
                    }
                }
                ClientsStat aggregateStat = ClientsStat.Aggregate(stats);
                sb.AppendLine($"  astat ({aggregateStat.ToNiceString()})");
            }
            return sb.ToString();
        }

        private void TargetChanged()
        {
            RampupTarget();
            PrintShortStat();
            DistributeTargetCount();
        }

        private void AgentsChanged()
        {
            PrintShortStat();
            DistributeTargetCount();
        }

        private bool RampupTarget()
        {
            if (_targetCliCount == _targetTargetCliCount)
            {
                // target reached
                return false;
            }
            long newTarget = 0;
            if (_targetCliCount > _targetTargetCliCount)
            {
                // too high, drop
                newTarget = _targetTargetCliCount;
            }
            else
            {
                double timeElapsed = DateTime.Now.Subtract(_targetSetTime).TotalSeconds;
                int increment = (int)(timeElapsed * (double)ClientCreateRate);
                newTarget = Math.Min(_targetPrev + increment, _targetTargetCliCount);
            }
            if (newTarget != _targetCliCount)
            {
                _targetCliCount = newTarget;
                PrintShortStat();
                return true;
            }
            return false;
        }

        /// <summary>
        /// Distribute the target clients to the available agents
        /// </summary>
        private void DistributeTargetCount()
        {
            //Console.WriteLine($"DistributeTargetCount {_targetCliCount}");
            lock (_agentsLock)
            {
                int n = ActiveAgentCount;
                int cPerAgent = 0;
                if (n != 0)
                {
                    cPerAgent = (int)(_targetCliCount / n);
                }
                long cRem = _targetCliCount;
                int z = 0;
                foreach(AgentInfo a in _agents.Values)
                {
                    a._targetClientCount = 0;
                    if (a._state != 1) continue;
                    int c1;
                    if (z == n-1)
                    {
                        // last agent
                        c1 = (int)Math.Min(cRem, a._maxClientCount);
                    }
                    else
                    {
                        c1 = (int)Math.Min(cPerAgent, Math.Min(cRem, a._maxClientCount));
                    }
                    a._targetClientCount = c1;
                    cRem -= c1;
                    ++z;
                }
            }
            //PrintLongStat();
        }

        public string Hello(string agentName, string agentIp, string startTime, int maxClientCount)
        {
            AgentInfo info = new AgentInfo
            { 
                _name = agentName, _ip = agentIp, _startTime = startTime, 
                _state = 1, _lastSeen = DateTime.Now, _maxClientCount = maxClientCount 
            };
            string newId;
            lock (_agentsLock)
            {
                newId = (++_agentIdCounter).ToString();
                info._id = newId;
                _agents.Add(newId, info);
            }
            AgentsChanged();
            return newId;
        }

        public string Sync(string agentId, int clientCount, int targetClientCount)
        {
            int responseTargetCount = 0;
            bool changed = false;
            lock (_agentsLock)
            {
                if (!_agents.ContainsKey(agentId)) 
                {
                    Console.WriteLine($"WARNING: Agent {agentId} not found");
                    return "0";
                }
                _agents[agentId]._state = 1;
                _agents[agentId]._lastSeen = DateTime.Now;
                int oldCount = _agents[agentId]._clientCount;
                if (oldCount != clientCount)
                {
                    _agents[agentId]._clientCount = clientCount;
                    Console.WriteLine($"CC: agent {agentId} has {clientCount}");
                    changed = true;
                }
                responseTargetCount = _agents[agentId]._targetClientCount;
            }
            if (changed)
                AgentsChanged();
            return responseTargetCount.ToString();
        }

        public void AgentStat(string agentId, int clientCount, string statString)
        {
            bool changed = false;
            ClientsStat stat = ClientsStat.FromString(statString);
            lock (_agentsLock)
            {
                if (!_agents.ContainsKey(agentId)) 
                {
                    Console.WriteLine($"WARNING: Agent {agentId} not found");
                    return;
                }
                _agents[agentId]._state = 1;
                _agents[agentId]._stat = stat;
                _agents[agentId]._lastSeen = DateTime.Now;
                int oldCount = _agents[agentId]._clientCount;
                if (oldCount != clientCount)
                {
                    _agents[agentId]._clientCount = clientCount;
                    changed = true;
                }
            }
            if (changed)
            {
                AgentsChanged();
            }
        }

        public void Bye(string agentId)
        {
            bool changed = false;
            lock (_agentsLock)
            {
                if (!_agents.ContainsKey(agentId)) 
                {
                    Console.WriteLine($"WARNING: Agent {agentId} not found");
                    return;
                }
                _agents[agentId]._state = 2;
                _agents[agentId]._clientCount = 0;
                _agents[agentId]._targetClientCount = 0;
                _agents[agentId]._lastSeen = DateTime.Now;
                changed = true;
            }
            if (changed)
            {
                AgentsChanged();
            }
        }

        private async Task HousekeepingLoop()
        {
            while (true)
            {
                if (_stopped)
                    break;

                bool agentsChanged = false;
                DateTime now = DateTime.Now;
                // check for old clients, time them out
                lock (_agentsLock)
                {
                    foreach(AgentInfo a in _agents.Values)
                    { 
                        if (a._state == 1)
                        {
                            double lastSeen = now.Subtract(a._lastSeen).TotalMilliseconds;
                            if (lastSeen > ClientTimeoutPeriod)
                            {
                                _agents[a._id]._state = 2;
                                _agents[a._id]._clientCount = 0;
                                _agents[a._id]._targetClientCount = 0;
                                agentsChanged = true;
                            }
                        }
                    }
                }

                // purge timed-out clients
                lock (_agentsLock)
                {
                    bool oldFound = false;
                    do
                    {
                        oldFound = false;
                        foreach(AgentInfo a in _agents.Values)
                        { 
                            if (a._state == 2)
                            {
                                double lastSeen = now.Subtract(a._lastSeen).TotalMilliseconds;
                                if (lastSeen > TimedoutClientPurgePeriod)
                                {
                                    _agents.Remove(a._id);
                                    oldFound = true;
                                    agentsChanged = true;
                                    break;
                                }
                            }
                        }
                    } while (oldFound);
                }
                bool targetChanged = RampupTarget();
                if (agentsChanged || targetChanged)
                {
                    AgentsChanged();
                }

                await Task.Delay(HousekeepingPeriod);
            }
        }
    }
}
