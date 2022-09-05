using InspectionSolution.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using System.Linq;
using System.Threading.Tasks;

namespace InspectionSolution.Controllers
{


    public class PlanningController : Controller
    {
        private readonly ILogger<PlanningController> _logger;

        private readonly TopMenus tm = new TopMenus();
        private readonly DBConnection dBConnection = new DBConnection();
        private readonly SessionHandler sessionHandler = new SessionHandler();
        public PlanningController(ILogger<PlanningController> logger)
        {
            _logger = logger;
        }

        public IActionResult audit_criteria()
        {
            ViewData["TopMenu"] = tm.GetTopMenus();
            ViewData["TopMenuPages"] = tm.GetTopMenusPages();
            ViewData["AuditEntities"] = dBConnection.GetAuditEntities();
            ViewData["AuditPeriodList"] = dBConnection.GetAuditPeriods();
            ViewData["AuditFrequencies"] = dBConnection.GetAuditFrequencies();
            ViewData["BranchSizesList"] = dBConnection.GetBranchSizes();
            ViewData["RiskList"] = dBConnection.GetRisks();
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


        public IActionResult refferedback_audit_criteria()
        {
            ViewData["TopMenu"] = tm.GetTopMenus();
            ViewData["TopMenuPages"] = tm.GetTopMenusPages();
            ViewData["ReferedBackAuditCriteriaList"] = dBConnection.GetRefferedBackAuditCriterias();
            ViewData["AuditEntities"] = dBConnection.GetAuditEntities();
            ViewData["AuditPeriodList"] = dBConnection.GetAuditPeriods();
            ViewData["AuditFrequencies"] = dBConnection.GetAuditFrequencies();
            ViewData["BranchSizesList"] = dBConnection.GetBranchSizes();
            ViewData["RiskList"] = dBConnection.GetRisks();
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



        public IActionResult audit_criteria_approval()
        {
            ViewData["TopMenu"] = tm.GetTopMenus();
            ViewData["TopMenuPages"] = tm.GetTopMenusPages();
            ViewData["ToAuthorizeAuditCriteriaList"] = dBConnection.GetAuditCriteriasToAuthorize();
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


        public IActionResult special_assignment()
        {
            ViewData["TopMenu"] = tm.GetTopMenus();
            ViewData["TopMenuPages"] = tm.GetTopMenusPages();
            ViewData["AuditDepartments"] = dBConnection.GetDepartments(354);
            ViewData["DivisionsList"] = dBConnection.GetDivisions(false);
            ViewData["AuditZonesList"] = dBConnection.GetZones();
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


        public IActionResult Index()
        {
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
