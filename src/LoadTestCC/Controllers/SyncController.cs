using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using LoadTestCC.Proto1.Iface;

namespace LoadTestCC.Proto1.LoadTestCC.Controllers
{
    [Route("ltcc/[controller]")]
    public class SyncController : Controller
    {
        public SyncController(ICCControl cc)
        {
            CC = cc;
        }
        public ICCControl CC { get; set; }

        [HttpGet("{agentId}/{clientCount}/{targetClientCount}", Name = "Sync")]
        public string Sync(string agentId, int clientCount, int targetClientCount)
        {
            string newTargetClientCount = CC.Sync(agentId, clientCount, targetClientCount);
            //Console.WriteLine($"Sync({agentId}, {clientCount}, {targetClientCount}) --> {newTargetClientCount}");
            return newTargetClientCount;
        }
    }
}
