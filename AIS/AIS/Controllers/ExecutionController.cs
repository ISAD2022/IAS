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
    public class ExecutionController : Controller
    {
        private readonly ILogger<ExecutionController> _logger;
        private readonly TopMenus tm;
        private readonly SessionHandler sessionHandler;
        private readonly DBConnection dBConnection;
        public ExecutionController(ILogger<ExecutionController> logger, SessionHandler _sessionHandler, DBConnection _dbCon, TopMenus _tpMenu)
        {
            _logger = logger;
            sessionHandler = _sessionHandler;
            dBConnection = _dbCon;
            tm = _tpMenu;
        }
        public IActionResult Index()
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

        public IActionResult AnnexureB()
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
        public IActionResult AnnexureB_2()
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
        public IActionResult AnnexureB_3()
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

        public IActionResult AnnexureB_4()
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
        public IActionResult AnnexureB_5()
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
        public IActionResult AnnexureB_6()
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

        public IActionResult AnnexureB_7()
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
        public IActionResult AnnexureB_8()
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
        public IActionResult engagement_plan()
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


        public IActionResult staff_pos_info()
        {
            ViewData["TopMenu"] = tm.GetTopMenus();
            ViewData["TopMenuPages"] = tm.GetTopMenusPages();
            ViewData["staffpos"] = dBConnection.GetStaffPosition();

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

      
        public IActionResult loan_case_document()
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

        public IActionResult audit_observation()
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
                    return View();
            }
        }
        public IActionResult cau_observation()
        {
            ViewData["TopMenu"] = tm.GetTopMenus();
            ViewData["TopMenuPages"] = tm.GetTopMenusPages();
            ViewData["RISK_GROUPS"] = dBConnection.GetRiskGroup();
            ViewData["ZonesList"] = dBConnection.GetZones(false);
            ViewData["ProcessList"] = dBConnection.GetAuditChecklistCAD();
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
        public IActionResult manage_observations(int engId= 0)
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
                    return View();
            }
        }
        public IActionResult manage_observations_branches(int engId = 0)
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
                    return View();
            }
        }
        public IActionResult manage_draft_report_paras(int engId = 0)
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
                    return View();
            }
        }

        public IActionResult manage_draft_report_paras_branch(int engId = 0)
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
                    return View();
            }
        }
        public IActionResult closing(int engId = 0)
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
        public IActionResult Sub_voilation_audit_observation()
        {
            ViewData["TopMenu"] = tm.GetTopMenus();
            ViewData["TopMenuPages"] = tm.GetTopMenusPages();
            ViewData["RISK_GROUPS"] = dBConnection.GetRiskGroup();
            ViewData["Voilation_Cat"] = dBConnection.GetAuditVoilationcats();
            ViewData["Voilation_Sub_Cat"] = dBConnection.GetVoilationSubGroup(1);

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
        [HttpPost]
        public List<RiskSubGroupModel> risk_sub_group(RiskGroupModel rgm)
        {
            return dBConnection.GetRiskSubGroup(rgm.GR_ID);
        }
        [HttpPost]
        public List<RiskActivityModel> risk_activities(RiskSubGroupModel rsg)
        {
            return dBConnection.GetRiskActivities(rsg.S_GR_ID);
        }

        [HttpPost]
        public List<AuditSubVoilationcatModel> sub_voilation(AuditSubVoilationcatModel vsg)
        {
            return dBConnection.GetVoilationSubGroup(vsg.V_ID);
        }

        [HttpPost]
        public List<AuditObservationTemplateModel> audit_observation_template(RiskActivityModel ra)
        {
            return dBConnection.GetAuditObservationTemplates(ra.ACTIVITY_ID);
        }

        public IActionResult checklist()
        {
            ViewData["TopMenu"] = tm.GetTopMenus();
            ViewData["TopMenuPages"] = tm.GetTopMenusPages();
            ViewData["Checklist"] = dBConnection.GetAuditChecklist();
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
        public IActionResult subchecklist()
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
        public IActionResult checklist_details()
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
        public IActionResult audit_execution()
        {
            ViewData["TopMenu"] = tm.GetTopMenus();
            ViewData["TopMenuPages"] = tm.GetTopMenusPages();
            ViewData["DivisionList"] = dBConnection.GetDivisions(false);
            ViewData["ProcessList"] = dBConnection.GetRiskProcessDefinition();
            ViewData["Voilation_Cat"] = dBConnection.GetAuditVoilationcats();
            ViewData["RiskList"]= dBConnection.GetRisks();
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
        public IActionResult pre_audit_info()
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
        public IActionResult pre_audit_info_detail()
        {
            ViewData["TopMenu"] = tm.GetTopMenus();
            ViewData["TopMenuPages"] = tm.GetTopMenusPages();
            ViewData["DivisionList"] = dBConnection.GetDivisions(false);
            ViewData["ProcessList"] = dBConnection.GetRiskProcessDefinition();
            ViewData["Voilation_Cat"] = dBConnection.GetAuditVoilationcats();
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
        public IActionResult loan_case_details()
        {
            ViewData["TopMenu"] = tm.GetTopMenus();
            ViewData["TopMenuPages"] = tm.GetTopMenusPages();
            ViewData["DivisionList"] = dBConnection.GetDivisions(false);
            ViewData["ProcessList"] = dBConnection.GetRiskProcessDefinition();
            ViewData["Voilation_Cat"] = dBConnection.GetAuditVoilationcats();
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
        public IActionResult branch_deposit_info()
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
        public IActionResult disb_info()
        {
            ViewData["TopMenu"] = tm.GetTopMenus();
            ViewData["TopMenuPages"] = tm.GetTopMenusPages();
            ViewData["DivisionList"] = dBConnection.GetDivisions(false);
            ViewData["ProcessList"] = dBConnection.GetRiskProcessDefinition();
            ViewData["Voilation_Cat"] = dBConnection.GetAuditVoilationcats();
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

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
