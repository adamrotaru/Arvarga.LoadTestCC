using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using LoadTestCC.Proto1.Iface;

namespace LoadTestCC.Proto1.LoadTestCC.Controllers
{
    [Route("ltcc/[controller]")]
    public class GetstatusController : Controller
    {
        public GetstatusController(ICCControl cc)
        {
            CC = cc;
        }
        public ICCControl CC { get; set; }

        [HttpGet]
        public string GetStatus()
        {
            return CC.GetStatus();
        }
    }
}
