using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using LoadTestCC.Iface;

namespace LoadTestCC.LoadTestCC.Controllers
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
