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
    public class SetupController : Controller
    {
        private readonly ILogger<SetupController> _logger;
        private readonly TopMenus tm = new TopMenus();
        private readonly DBConnection dBConnection = new DBConnection();
        private readonly SessionHandler sessionHandler = new SessionHandler();

        public SetupController(ILogger<SetupController> logger)
        {
            _logger = logger;
        }

        public IActionResult branches()
        {
            
            ViewData["TopMenu"] = tm.GetTopMenus();
            ViewData["TopMenuPages"] = tm.GetTopMenusPages();
            ViewData["BranchList"] = dBConnection.GetBranches();
            ViewData["ZoneList"] = dBConnection.GetZones();
            ViewData["BranchSizeList"] = dBConnection.GetBranchSizes();
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
        public IActionResult manage_audit_zone_branches()
        {

            ViewData["TopMenu"] = tm.GetTopMenus();
            ViewData["TopMenuPages"] = tm.GetTopMenusPages();
            ViewData["BranchList"] = dBConnection.GetBranches();
            ViewData["ZoneList"] = dBConnection.GetAuditZones();
            ViewData["BranchSizeList"] = dBConnection.GetBranchSizes();
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
        public IActionResult manage_inspection_unit_branches()
        {

            ViewData["TopMenu"] = tm.GetTopMenus();
            ViewData["TopMenuPages"] = tm.GetTopMenusPages();
            ViewData["BranchList"] = dBConnection.GetBranches();
            ViewData["ZoneList"] = dBConnection.GetZones();
            ViewData["BranchSizeList"] = dBConnection.GetBranchSizes();
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
        public BranchModel branch_add(BranchModel br)
        {
            if (br.ISACTIVE == "Active")
                br.ISACTIVE = "Y";
            else if (br.ISACTIVE == "InActive")
                br.ISACTIVE = "N";

            if (br.BRANCHID == 0)
                br = dBConnection.AddBranch(br);
            else
                br = dBConnection.UpdateBranch(br);
            return br;
        }

        public IActionResult divisions()
        {

            ViewData["TopMenu"] = tm.GetTopMenus();
            ViewData["TopMenuPages"] = tm.GetTopMenusPages();
            ViewData["DivisionList"] = dBConnection.GetDivisions(false);
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
        public DivisionModel division_add(DivisionModel div)
        {
            if (div.ISACTIVE == "Active")
                div.ISACTIVE = "Y";
            else if (div.ISACTIVE == "InActive")
                div.ISACTIVE = "N";
           
            if (div.DIVISIONID == 0)
                div=dBConnection.AddDivision(div);
            else
                div=dBConnection.UpdateDivision(div);
            return div;
        }
        public IActionResult departments()
        {

            ViewData["TopMenu"] = tm.GetTopMenus();
            ViewData["TopMenuPages"] = tm.GetTopMenusPages();
            ViewData["DivisionList"] = dBConnection.GetDivisions(false);
            ViewData["DepartmentList"] = dBConnection.GetDepartments(0,false);
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
        public DepartmentModel department_add(DepartmentModel dept)
        {
            if (dept.STATUS == "Active")
                dept.STATUS = "A";
            else if (dept.STATUS == "InActive")
                dept.STATUS = "I";

            if (dept.ID == 0)
                dept = dBConnection.AddDepartment(dept);
            else
                dept = dBConnection.UpdateDepartment(dept);
            return dept;
        }
        public IActionResult audit_zones()
        {
            ViewData["TopMenu"] = tm.GetTopMenus();
            ViewData["TopMenuPages"] = tm.GetTopMenusPages();
            ViewData["AuditZoneList"] = dBConnection.GetAuditZones();
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

        public IActionResult Inspection_Unit()
        {
            ViewData["TopMenu"] = tm.GetTopMenus();
            ViewData["TopMenuPages"] = tm.GetTopMenusPages();
            ViewData["ICList"] = dBConnection.GetInspectionUnits();
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
        public IActionResult control_violation()
        {
            ViewData["TopMenu"] = tm.GetTopMenus();
            ViewData["TopMenuPages"] = tm.GetTopMenusPages();
            ViewData["ControlViolationList"] = dBConnection.GetControlViolations();
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
        public ControlViolationsModel add_control_violation(ControlViolationsModel cv)
        {
            return dBConnection.AddControlViolation(cv);
        }
        public IActionResult processes()
        {
            ViewData["TopMenu"] = tm.GetTopMenus();
            ViewData["TopMenuPages"] = tm.GetTopMenusPages();
            ViewData["DivisionList"] = dBConnection.GetDivisions(false);
            ViewData["ProcessList"] = dBConnection.GetRiskProcessDefinition();
            ViewData["AuditableEntityTypes"] = dBConnection.GetAuditEntities();
            ViewData["ControlViolationsList"] = dBConnection.GetControlViolations();
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
        public IActionResult process_review()
        {
            ViewData["TopMenu"] = tm.GetTopMenus();
            ViewData["TopMenuPages"] = tm.GetTopMenusPages();
            int[] statusIds = { 1, 4 };
            List<RiskProcessTransactions> trList = dBConnection.GetRiskProcessTransactionsWithStatus(statusIds);
            foreach(var item in trList)
            {
                RiskProcessTransactions pt = new RiskProcessTransactions();
                pt = dBConnection.GetRiskProcessTransactionLastStatus(item);
                item.PROCESS_COMMENTS = pt.PROCESS_COMMENTS;
            }
            ViewData["TransactionsList"] = trList;
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
        public IActionResult process_authorize()
        {
            ViewData["TopMenu"] = tm.GetTopMenus();
            ViewData["TopMenuPages"] = tm.GetTopMenusPages();
            int[] statusIds = {3};
            List<RiskProcessTransactions> trList = dBConnection.GetRiskProcessTransactionsWithStatus(statusIds);
            foreach (var item in trList)
            {
                RiskProcessTransactions pt = new RiskProcessTransactions();
                pt = dBConnection.GetRiskProcessTransactionLastStatus(item);
                item.PROCESS_COMMENTS = pt.PROCESS_COMMENTS;
            }
            ViewData["TransactionsList"] = trList;
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
        public IActionResult sub_entities()
        {
            ViewData["TopMenu"] = tm.GetTopMenus();
            ViewData["TopMenuPages"] = tm.GetTopMenusPages();
            ViewData["SubEntitiesList"] = dBConnection.GetSubEntities();
            ViewData["DivisionList"] = dBConnection.GetDivisions(false);
            ViewData["DepartmentList"] = dBConnection.GetDepartments(0,false);
            return View();
        }
        [HttpPost]
        public List<DepartmentModel> get_departments(int div_id)
        {
            return dBConnection.GetDepartments(div_id,false);
        }
        [HttpPost]
        public List<SubEntitiesModel> get_sub_entities(int div_id=0,int dept_id=0)
        {
            return dBConnection.GetSubEntities(div_id,dept_id);
        }
        [HttpPost]
        public SubEntitiesModel add_sub_entity(SubEntitiesModel entity)
        {
            if (entity.STATUS == "Active")
                entity.STATUS = "Y";
            else
                entity.STATUS = "N";
            if(entity.ID==0)
                return dBConnection.AddSubEntity(entity);
            else
                return dBConnection.UpdateSubEntity(entity);
        }
        [HttpPost]
        public List<RiskProcessDetails> process_details(int ProcessId)
        {
            return dBConnection.GetRiskProcessDetails(ProcessId);
        }
        [HttpPost]
        public List<RiskProcessTransactions> process_transactions(int ProcessDetailId=0, int transactionId = 0)
        {
            return dBConnection.GetRiskProcessTransactions(ProcessDetailId, transactionId);
        }
        [HttpPost]
        public RiskProcessDefinition process_add(RiskProcessDefinition proc)
        {
            return dBConnection.AddRiskProcess(proc);            
        }
        [HttpPost]
        public RiskProcessDetails sub_process_add(RiskProcessDetails subProc)
        {
            return dBConnection.AddRiskSubProcess(subProc);
        }
        [HttpPost]
        public RiskProcessTransactions sub_process_transaction_add(RiskProcessTransactions tran)
        {
            return dBConnection.AddRiskSubProcessTransaction(tran);
        }
        [HttpPost]
        public bool recommend_process_transaction_by_reviewer(int T_ID, string COMMENTS)
        {
            return dBConnection.RecommendProcessTransactionByReviewer(T_ID,COMMENTS);
        }
        public bool reffered_back_process_transaction_by_reviewer(int T_ID, string COMMENTS)
        {
            return dBConnection.RefferedBackProcessTransactionByReviewer(T_ID, COMMENTS);
        }
        [HttpPost]
        public bool recommend_process_transaction_by_authorizer(int T_ID, string COMMENTS)
        {
            return dBConnection.RecommendProcessTransactionByAuthorizer(T_ID, COMMENTS);
        }
        public bool reffered_back_process_transaction_by_authorizer(int T_ID, string COMMENTS)
        {
            return dBConnection.RefferedBackProcessTransactionByAuthorizer(T_ID, COMMENTS);
        }
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
