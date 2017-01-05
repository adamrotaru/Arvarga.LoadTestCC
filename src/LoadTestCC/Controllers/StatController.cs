using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using LoadTestCC.Proto1.Iface;

namespace LoadTestCC.Proto1.LoadTestCC.Controllers
{
    [Route("ltcc/[controller]")]
    public class StatController : Controller
    {
        public StatController(ICCControl cc)
        {
            CC = cc;
        }
        public ICCControl CC { get; set; }

        [HttpGet("{agentId}/{clientCount}/{statString}", Name = "Stat")]
        public void Sync(string agentId, int clientCount, string statString)
        {
            CC.AgentStat(agentId, clientCount, statString);
            //Console.WriteLine($"AgentStat({agentId}, {clientCount}, {statString})");
        }
    }
}
