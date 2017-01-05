using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using LoadTestCC.Iface;

namespace LoadTestCC.LoadTestCC.Controllers
{
    [Route("ltcc/[controller]")]
    public class ByeController : Controller
    {
        public ByeController(ICCControl cc)
        {
            CC = cc;
        }
        public ICCControl CC { get; set; }

        [HttpGet("{agentId}", Name = "Bye")]
        public void Bye(string agentId)
        {
            CC.Bye(agentId);
            Console.WriteLine($"Bye({agentId})");
        }
    }
}
