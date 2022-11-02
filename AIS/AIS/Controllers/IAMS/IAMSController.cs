using AIS.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Reflection;
namespace AIS.Controllers
{
    
    public class AMSController : Controller
    {
        private readonly ILogger<AMSController> _logger;
        private readonly TopMenus tm;
        private readonly SessionHandler sessionHandler;
        private readonly DBConnection dBConnection;
        public AMSController(ILogger<AMSController> logger, SessionHandler _sessionHandler, DBConnection _dbCon, TopMenus _tpMenu)
        {
            _logger = logger;
            sessionHandler = _sessionHandler;
            dBConnection = _dbCon;
            tm = _tpMenu;
        }
        [HttpGet("IAMS/paras")]
        public IActionResult paras()
        {
            ViewData["TopMenu"] = tm.GetTopMenus();
            ViewData["TopMenuPages"] = tm.GetTopMenusPages();
            ViewData["DivisionList"] = dBConnection.GetDivisions(false);
            ViewData["ProcessList"] = dBConnection.GetRiskProcessDefinition();
            ViewData["subProcessList"] = dBConnection.GetRiskProcessDetails();
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
                    return View("../IAMS/paras");
            }
        }

        [HttpGet("IAMS/old_para")]
        public IActionResult old_para()
        {
            ViewData["TopMenu"] = tm.GetTopMenus();
            ViewData["TopMenuPages"] = tm.GetTopMenusPages();
            ViewData["ProcessList"] = dBConnection.GetRiskProcessDefinition();
            ViewData["EntitiesList"] = dBConnection.GetAuditeeEntitiesForOldParas(0);
            ViewData["AuditYearList"] = dBConnection.GetOldParasAuditYear();
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
                    return View("../IAMS/old_para");
            }
        }



        


    }
}
