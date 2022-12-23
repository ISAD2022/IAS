using AIS.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
namespace AIS.Controllers
{
    public class InspectionController : Controller
    {
        private readonly ILogger<InspectionController> _logger;
        private readonly TopMenus tm;
        private readonly SessionHandler sessionHandler;
        private readonly DBConnection dBConnection;
        public InspectionController(ILogger<InspectionController> logger, SessionHandler _sessionHandler, DBConnection _dbCon, TopMenus _tpMenu)
        {
            _logger = logger;
            sessionHandler = _sessionHandler;
            dBConnection = _dbCon;
            tm = _tpMenu;
        }
        [HttpGet("Inspection/Home")]
        public IActionResult Home()
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
                    return View("../Inspection/Home/Index");
            }
        }


        /////////////////////////////////////////////////////////////////////////////////////////////////////
        [HttpGet("Inspection/inspection_criteria")]
        public IActionResult inspection_criteria()
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
                    return View("../Inspection/Planning/inspection_criteria");
            }
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////
        


     [HttpGet("Inspection/tentative_plan")]
        public IActionResult tentative_plan()
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
                    return View("../Inspection/Planning/tentative_plan");
            }
        }


        ////////////////////////////////////////////////////////////////////////////////////////////////////////////
        


 [HttpGet("Inspection/engagement_plan")]
        public IActionResult engagement_plan()
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
                    return View("../Inspection/Planning/engagement_plan");
            }
        }



        ///////////////////////////////////////////////////////////////////////////////////////////////////////////
        

 [HttpGet("Inspection/task_list")]
        public IActionResult task_list()
        {
            ViewData["TopMenu"] = tm.GetTopMenus();
            ViewData["TopMenuPages"] = tm.GetTopMenusPages();
            var loggedInUser = sessionHandler.GetSessionUser();
            ViewData["PPNumber"] = loggedInUser.PPNumber;
            ViewData["EMP_NAME"] = loggedInUser.Name;
            List<TaskListModel> tlist = dBConnection.GetTaskList();
            foreach (var item in tlist)
            {
                ViewData["TEAM_NAME"] = item.TEAM_NAME.ToString();
            }
            ViewData["TaskList"] = tlist;

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
                    return View("../Inspection/Planning/task_list");
            }
        }



        ////////////////////////////////////////////////////////////////////////////////////////////////////////////

        [HttpGet("Inspection/Inspection_Assigned")]
        public IActionResult Inspection_Assigned()
        {
            ViewData["TopMenu"] = tm.GetTopMenus();
            ViewData["TopMenuPages"] = tm.GetTopMenusPages();
            ViewData["EntitiesList"] = dBConnection.GetAuditeeAssignedEntities();
            //ViewData["AssignedObservations"] = dBConnection.GetAssignedObservations();
            if (!sessionHandler.IsUserLoggedIn())
                return RedirectToAction("Index", "Login");
            else
            {
                if (!sessionHandler.HasPermissionToViewPage("home"))
                {
                    return RedirectToAction("Index", "PageNotFound");
                }
                else
                    return View("../Inspection/Planning/Inspection_Assigned");
            }
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////
        [HttpGet("Inspection/Manage_Inspection_Observation")]

        public IActionResult Manage_Inspection_Observation(int engId = 0)
        {
            ViewData["TopMenu"] = tm.GetTopMenus();
            ViewData["TopMenuPages"] = tm.GetTopMenusPages();
            ViewData["EntitiesList"] = dBConnection.GetObservationEntities();
            ViewData["ManageObservations"] = "";
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
                    return View("../Inspection/Planning/Manage_Inspection_Observation");
            }
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////
        [HttpGet("Inspection/Inspection_Report")]

        public IActionResult Inspection_Report(int engId = 0)
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
                    return View("../Inspection/Planning/Inspection_Report");
            }
        }
        ///////////////////////////////////////////////////////////////////////////////////////////////////////////////
        [HttpGet("Inspection/Inspection_Observation")]
        public IActionResult Inspection_Observation()
        {
            ViewData["TopMenu"] = tm.GetTopMenus();
            ViewData["TopMenuPages"] = tm.GetTopMenusPages();
            ViewData["RISK_GROUPS"] = dBConnection.GetRiskGroup();
            ViewData["ZonesList"] = dBConnection.GetZones(false);
            ViewData["ProcessList"] = dBConnection.GetRiskProcessDefinition();
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
                    return View("../Inspection/Planning/Inspection_Observation");
            }
        }




        ////////////////////////////////////////////////////////////////////////////////////////////////////////////

        [HttpGet("Inspection/manage_Inspection_report_paras")]
        public IActionResult manage_Inspection_report_paras(int engId = 0)
        {
            ViewData["TopMenu"] = tm.GetTopMenus();
            ViewData["TopMenuPages"] = tm.GetTopMenusPages();
            ViewData["EntitiesList"] = dBConnection.GetObservationEntities();

            //ViewData["ManageObservations"] = dBConnection.GetManagedDraftObservations(engId);
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
                    return View("../Inspection/Planning/manage_Inspection_report_paras");
            }
        }


        ////////////////////////////////////////////////////////////////////////////////////////////////////////////
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
