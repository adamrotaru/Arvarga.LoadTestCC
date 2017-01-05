using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using LoadTestCC.Iface;

namespace LoadTestCC.LoadTestCC.Controllers
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
