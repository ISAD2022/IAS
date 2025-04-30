using AIS.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Diagnostics;
using System.Reflection;


namespace AIS.Controllers
    {

    public class HMController : Controller
        {
        private readonly ILogger<HMController> _logger;
        private readonly TopMenus tm;
        private readonly SessionHandler sessionHandler;
        private readonly DBConnection dBConnection;
        public HMController(ILogger<HMController> logger, SessionHandler _sessionHandler, DBConnection _dbCon, TopMenus _tpMenu)
            {
            _logger = logger;
            sessionHandler = _sessionHandler;
            dBConnection = _dbCon;
            tm = _tpMenu;
            }
        public IActionResult dashboard()
            {
            ViewData["TopMenu"] = tm.GetTopMenus();
            ViewData["TopMenuPages"] = tm.GetTopMenusPages();
            ViewData["ActiveInactive"] = dBConnection.GetActiveInactiveChartData();
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
        public IActionResult old_paras_monitoring()
            {
            ViewData["TopMenu"] = tm.GetTopMenus();
            ViewData["TopMenuPages"] = tm.GetTopMenusPages();
            ViewData["ZonesList"] = dBConnection.GetZonesoldparamointoring();
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

        public IActionResult old_paras_monitoring_ppno()
            {
            ViewData["TopMenu"] = tm.GetTopMenus();
            ViewData["TopMenuPages"] = tm.GetTopMenusPages();
            ViewData["ZonesList"] = dBConnection.GetZones();
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

        public IActionResult Error()
            {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
            }
        }
    }
