using AIS.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Microsoft.AspNetCore.Http;
using System.Reflection;

namespace AIS.Controllers
{
    [Route("Inspection/Home")]
    public class InsHomeController : Controller
    {
        private readonly ILogger<InsHomeController> _logger;
        private readonly TopMenus tm = new TopMenus();
        private readonly SessionHandler sessionHandler = new SessionHandler();

        public InsHomeController(ILogger<InsHomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
           ViewData["TopMenu"] = tm.GetTopMenus();
           ViewData["TopMenuPages"] = tm.GetTopMenusPages();
            if (!sessionHandler.IsUserLoggedIn())
                return RedirectToAction("Index", "Login");
            else
            {
                if (!sessionHandler.HasPermissionToViewPage("home")) {
                    return RedirectToAction("Index", "PageNotFound"); 
                }
                else
                    return View();
            }
        }

        public IActionResult Privacy()
        {
            ViewData["TopMenu"] = tm.GetTopMenus();
            ViewData["TopMenuPages"] = tm.GetTopMenusPages();
            if (!sessionHandler.IsUserLoggedIn())
            { 
                return RedirectToAction("Index", "Login"); 
            }
            else
            {
                if (!sessionHandler.HasPermissionToViewPage(MethodBase.GetCurrentMethod().Name))
                {
                    return RedirectToAction("Index", "PageNotFound");
                }
                else
                    return View();
            }
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
