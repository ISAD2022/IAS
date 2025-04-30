using AIS.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Diagnostics;
using System.Reflection;

namespace AIS.Controllers
    {

    public class HomeController : Controller
        {
        private readonly ILogger<HomeController> _logger;
        private readonly TopMenus tm;
        private readonly SessionHandler sessionHandler;
        private readonly DBConnection dBConnection;

        public HomeController(ILogger<HomeController> logger, SessionHandler _sessionHandler, DBConnection _dbCon, TopMenus _tpMenu)
            {
            _logger = logger;
            sessionHandler = _sessionHandler;
            dBConnection = _dbCon;
            tm = _tpMenu;
            }
        public IActionResult Index()
            {
            if (!sessionHandler.IsUserLoggedIn())
                return RedirectToAction("Index", "Login");
            else
                {
                if (!sessionHandler.HasPermissionToViewPage("home"))
                    {
                    return RedirectToAction("Index", "PageNotFound");
                    }
                else
                    {
                    ViewData["TopMenu"] = tm.GetTopMenus();
                    ViewData["TopMenuPages"] = tm.GetTopMenusPages();
                    return View();
                    }

                }
            }
        public IActionResult Change_Password()
            {
            ViewData["TopMenu"] = tm.GetTopMenus();
            ViewData["TopMenuPages"] = tm.GetTopMenusPages();
            TempData["Message"] = "";
            TempData["SessionKill"] = "";
            return View();
            }
        [HttpPost]
        public bool DoChangePassword(string Password, string NewPassword)
            {
            return dBConnection.ChangePassword(Password, NewPassword);
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
