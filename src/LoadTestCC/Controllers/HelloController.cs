using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using LoadTestCC.Iface;

namespace LoadTestCC.LoadTestCC.Controllers
{
    /// <summary>
    /// Example calls:
    ///  http://localhost:5000/ltcc/getstatus
    ///  http://localhost:5000/ltcc/hello/name/b/c/10
    ///  http://localhost:5000/ltcc/settarget/5
    ///  http://localhost:5000/ltcc/sync/1/0/10
    /// </summary>
    [Route("ltcc/[controller]")]
    public class HelloController : Controller
    {
        public HelloController(ICCControl cc)
        {
            CC = cc;
        }
        public ICCControl CC { get; set; }

        [HttpGet("{agentName}/{agentIp}/{startTime}/{maxClientCount}", Name = "Hello")]
        public string Hello(string agentName, string agentIp, string startTime, int maxClientCount)
        {
            string agentId = CC.Hello(agentName, agentIp, startTime, maxClientCount);
            Console.WriteLine($"Hello({agentName}, {agentIp}, {startTime}, {maxClientCount}) --> {agentId}");
            return agentId;
        }
    }
}
