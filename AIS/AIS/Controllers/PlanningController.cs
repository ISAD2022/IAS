using AIS.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
namespace AIS.Controllers
    {

    public class PlanningController : Controller
        {
        private readonly ILogger<PlanningController> _logger;
        private readonly TopMenus tm;
        private readonly SessionHandler sessionHandler;
        private readonly DBConnection dBConnection;
        public PlanningController(ILogger<PlanningController> logger, SessionHandler _sessionHandler, DBConnection _dbCon, TopMenus _tpMenu)
            {
            _logger = logger;
            sessionHandler = _sessionHandler;
            dBConnection = _dbCon;
            tm = _tpMenu;
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
            ViewData["PendingCriteriaList"] = dBConnection.GetPendingAuditCriterias();
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
        public IActionResult special_audit_criteria()
            {
            ViewData["TopMenu"] = tm.GetTopMenus();
            ViewData["TopMenuPages"] = tm.GetTopMenusPages();
            ViewData["AuditNatureList"] = dBConnection.GetAuditNatureForAddLegacyPara();
            ViewData["AuditPeriodList"] = dBConnection.GetAuditPeriods();
            ViewData["ReportingOfficeList"] = dBConnection.Getparentrepoffice(5);

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
        public IActionResult special_audit_criteria_approval()
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
        public IActionResult audit_period()
            {
            ViewData["TopMenu"] = tm.GetTopMenus();
            ViewData["TopMenuPages"] = tm.GetTopMenusPages();
            bool sessionCheck = true;
            var loggedInUser = sessionHandler.GetSessionUser();
            if (loggedInUser.UserRoleID == 1)
                sessionCheck = false;
            ViewData["AuditDepartments"] = dBConnection.GetDepartments(354, sessionCheck);
            ViewData["AuditPeriodStatus"] = dBConnection.GetAuditPeriodStatus();
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
        [HttpGet]
        public IActionResult audit_plan(int dept_code, int periodId)
            {
            ViewData["TopMenu"] = tm.GetTopMenus();
            ViewData["TopMenuPages"] = tm.GetTopMenusPages();
            ViewData["AuditTeams"] = dBConnection.GetAuditTeams(dept_code);
            ViewData["AuditPlan"] = dBConnection.GetAuditPlan(periodId);
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
        public IActionResult holiday_calendar()
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
        public IActionResult post_changes_criteria()
            {
            ViewData["TopMenu"] = tm.GetTopMenus();
            ViewData["TopMenuPages"] = tm.GetTopMenusPages();
            ViewData["PostChangesAuditCriteriaList"] = dBConnection.GetPostChangesAuditCriterias();
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
        public IActionResult post_changes_approved_plan()
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
        public IActionResult post_changes_team_members()
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
        public IActionResult submission_for_approval()
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
        public IActionResult submission_for_review()
            {
            ViewData["TopMenu"] = tm.GetTopMenus();
            ViewData["TopMenuPages"] = tm.GetTopMenusPages();
            ViewData["TentativePlansList"] = dBConnection.GetTentativePlansForFields();
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
        public IActionResult staff_position()
            {
            ViewData["TopMenu"] = tm.GetTopMenus();
            ViewData["TopMenuPages"] = tm.GetTopMenusPages();
            bool sessionCheck = true;
            var loggedInUser = sessionHandler.GetSessionUser();
            if (loggedInUser.UserRoleID == 1)
                sessionCheck = false;
            ViewData["AuditDepartments"] = dBConnection.GetDepartments(354, sessionCheck);
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
        public IActionResult team_members()
            {
            ViewData["TopMenu"] = tm.GetTopMenus();
            ViewData["TopMenuPages"] = tm.GetTopMenusPages();
            ViewData["AuditDepartments"] = dBConnection.GetDepartments(354);
            var loggedInUser = sessionHandler.GetSessionUser();
            ViewData["AuditEmployees"] = dBConnection.GetAuditEmployees((int)loggedInUser.UserEntityID);

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
        public IActionResult tentative_audit_plan_ho_units()
            {
            ViewData["TopMenu"] = tm.GetTopMenus();
            ViewData["TopMenuPages"] = tm.GetTopMenusPages();
            ViewData["AuditDepartments"] = dBConnection.GetDepartments(354);
            var loggedInUser = sessionHandler.GetSessionUser();
            if (loggedInUser.UserPostingAuditZone != null && loggedInUser.UserPostingAuditZone != 0)
                ViewData["AuditEmployees"] = dBConnection.GetAuditEmployees((int)loggedInUser.UserPostingAuditZone);
            else if (loggedInUser.UserPostingBranch != null && loggedInUser.UserPostingBranch != 0)
                ViewData["AuditEmployees"] = dBConnection.GetAuditEmployees((int)loggedInUser.UserPostingBranch);
            else if (loggedInUser.UserPostingDept != null && loggedInUser.UserPostingDept != 0)
                ViewData["AuditEmployees"] = dBConnection.GetAuditEmployees((int)loggedInUser.UserPostingDept);
            else if (loggedInUser.UserPostingDiv != null && loggedInUser.UserPostingDiv != 0)
                ViewData["AuditEmployees"] = dBConnection.GetAuditEmployees((int)loggedInUser.UserPostingDiv);
            else if (loggedInUser.UserPostingZone != null && loggedInUser.UserPostingZone != 0)
                ViewData["AuditEmployees"] = dBConnection.GetAuditEmployees((int)loggedInUser.UserPostingZone);


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
        public List<AuditEmployeeModel> audit_employees(int dept_code = 0)
            {
            return dBConnection.GetAuditEmployees(dept_code);
            }
        [HttpPost]
        public bool referredBack_auditCriteria(List<CriteriaIDComment> DATALIST)
            {
            if (DATALIST.Count > 0)
                {
                foreach (var criteria in DATALIST)
                    {
                    dBConnection.SetAuditCriteriaStatusReferredBack(criteria.ID, criteria.COMMENT);
                    }
                }
            return true;
            }
        [HttpPost]
        public bool authorize_auditCriteria(List<CriteriaIDComment> DATALIST)
            {
            if (DATALIST.Count > 0)
                {
                foreach (var criteria in DATALIST)
                    {
                    dBConnection.SetAuditCriteriaStatusApprove(criteria.ID, criteria.COMMENT);
                    }
                }
            return true;
            }

        [HttpPost]
        public bool update_audit_criteria(List<String> CRITERIA_LIST)
            {

            AddAuditCriteriaModel cm = new AddAuditCriteriaModel();
            cm.ID = Convert.ToInt32(CRITERIA_LIST[0]);
            cm.AUDITPERIODID = Convert.ToInt32(CRITERIA_LIST[1]);
            cm.ENTITY_TYPEID = Convert.ToInt32(CRITERIA_LIST[2]);
            cm.RISK_ID = Convert.ToInt32(CRITERIA_LIST[3]);
            cm.FREQUENCY_ID = Convert.ToInt32(CRITERIA_LIST[4]);
            cm.SIZE_ID = Convert.ToInt32(CRITERIA_LIST[5]);
            cm.NO_OF_DAYS = Convert.ToInt32(CRITERIA_LIST[6]);
            if ((CRITERIA_LIST[7].ToLower()) == "y")
                CRITERIA_LIST[7] = "Y";
            else
                CRITERIA_LIST[7] = "N";

            cm.VISIT = CRITERIA_LIST[7];
            cm.APPROVAL_STATUS = 3;
            dBConnection.UpdateAuditCriteria(cm, CRITERIA_LIST[8]);

            return true;
            }

        [HttpPost]
        public bool post_changes_audit_criteria(List<String> CRITERIA_LIST)
            {

            AddAuditCriteriaModel cm = new AddAuditCriteriaModel();
            cm.ID = Convert.ToInt32(CRITERIA_LIST[0]);
            cm.AUDITPERIODID = Convert.ToInt32(CRITERIA_LIST[1]);
            cm.ENTITY_TYPEID = Convert.ToInt32(CRITERIA_LIST[2]);
            cm.RISK_ID = Convert.ToInt32(CRITERIA_LIST[3]);
            cm.FREQUENCY_ID = Convert.ToInt32(CRITERIA_LIST[4]);
            cm.SIZE_ID = Convert.ToInt32(CRITERIA_LIST[5]);
            cm.NO_OF_DAYS = Convert.ToInt32(CRITERIA_LIST[6]);
            if ((CRITERIA_LIST[7].ToLower()) == "y")
                CRITERIA_LIST[7] = "Y";
            else
                CRITERIA_LIST[7] = "N";

            cm.VISIT = CRITERIA_LIST[7];
            cm.APPROVAL_STATUS = 6;
            dBConnection.UpdateAuditCriteria(cm, CRITERIA_LIST[8]);

            return true;
            }
        [HttpPost]
        public List<AuditTeamModel> audit_team(int dept_code)
            {
            return dBConnection.GetAuditTeams(dept_code);
            }



        [HttpPost]
        public string generate_plan_audit_criteria(int CRITERIA_ID)
            {

            return "{\"Message\": \"" + dBConnection.GeneratePlanForAuditCriteria(CRITERIA_ID) + "\"}";


            }

        [HttpPost]
        public AuditPlanModel add_audit_plan(AuditPlanModel plan)
            {
            return plan;
            }
        public IActionResult tentative_audit_plan()
            {
            ViewData["TopMenu"] = tm.GetTopMenus();
            ViewData["TopMenuPages"] = tm.GetTopMenusPages();
            ViewData["AuditDepartments"] = dBConnection.GetDepartments(354);
            ViewData["DivisionsList"] = dBConnection.GetDivisions(false);
            ViewData["AuditZonesList"] = dBConnection.GetZones();
            List<TentativePlanModel> pl = new List<TentativePlanModel>();
            pl = dBConnection.GetTentativePlansForFields();
            ViewData["TotalPlanEntities"] = pl.Count;
            ViewData["TentativePlansList"] = pl;
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
        public IActionResult tentative_engagement_plan()
            {
            ViewData["TopMenu"] = tm.GetTopMenus();
            ViewData["TopMenuPages"] = tm.GetTopMenusPages();
            ViewData["AuditDepartments"] = dBConnection.GetDepartments(354);
            ViewData["DivisionsList"] = dBConnection.GetDivisions(false);
            ViewData["AuditZonesList"] = dBConnection.GetZones();
            ViewData["AuditTeamsList"] = dBConnection.GetAuditTeams();
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
        public List<BranchModel> zone_branches(int zone_code, bool session_check = true)
            {
            return dBConnection.GetBranches(zone_code, session_check);
            }
        [HttpPost]
        public List<DepartmentModel> div_departments(int div_code)
            {
            return dBConnection.GetDepartments(div_code, false);
            }
        [HttpPost]
        public List<AuditTeamModel> audit_teams(int dept_code)
            {
            return dBConnection.GetAuditTeams(dept_code);
            }
        [HttpPost]
        public string get_operational_start_date(int periodId, int entityCode)
            {
            return dBConnection.GetAuditOperationalStartDate(periodId, entityCode);
            }
        [HttpPost]
        public AuditEngagementPlanModel add_engagement_plan(AuditEngagementPlanModel eng)
            {
            return dBConnection.AddAuditEngagementPlan(eng);
            }
        [HttpPost]
        public string add_audit_team(List<AddAuditTeamModel> AUDIT_TEAM)
            {
            string resp = "";
            List<AuditTeamModel> aTeams = new List<AuditTeamModel>();
            int newTeamId = dBConnection.GetLatestTeamID();
            if (AUDIT_TEAM.Count > 0 && AUDIT_TEAM != null)
                {
                foreach (var item in AUDIT_TEAM)
                    {
                    AuditTeamModel ateam = new AuditTeamModel();
                    ateam.T_ID = newTeamId;
                    ateam.CODE = (newTeamId).ToString();
                    ateam.NAME = item.T_NAME;
                    ateam.EMPLOYEENAME = item.NAME;
                    ateam.TEAMMEMBER_ID = item.PPNO;
                    ateam.IS_TEAMLEAD = item.ISTEAMLEAD;
                    ateam.PLACE_OF_POSTING = item.PLACEOFPOSTING;
                    ateam.STATUS = "Y";
                    resp += dBConnection.AddAuditTeam(ateam);

                    }
                }
            return "{\"Status\":true,\"Message\":\"" + resp + "\"}";
            }
        [HttpPost]
        public bool delete_audit_team(string T_CODE)
            {
            return dBConnection.DeleteAuditTeam(T_CODE);

            }
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
            {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
            }
        }
    }
