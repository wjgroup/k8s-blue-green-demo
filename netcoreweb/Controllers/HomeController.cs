using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using netcoreweb.Models;

namespace netcoreweb.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            this.ViewBag.Scheme = Request.Scheme;
            this.ViewBag.Path = Request.Path;
            this.ViewBag.PathBase = Request.PathBase;
            this.ViewBag.MachineName = Environment.MachineName;

            return View();
        }

        public IActionResult Privacy()
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
