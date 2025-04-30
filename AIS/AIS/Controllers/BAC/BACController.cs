using AIS.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Diagnostics;
namespace AIS.Controllers
    {

    public class BACController : Controller
        {
        private readonly ILogger<BACController> _logger;
        private readonly TopMenus tm;
        private readonly SessionHandler sessionHandler;
        private readonly DBConnection dBConnection;
        public BACController(ILogger<BACController> logger, SessionHandler _sessionHandler, DBConnection _dbCon, TopMenus _tpMenu)
            {
            _logger = logger;
            sessionHandler = _sessionHandler;
            dBConnection = _dbCon;
            tm = _tpMenu;
            }

        [HttpGet("BAC/dashboard")]
        public IActionResult dashboard()
            {
            ViewData["TopMenu"] = tm.GetTopMenus();
            ViewData["TopMenuPages"] = tm.GetTopMenusPages();
            if (!sessionHandler.IsUserLoggedIn())
                {
                return RedirectToAction("Index", "Login");
                }
            else
                {
                return View("../BAC/dashboard");
                }
            }

        [HttpGet("BAC/cia_analysis")]
        public IActionResult cia_analysis()
            {
            ViewData["TopMenu"] = tm.GetTopMenus();
            ViewData["TopMenuPages"] = tm.GetTopMenusPages();
            ViewData["OptionList"] = dBConnection.GetBACCIAAnalysisOptions();
            if (!sessionHandler.IsUserLoggedIn())
                {
                return RedirectToAction("Index", "Login");
                }
            else
                {
                return View("../BAC/cia_analysis");
                }
            }

        [HttpGet("BAC/cia_analysis_detail")]
        public IActionResult cia_analysis_detail()
            {
            ViewData["TopMenu"] = tm.GetTopMenus();
            ViewData["TopMenuPages"] = tm.GetTopMenusPages();
            ViewData["OptionList"] = dBConnection.GetBACCIAAnalysisOptions();
            if (!sessionHandler.IsUserLoggedIn())
                {
                return RedirectToAction("Index", "Login");
                }
            else
                {
                return View("../BAC/cia_analysis_detail");
                }
            }

        [HttpGet("BAC/agenda")]
        public IActionResult agenda()
            {
            ViewData["TopMenu"] = tm.GetTopMenus();
            ViewData["TopMenuPages"] = tm.GetTopMenusPages();
            if (!sessionHandler.IsUserLoggedIn())
                {
                return RedirectToAction("Index", "Login");
                }
            else
                {
                return View("../BAC/agenda");
                }
            }


        [HttpGet("BAC/agenda_approval")]
        public IActionResult agenda_approval()
            {
            ViewData["TopMenu"] = tm.GetTopMenus();
            ViewData["TopMenuPages"] = tm.GetTopMenusPages();
            if (!sessionHandler.IsUserLoggedIn())
                {
                return RedirectToAction("Index", "Login");
                }
            else
                {
                return View("../BAC/agenda_approval");
                }
            }

        [HttpGet("BAC/assignment")]
        public IActionResult assignment()
            {
            ViewData["TopMenu"] = tm.GetTopMenus();
            ViewData["TopMenuPages"] = tm.GetTopMenusPages();
            if (!sessionHandler.IsUserLoggedIn())
                {
                return RedirectToAction("Index", "Login");
                }
            else
                {
                return View("../BAC/assignment");
                }
            }

        [HttpGet("BAC/bac_assignment_response")]
        public IActionResult bac_assignment_response()
            {
            ViewData["TopMenu"] = tm.GetTopMenus();
            ViewData["TopMenuPages"] = tm.GetTopMenusPages();
            if (!sessionHandler.IsUserLoggedIn())
                {
                return RedirectToAction("Index", "Login");
                }
            else
                {
                return View("../BAC/bac_assignment_response");
                }
            }

        [HttpGet("BAC/mom")]
        public IActionResult mom()
            {
            ViewData["TopMenu"] = tm.GetTopMenus();
            ViewData["TopMenuPages"] = tm.GetTopMenusPages();
            if (!sessionHandler.IsUserLoggedIn())
                {
                return RedirectToAction("Index", "Login");
                }
            else
                {
                return View("../BAC/mom");
                }
            }

        [HttpGet("BAC/notice")]
        public IActionResult notice()
            {
            ViewData["TopMenu"] = tm.GetTopMenus();
            ViewData["TopMenuPages"] = tm.GetTopMenusPages();
            if (!sessionHandler.IsUserLoggedIn())
                {
                return RedirectToAction("Index", "Login");
                }
            else
                {
                return View("../BAC/notice");
                }
            }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
            {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
            }
        }
    }
