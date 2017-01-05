using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using LoadTestCC.Iface;

namespace LoadTestCC.LoadTestCC.Controllers
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
