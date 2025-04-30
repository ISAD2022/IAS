using AIS.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Diagnostics;

namespace AIS.Controllers
    {

    public class WorkingPaperController : Controller
        {
        private readonly ILogger<WorkingPaperController> _logger;
        private readonly TopMenus tm;
        private readonly SessionHandler sessionHandler;
        private readonly DBConnection dBConnection;

        public WorkingPaperController(ILogger<WorkingPaperController> logger, SessionHandler _sessionHandler, DBConnection _dbCon, TopMenus _tpMenu)
            {
            _logger = logger;
            sessionHandler = _sessionHandler;
            dBConnection = _dbCon;
            tm = _tpMenu;
            }

        public IActionResult loan_case_file()
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

        public IActionResult voucher_checking()
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

        public IActionResult account_opening()
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

        public IActionResult fixed_assets()
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

        public IActionResult cash_count()
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

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
            {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
            }
        }
    }
