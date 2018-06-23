using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Blockche.Miner.PoolWebApp.Models;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Configuration;
using Blockche.Miner.PoolWebApp.Mining;
using Blockche.Miner.PoolWebApp.Hubs;
using Microsoft.AspNetCore.SignalR;

namespace Blockche.Miner.PoolWebApp.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return this.View();
        }

        public IActionResult Blocks()
        {
            return this.View();
        }

        public IActionResult Users()
        {
            return this.View();
        }

        public IActionResult WebMiner()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
