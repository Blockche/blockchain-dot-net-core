using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Blockche.Blockchain.Common;
using Blockche.Blockchain.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace Blockche.Blockchain.Web.Controllers
{
    
    public class ExplorerController : BaseController
    {

        public IActionResult Index()
        {

            return View();
        }

    }
}


























