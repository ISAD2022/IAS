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
    public class ApiCallsController : Controller
    {
        private readonly ILogger<ApiCallsController> _logger;

        private readonly SessionHandler sessionHandler;
        private readonly DBConnection dBConnection;
        private readonly DBContext _context;
        public ApiCallsController(ILogger<ApiCallsController> logger, SessionHandler _sessionHandler, DBConnection _dbCon)
        {
            _logger = logger;
            sessionHandler = _sessionHandler;
            dBConnection = _dbCon;
        }
        [HttpPost]
        public bool kill_session(LoginModel user)
        {
           return dBConnection.KillExistSession(user);
           
        }
        [HttpPost]
        public bool terminate_idle_session()
        {
            dBConnection.TerminateIdleSession();
            return true;
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
        
        [HttpPost]
        public ControlViolationsModel add_control_violation(ControlViolationsModel cv)
        {
            return dBConnection.AddControlViolation(cv);
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
        [HttpPost]
        public bool reffered_back_process_transaction_by_authorizer(int T_ID, string COMMENTS)
        {
            return dBConnection.RefferedBackProcessTransactionByAuthorizer(T_ID, COMMENTS);
        }

        [HttpPost]
        public List<AuditChecklistSubModel> sub_checklist(int T_ID, int ENG_ID)
        {
            return dBConnection.GetAuditChecklistSub(T_ID,ENG_ID);
        }
        [HttpPost]
        public List<AuditChecklistDetailsModel> checklist_details(int S_ID)
        {
            return dBConnection.GetAuditChecklistDetails(S_ID);
        }
        [HttpPost]
        public string save_observations(List<ListObservationModel> LIST_OBS, int ENG_ID, int S_ID, int V_CAT_ID=0, int V_CAT_NATURE_ID=0, int RISK_ID=0 )
        {
            int success = 0;
            int failed = 0;
            foreach (ListObservationModel m in LIST_OBS)
            {
                ObservationModel ob = new ObservationModel();
                ob.SUBCHECKLIST_ID = S_ID;
                ob.CHECKLISTDETAIL_ID =Convert.ToInt32(m.ID.Split("obs_")[1]);
                ob.V_CAT_ID = V_CAT_ID;
                ob.V_CAT_NATURE_ID = V_CAT_NATURE_ID;
                ob.ENGPLANID = ENG_ID;
                ob.REPLYDATE = DateTime.Today.AddDays(m.DAYS);
                ob.OBSERVATION_TEXT = m.MEMO;
                ob.SEVERITY = RISK_ID;
                ob.STATUS = 1;
                if (dBConnection.SaveAuditObservation(ob))
                    success++;
                else
                    failed++;
            }
            return "{\"success\":"+success+" , \"failed\":"+failed+"}";
        }
        [HttpPost]
        public bool reply_observation(ObservationResponseModel or)
        {
            return dBConnection.ResponseAuditObservation(or);            
        }
        [HttpPost]
        public bool update_observation_status(int OBS_ID, int NEW_STATUS_ID, int RISK_ID, string AUDITOR_COMMENT)
        {

            if (NEW_STATUS_ID == 4)
                if (RISK_ID != 3)
                    return false;

            if (dBConnection.UpdateAuditObservationStatus(OBS_ID, NEW_STATUS_ID,AUDITOR_COMMENT))
                return true;
            else
                return false;
        }
        [HttpPost]
        public bool drop_observation(int OBS_ID)
        {
            return dBConnection.DropAuditObservation(OBS_ID);
               
        }
        [HttpPost]
        public bool submit_observation_to_auditee(int OBS_ID)
        {
            return dBConnection.SubmitAuditObservationToAuditee(OBS_ID);
        }
        [HttpPost]
        public List<ManageObservations> get_observation(int OBS_ID=0)
        {
            return dBConnection.GetManagedObservations(0,OBS_ID);
        }
        [HttpPost]
        public List<ManageObservations> get_observation_branches(int OBS_ID = 0)
        {
            return dBConnection.GetManagedObservationsForBranches(0, OBS_ID);
        }

        [HttpPost]
        public List<ManageObservations> draft_report_summary(int ENG_ID)
        {
           return dBConnection.GetManagedObservations(ENG_ID);
        }
        [HttpPost]
        public List<ObservationModel> closing_draft_report_status(int ENG_ID = 0)
        {
            return dBConnection.GetClosingDraftObservations(ENG_ID);
        }
        [HttpPost]
        public List<ClosingDraftTeamDetailsModel> get_team_details(int ENG_ID=0)
        {
            return dBConnection.GetClosingDraftTeamDetails(ENG_ID);
        }
        [HttpPost]
        public bool close_draft_audit(int ENG_ID)
        {
            return dBConnection.CloseDraftAuditReport(ENG_ID);
        }
 		[HttpPost]
        public List<LoanCaseModel> Loan_Case_Details(int Loan_case, string LOAN_TYPE="")
        {
          return dBConnection.GetLoanCaseDetails(Loan_case, LOAN_TYPE);
        }
        [HttpPost]
        public GlHeadSubDetailsModel Glhead_Sub_Details(int GL_CODE)
        {
            return dBConnection.GetGlheadSubDetails(GL_CODE);
        }

       
        [HttpPost]
        public List<DepositAccountModel> GetDepositAccountSubdetails(string b_name)
        {
            return dBConnection.GetDepositAccountSubdetails(b_name);
        }

        [HttpPost]
        public List<LoanCaseModel> GetBranchDesbursementaccountdetails(int b_id)
        {
            return dBConnection.GetBranchDesbursementAccountdetails(b_id);
        }

        [HttpPost]
        public List<GlHeadDetailsModel> GetIncomeExpenceDetails(int b_id)
        {
            return dBConnection.GetIncomeExpenceDetails(b_id);
        }
      
        [HttpPost]
        public int GetAuditEntitiesCount(int RISK_ID, int SIZE_ID, int ENTITY_TYPE_ID, int PERIOD_ID, int FREQUENCY_ID)
        {
            return dBConnection.GetExpectedCountOfAuditEntitiesOnCriteria(RISK_ID,SIZE_ID,ENTITY_TYPE_ID, PERIOD_ID, FREQUENCY_ID);
        }
        [HttpPost]
        public bool DeletePendingCriteria(int RISK_ID, int SIZE_ID, int ENTITY_TYPE_ID, int PERIOD_ID, int FREQUENCY_ID)
        {
            return dBConnection.DeletePendingCriteria(RISK_ID, SIZE_ID, ENTITY_TYPE_ID, PERIOD_ID, FREQUENCY_ID);
        }
        [HttpPost]
        public bool submit_audit_criterias(int PERIOD_ID)
        {
            return dBConnection.SubmitAuditCriteriaForApproval(PERIOD_ID);
        }
        [HttpPost]
        public List<COSORiskModel> GetCOSORiskForDepartment(int PERIOD_ID=0)
        {
            return dBConnection.GetCOSORiskForDepartment(PERIOD_ID);
        }
        [HttpPost]
        public bool CAU_OM_assignment(CAUOMAssignmentModel caumodel)
        {
            return dBConnection.CAUOMAssignment(caumodel);
        }
        [HttpPost]
        public List<CAUOMAssignmentModel> CAU_Get_OMs()
        {
            return dBConnection.CAUGetAssignedOMs();
        }
        [HttpPost]
        public List<ManageObservations> get_observations(int ENG_ID)
        {
            return dBConnection.GetManagedObservations(ENG_ID,0);

        }
        [HttpPost]
        public List<ManageObservations> get_observations_draft(int ENG_ID)
        {
            return dBConnection.GetManagedDraftObservations(ENG_ID);

        }
        [HttpPost]
        public List<AssignedObservations> get_assigned_observation(int ENG_ID)
        {
            return dBConnection.GetAssignedObservations(ENG_ID);

        }
        [HttpPost]
        public List<AuditCCQModel> get_ccqs(int ENTITY_ID)
        {
            return dBConnection.GetCCQ(ENTITY_ID);

        }
        [HttpPost]
        public bool update_ccq(AuditCCQModel ccq)
        {
            return dBConnection.UpdateCCQ(ccq);

        }
        [HttpPost]
        public List<object> get_observation_text(int OBS_ID)
        {
            return dBConnection.GetObservationText(OBS_ID);
            
        }
        [HttpPost]
        public bool old_para_response(AuditeeOldParasResponseModel ob)
        {
            return dBConnection.AuditeeOldParaResponse(ob);
        }
        [HttpPost]
        public List<OldParasModel> get_legacy_para(string AUDITED_BY, string AUDIT_YEAR)
        {
            return dBConnection.GetOldParas(AUDITED_BY, AUDIT_YEAR);
        }
        [HttpPost]
        public List<OldParasModel> get_legacy_para_for_response()
        {
            return dBConnection.GetOldParasForResponse();
        }
        [HttpPost]
        public List<AuditeeOldParasModel> get_outstanding_para(string ENTITY_ID)
        {
            return dBConnection.GetOutstandingParas(ENTITY_ID);
        }
        [HttpPost]
        public bool add_legacy_para_observation_text(OldParasModel ob)
        {
            return dBConnection.AddOldParas(ob);
        }
        [HttpPost]
        public bool add_legacy_para_reply(int ID, string REPLY)
        {
            return dBConnection.AddOldParasReply(ID,REPLY);
        }
        [HttpPost]
        public ActiveInactiveChart get_pie_chart_data()
        {
            return dBConnection.GetActiveInactiveChartData();
        }

        [HttpPost]
        public List<UserRelationshipModel> getparentrel(int ENTITY_REALTION_ID)
        {
            return dBConnection.Getparentrepoffice(ENTITY_REALTION_ID);
        }

        [HttpPost]
        public List<UserRelationshipModel> getpostplace(int E_R_ID)
        {
            return dBConnection.Getchildposting(E_R_ID);
        }
        [HttpPost]
        public List<ManageObservations> get_violation_observations(int ENTITY_ID,int VIOLATION_ID)
        {
            return dBConnection.GetViolationObservations(ENTITY_ID,VIOLATION_ID);
        }

        [HttpPost]
        public bool approve_engagement_plan(int ENG_ID)
        {
            return dBConnection.ApproveAuditEngagementPlan(ENG_ID);
        }

        [HttpPost]
        public bool reject_engagement_plan(int ENG_ID, string COMMENTS)
        {
            return dBConnection.RefferedBackAuditEngagementPlan(ENG_ID, COMMENTS);
        }


        


  [HttpPost]
        public List<LoanCasedocModel> Getloancasedocuments()
        {
            return dBConnection.GetLoanCaseDocuments();
        }


        ///////////////////////////////////////////////////////////////////////////////////////////////////

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
