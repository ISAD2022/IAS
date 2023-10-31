﻿using AIS.Models;
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

        public IActionResult Settled_Para()
        {
            ViewData["TopMenu"] = tm.GetTopMenus();
            ViewData["TopMenuPages"] = tm.GetTopMenusPages();
            ViewData["EntitiesList"] = dBConnection.GetLegacyParasEntities();

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

        public IActionResult search_checklistdetails()
        {
            ViewData["TopMenu"] = tm.GetTopMenus();
            ViewData["TopMenuPages"] = tm.GetTopMenusPages();
            ViewData["EntitiesList"] = dBConnection.SearchChecklistDetails();

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

        public IActionResult Authorize_Change_Settle_Para_Status()
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
        public IActionResult Authorize_Update_Legacy_Para_Gist_Para()
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
        public IActionResult auditee_observations_report()
        {
            ViewData["TopMenu"] = tm.GetTopMenus();
            ViewData["TopMenuPages"] = tm.GetTopMenusPages();
            ViewData["EntitiesList"] = dBConnection.GetObservationEntities();

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
        public IActionResult auditee_observations_report_cau()
        {
            ViewData["TopMenu"] = tm.GetTopMenus();
            ViewData["TopMenuPages"] = tm.GetTopMenusPages();
            ViewData["EntitiesList"] = dBConnection.GetObservationEntities();

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

        public IActionResult auditee_position_outlook()
        {
            ViewData["TopMenu"] = tm.GetTopMenus();
            ViewData["TopMenuPages"] = tm.GetTopMenusPages();
            ViewData["EntitiesList"] = dBConnection.GetObservationEntities();

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
        public IActionResult manage_legacy_paras()
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

     
        public IActionResult cau_observation()
        {
            ViewData["TopMenu"] = tm.GetTopMenus();
            ViewData["TopMenuPages"] = tm.GetTopMenusPages();
            ViewData["RISK_GROUPS"] = dBConnection.GetRiskGroup();
            ViewData["ZonesList"] = dBConnection.GetZones(false);
            ViewData["AnnexList"] = dBConnection.GetAnnexuresForChecklistDetail();
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
            ViewData["EntitiesList"] = dBConnection.GetObservationEntitiesForManageObservations();
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
            ViewData["EntitiesList"] = dBConnection.GetObservationEntitiesForManageObservations();
            ViewData["ProcessList"] = dBConnection.GetAuditChecklistCAD();
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
            ViewData["EntitiesList"] = dBConnection.GetObservationEntitiesForManageObservations();
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
        public IActionResult checklist_summary(int engId = 0)
        {
            ViewData["TopMenu"] = tm.GetTopMenus();
            ViewData["TopMenuPages"] = tm.GetTopMenusPages();
            ViewData["EntitiesList"] = dBConnection.GetObservationEntitiesForManageObservations();
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
            ViewData["EntitiesList"] = dBConnection.GetObservationEntitiesForManageObservations();

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
            ViewData["AnnexList"] = dBConnection.GetAnnexuresForChecklistDetail();
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

        public IActionResult deposit_account()
        {
            ViewData["TopMenu"] = tm.GetTopMenus();
            ViewData["TopMenuPages"] = tm.GetTopMenusPages();
            ViewData["Depositcat"] = dBConnection.GetDepositCat();
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

        public IActionResult loan_scheme()
        {
            ViewData["TopMenu"] = tm.GetTopMenus();
            ViewData["TopMenuPages"] = tm.GetTopMenusPages();
            ViewData["GetLoanScheme"] = dBConnection.GetLoansScheme();

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

        public IActionResult loan_scheme_yearly()
        {
            ViewData["TopMenu"] = tm.GetTopMenus();
            ViewData["TopMenuPages"] = tm.GetTopMenusPages();
            ViewData["GetLoanScheme"] = dBConnection.GetLoansSchemeYearly();

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



        public IActionResult Concluding_Closing_Audit()
        {
            ViewData["TopMenu"] = tm.GetTopMenus();
            ViewData["TopMenuPages"] = tm.GetTopMenusPages();
            ViewData["EntitiesList"] = dBConnection.GetAuditConcludingEntities();

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
        public IActionResult Add_Legacy_Para()
        {
            ViewData["TopMenu"] = tm.GetTopMenus();
            ViewData["TopMenuPages"] = tm.GetTopMenusPages();
            ViewData["EntitiesTypeList"] = dBConnection.GetAuditeeEntitiesType();
            ViewData["AuditYearList"] = dBConnection.GetAuditYearForAddLegacyPara();
            ViewData["AuditNatureList"] = dBConnection.GetAuditNatureForAddLegacyPara();
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

        public IActionResult Authorize_Adding_Legacy_Para()
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
        public IActionResult Update_Gist_Para_No()
        {
            ViewData["TopMenu"] = tm.GetTopMenus();
            ViewData["TopMenuPages"] = tm.GetTopMenusPages();
            ViewData["EntitiesList"] = dBConnection.GetLegacyParasEntities();
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
        public IActionResult Update_Legacy_Paras()
        {
            ViewData["TopMenu"] = tm.GetTopMenus();
            ViewData["TopMenuPages"] = tm.GetTopMenusPages();
            ViewData["EntitiesList"] = dBConnection.GetLegacyParasEntities();
            ViewData["ProcessList"] = dBConnection.GetAuditChecklistCAD();
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
        public IActionResult Update_Legacy_Paras_HO()
        {
            ViewData["TopMenu"] = tm.GetTopMenus();
            ViewData["TopMenuPages"] = tm.GetTopMenusPages();
            ViewData["EntitiesList"] = dBConnection.GetLegacyParasEntitiesHO();
            ViewData["ProcessList"] = dBConnection.GetAuditChecklistCAD();
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
        public IActionResult Update_Legacy_Paras_FAD()
        {
            ViewData["TopMenu"] = tm.GetTopMenus();
            ViewData["TopMenuPages"] = tm.GetTopMenusPages();
            ViewData["EntitiesList"] = dBConnection.GetLegacyParasEntitiesFAD();
            ViewData["ProcessList"] = dBConnection.GetAuditChecklistCAD();
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

        public IActionResult pre_concluding_audit()
        {
            ViewData["TopMenu"] = tm.GetTopMenus();
            ViewData["TopMenuPages"] = tm.GetTopMenusPages();
            ViewData["EntitiesList"] = dBConnection.GetObservationEntitiesForPreConcluding();
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

        public IActionResult deposit_account_details()
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

        public IActionResult draft_audit_report()
        {
            ViewData["TopMenu"] = tm.GetTopMenus();
            ViewData["TopMenuPages"] = tm.GetTopMenusPages();
            //ViewData["ObservationsList"] = dBConnection.GetManagedObservations();
            ViewData["EntitiesList"] = dBConnection.GetObservationEntities();
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
        public IActionResult draft_audit_report_branch()
        {
            ViewData["TopMenu"] = tm.GetTopMenus();
            ViewData["TopMenuPages"] = tm.GetTopMenusPages();
            //ViewData["ObservationsList"] = dBConnection.GetManagedObservations();
            ViewData["EntitiesList"] = dBConnection.GetObservationEntities();
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
