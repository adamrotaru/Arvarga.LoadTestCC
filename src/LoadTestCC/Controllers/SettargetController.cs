using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using LoadTestCC.Proto1.Iface;

namespace LoadTestCC.Proto1.LoadTestCC.Controllers
{
    [Route("ltcc/[controller]")]
    public class SettargetController : Controller
    {
        public SettargetController(ICCControl cc)
        {
            CC = cc;
        }
        public ICCControl CC { get; set; }

        [HttpGet("{newTargetCount}", Name = "SetTarget")]
        public void SetTarget(long newTargetCount)
        {
            CC.SetTarget(newTargetCount);
            Console.WriteLine($"SetTarget({newTargetCount})");
        }
    }
}
