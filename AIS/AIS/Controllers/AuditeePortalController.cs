using AIS.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Diagnostics;

namespace AIS.Controllers
    {

    public class AuditeePortalController : Controller
        {
        private readonly ILogger<AuditeePortalController> _logger;
        private readonly TopMenus tm;
        private readonly SessionHandler sessionHandler;
        private readonly DBConnection dBConnection;
        public AuditeePortalController(ILogger<AuditeePortalController> logger, SessionHandler _sessionHandler, DBConnection _dbCon, TopMenus _tpMenu)
            {
            _logger = logger;
            sessionHandler = _sessionHandler;
            dBConnection = _dbCon;
            tm = _tpMenu;
            }

        public IActionResult observation_assigned()
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
                    return View();
                }
            }

        /////////////////////////

        public IActionResult Para_Text_Update_FAD()
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
                    return View();
                }
            }

        /////////////////////////

        public IActionResult old_outstanding_paras()
            {
            ViewData["TopMenu"] = tm.GetTopMenus();
            ViewData["TopMenuPages"] = tm.GetTopMenusPages();
            ViewData["OldParasEntities"] = dBConnection.GetAuditeeOldParasEntities();
            if (!sessionHandler.IsUserLoggedIn())
                return RedirectToAction("Index", "Login");
            else
                {
                if (!sessionHandler.HasPermissionToViewPage("home"))
                    {
                    return RedirectToAction("Index", "PageNotFound");
                    }
                else
                    return View();
                }
            }
        public IActionResult old_para_reply()
            {
            ViewData["TopMenu"] = tm.GetTopMenus();
            ViewData["TopMenuPages"] = tm.GetTopMenusPages();
            if (!sessionHandler.IsUserLoggedIn())
                return RedirectToAction("Index", "Login");
            else
                {
                if (!sessionHandler.HasPermissionToViewPage("home"))
                    {
                    return RedirectToAction("Index", "PageNotFound");
                    }
                else
                    return View();
                }
            }
        public IActionResult old_para_reply_cad()
            {
            ViewData["TopMenu"] = tm.GetTopMenus();
            ViewData["TopMenuPages"] = tm.GetTopMenusPages();
            ViewData["Voilation_Cat"] = dBConnection.GetAuditVoilationcats();
            ViewData["RiskList"] = dBConnection.GetRisks();
            if (!sessionHandler.IsUserLoggedIn())
                return RedirectToAction("Index", "Login");
            else
                {
                if (!sessionHandler.HasPermissionToViewPage("home"))
                    {
                    return RedirectToAction("Index", "PageNotFound");
                    }
                else
                    return View();
                }
            }
        public IActionResult ccqs()
            {
            ViewData["TopMenu"] = tm.GetTopMenus();
            ViewData["TopMenuPages"] = tm.GetTopMenusPages();
            ViewData["EntitiesList"] = dBConnection.GetCCQsEntities();
            ViewData["ControlViolationList"] = dBConnection.GetAuditVoilationcats();
            ViewData["RiskList"] = dBConnection.GetCOSORisks();
            ViewData["AUDITCCQ"] = "";
            if (!sessionHandler.IsUserLoggedIn())
                return RedirectToAction("Index", "Login");
            else
                {
                if (!sessionHandler.HasPermissionToViewPage("home"))
                    {
                    return RedirectToAction("Index", "PageNotFound");
                    }
                else
                    return View();
                }
            }
        public IActionResult reply()
            {
            ViewData["TopMenu"] = tm.GetTopMenus();
            ViewData["TopMenuPages"] = tm.GetTopMenusPages();
            if (!sessionHandler.IsUserLoggedIn())
                return RedirectToAction("Index", "Login");
            else
                {
                if (!sessionHandler.HasPermissionToViewPage("home"))
                    {
                    return RedirectToAction("Index", "PageNotFound");
                    }
                else
                    return View();
                }
            }
        public IActionResult reply_new()
            {
            ViewData["TopMenu"] = tm.GetTopMenus();
            ViewData["TopMenuPages"] = tm.GetTopMenusPages();
            if (!sessionHandler.IsUserLoggedIn())
                return RedirectToAction("Index", "Login");
            else
                {
                if (!sessionHandler.HasPermissionToViewPage("home"))
                    {
                    return RedirectToAction("Index", "PageNotFound");
                    }
                else
                    return View();
                }
            }
        public IActionResult paras_compliance_by_auditee()
            {
            ViewData["TopMenu"] = tm.GetTopMenus();
            ViewData["TopMenuPages"] = tm.GetTopMenusPages();

            if (!sessionHandler.IsUserLoggedIn())
                return RedirectToAction("Index", "Login");
            else
                {
                if (!sessionHandler.HasPermissionToViewPage("home"))
                    {
                    return RedirectToAction("Index", "PageNotFound");
                    }
                else
                    return View();
                }
            }

        public IActionResult old_para_br_comp_ref()
            {
            ViewData["TopMenu"] = tm.GetTopMenus();
            ViewData["TopMenuPages"] = tm.GetTopMenusPages();
            if (!sessionHandler.IsUserLoggedIn())
                return RedirectToAction("Index", "Login");
            else
                {
                if (!sessionHandler.HasPermissionToViewPage("home"))
                    {
                    return RedirectToAction("Index", "PageNotFound");
                    }
                else
                    return View();
                }
            }

        public IActionResult old_para_br_comp_review()
            {
            ViewData["TopMenu"] = tm.GetTopMenus();
            ViewData["TopMenuPages"] = tm.GetTopMenusPages();


            if (!sessionHandler.IsUserLoggedIn())
                return RedirectToAction("Index", "Login");
            else
                {
                if (!sessionHandler.HasPermissionToViewPage("home"))
                    {
                    return RedirectToAction("Index", "PageNotFound");
                    }
                else
                    return View();
                }
            }


        public IActionResult old_para_br_comp_review_ref()
            {
            ViewData["TopMenu"] = tm.GetTopMenus();
            ViewData["TopMenuPages"] = tm.GetTopMenusPages();


            if (!sessionHandler.IsUserLoggedIn())
                return RedirectToAction("Index", "Login");
            else
                {
                if (!sessionHandler.HasPermissionToViewPage("home"))
                    {
                    return RedirectToAction("Index", "PageNotFound");
                    }
                else
                    return View();
                }
            }

        public IActionResult Auditee_Branch_Response()
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
                    return View();
                }
            }

        ///////////////////////////////////////////////////////////////////////////////////////////////////////////////


        public IActionResult Branch_Compliance()
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
                    return View();
                }
            }


        public IActionResult Zonal_Administration()
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
                    return View();
                }
            }



        public IActionResult Implementation_officer()
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
                    return View();
                }
            }

        public IActionResult Audit_Zone_Action()
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
