using System;

namespace LoadTestCC.Proto1.Iface
{
    /// <summary>
    /// Command-and-Control, interface for agents
    /// </summary>
    public interface ICCAgent
    {
        string Hello(string agentName, string agentIp, string startTime, int maxClientCount);
        string Sync(string agentId, int clientCount, int targetClientCount);

        /// <param name="statString">Total count call</param>
        void AgentStat(string agentId, int clientCount, string statString);

        void Bye(string agentId);
    }
}