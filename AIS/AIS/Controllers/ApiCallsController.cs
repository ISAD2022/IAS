using AIS.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Reflection;
using Microsoft.CodeAnalysis;


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
        public List<AuditPlanEngagementModel> getauditplanengagement(int b_id)
        {
            return dBConnection.GetAuditPlanEngagement(b_id);
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
                div = dBConnection.AddDivision(div);
            else
                div = dBConnection.UpdateDivision(div);
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
            return dBConnection.GetDepartments(div_id, false);
        }
        [HttpPost]
        public List<SubEntitiesModel> get_sub_entities(int div_id = 0, int dept_id = 0)
        {
            return dBConnection.GetSubEntities(div_id, dept_id);
        }
        [HttpPost]
        public SubEntitiesModel add_sub_entity(SubEntitiesModel entity)
        {
            if (entity.STATUS == "Active")
                entity.STATUS = "Y";
            else
                entity.STATUS = "N";
            if (entity.ID == 0)
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
        public List<RiskProcessTransactions> process_transactions(int ProcessDetailId = 0, int transactionId = 0)
        {
            return dBConnection.GetRiskProcessTransactions(ProcessDetailId, transactionId);
        }
        [HttpPost]
        public List<ChecklistDetailComparisonModel> get_checklist_detail_comparison_by_Id(int CHECKLIST_DETAIL_ID = 0)
        {
            return dBConnection.GetChecklistComparisonDetailById(CHECKLIST_DETAIL_ID);
        }
        [HttpPost]
        public List<ChecklistDetailComparisonModel> get_checklist_detail_comparison_by_Id_for_referredBack(int CHECKLIST_DETAIL_ID = 0)
        {
            return dBConnection.GetChecklistComparisonDetailByIdForRefferedBack(CHECKLIST_DETAIL_ID);
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
        public bool authorize_sub_process_by_authorizer(int T_ID, string COMMENTS)
        {
            return dBConnection.AuthorizeSubProcessByAuthorizer(T_ID, COMMENTS);
        }
        [HttpPost]
        public bool reffered_back_sub_process_by_authorizer(int T_ID, string COMMENTS)
        {
            return dBConnection.RefferedBackSubProcessByAuthorizer(T_ID, COMMENTS);
        }

        [HttpPost]
        public bool recommend_process_transaction_by_reviewer(int T_ID, string COMMENTS)
        {
            return dBConnection.RecommendProcessTransactionByReviewer(T_ID, COMMENTS);
        }
        public bool reffered_back_process_transaction_by_reviewer(int T_ID, string COMMENTS)
        {
            return dBConnection.RefferedBackProcessTransactionByReviewer(T_ID, COMMENTS);
        }
        [HttpPost]
        public bool authorize_process_transaction_by_authorizer(int T_ID, string COMMENTS)
        {
            return dBConnection.AuthorizeProcessTransactionByAuthorizer(T_ID, COMMENTS);
        }
        [HttpPost]
        public bool reffered_back_process_transaction_by_authorizer(int T_ID, string COMMENTS)
        {
            return dBConnection.RefferedBackProcessTransactionByAuthorizer(T_ID, COMMENTS);
        }

        [HttpPost]
        public List<AuditChecklistSubModel> sub_checklist(int T_ID, int ENG_ID)
        {
            return dBConnection.GetAuditChecklistSub(T_ID, ENG_ID);
        }
        [HttpPost]
        public List<AuditChecklistDetailsModel> checklist_details(int S_ID)
        {
            return dBConnection.GetAuditChecklistDetails(S_ID);
        }
        [HttpPost]
        public string save_observations(List<ListObservationModel> LIST_OBS, int ENG_ID, int S_ID, int V_CAT_ID = 0, int V_CAT_NATURE_ID = 0, int RISK_ID = 0)
        {
            int success = 0;
            int failed = 0;
            foreach (ListObservationModel m in LIST_OBS)
            {
                ObservationModel ob = new ObservationModel();
                ob.HEADING = m.HEADING;
                ob.SUBCHECKLIST_ID = S_ID;
                ob.CHECKLISTDETAIL_ID = Convert.ToInt32(m.ID.Split("obs_")[1]);
                ob.V_CAT_ID = V_CAT_ID;
                ob.V_CAT_NATURE_ID = V_CAT_NATURE_ID;
                ob.ENGPLANID = ENG_ID;
                ob.REPLYDATE = DateTime.Today.AddDays(m.DAYS);
                ob.OBSERVATION_TEXT = m.MEMO;
                ob.SEVERITY = RISK_ID;
                ob.NO_OF_INSTANCES = m.NO_OF_INSTANCES;
                ob.RESPONSIBLE_PPNO = m.RESPONSIBLE_PPNO;
                ob.STATUS = 1;
                if (dBConnection.SaveAuditObservation(ob))
                    success++;
                else
                    failed++;
            }
            return "{\"success\":" + success + " , \"failed\":" + failed + "}";
        }
        [HttpPost]
        public string save_observations_cau(List<ListObservationModel> LIST_OBS, int ENG_ID = 0, int BRANCH_ID = 0, int SUB_CHECKLISTID = 0, int CHECKLIST_ID = 0)
        {
            int success = 0;
            int failed = 0;
            foreach (ListObservationModel m in LIST_OBS)
            {
                ObservationModel ob = new ObservationModel();
                ob.SUBCHECKLIST_ID = SUB_CHECKLISTID;
                ob.CHECKLISTDETAIL_ID = CHECKLIST_ID;

                ob.ENGPLANID = ENG_ID;
                ob.REPLYDATE = DateTime.Today.AddDays(m.DAYS);
                ob.OBSERVATION_TEXT = m.MEMO;
                ob.HEADING = m.HEADING;
                ob.SEVERITY = 1;
                ob.BRANCH_ID = BRANCH_ID;
                ob.RESPONSIBLE_PPNO = m.RESPONSIBLE_PPNO;
                ob.STATUS = 1;
                if (dBConnection.SaveAuditObservationCAU(ob))
                    success++;
                else
                    failed++;
            }
            return "{\"success\":" + success + " , \"failed\":" + failed + "}";
        }


        [HttpPost]
        public bool reply_observation(ObservationResponseModel or)
        {
            return dBConnection.ResponseAuditObservation(or);
        }
        [HttpPost]
        public string update_observation_text(int OBS_ID, string OBS_TEXT, int PROCESS_ID = 0, int SUBPROCESS_ID = 0, int CHECKLIST_ID = 0, string OBS_TITLE="")
        {
            string response = "";
            response = dBConnection.UpdateAuditObservationText(OBS_ID, OBS_TEXT, PROCESS_ID, SUBPROCESS_ID, CHECKLIST_ID, OBS_TITLE);
            return "{\"Status\":true,\"Message\":\"" + response + "\"}";
        }
        [HttpPost]
        public string update_observation_status(int OBS_ID, int NEW_STATUS_ID, int RISK_ID, string AUDITOR_COMMENT)
        {
            string response = "";

            if (NEW_STATUS_ID == 4)
                if (RISK_ID != 3)
                    return "{\"Status\":false,\"Message\":\"Only Low Risk para can be settled by Team Lead\"}";

            response = dBConnection.UpdateAuditObservationStatus(OBS_ID, NEW_STATUS_ID, AUDITOR_COMMENT);

            return "{\"Status\":true,\"Message\":\"" + response + "\"}";

        }
        [HttpPost]
        public string drop_observation(int OBS_ID)
        {
            string response = "";
            response = dBConnection.DropAuditObservation(OBS_ID);
            return "{\"Status\":true,\"Message\":\"" + response + "\"}";

        }
        [HttpPost]
        public string submit_observation_to_auditee(int OBS_ID)
        {
            string response = "";
            response = dBConnection.SubmitAuditObservationToAuditee(OBS_ID);
            return "{\"Status\":true,\"Message\":\"" + response + "\"}";

        }
        [HttpPost]
        public List<ManageObservations> get_observation(int ENG_ID = 0, int OBS_ID = 0)
        {
            return dBConnection.GetManagedObservations(ENG_ID, OBS_ID);
        }
        [HttpPost]
        public List<ManageObservations> get_dept_observation_text(int ENG_ID = 0, int OBS_ID = 0)
        {
            return dBConnection.GetManagedObservationText(ENG_ID, OBS_ID);
        }
        [HttpPost]
        public List<SubCheckListStatus> get_subchecklist_status(int ENG_ID = 0, int S_ID = 0)
        {
            return dBConnection.GetSubChecklistStatus(ENG_ID, S_ID);
        }


        [HttpPost]
        public List<ManageObservations> get_observation_branches(int ENG_ID = 0, int OBS_ID = 0)
        {
            return dBConnection.GetManagedObservationsForBranches(ENG_ID, OBS_ID);
        }
        [HttpPost]
        public string add_observation_gist_and_recommendation(int OBS_ID = 0, string GIST_OF_PARA="", string AUDITOR_RECOMMENDATION="")
        {
            string response = "";
            response = dBConnection.AddObservationGistAndRecommendation(OBS_ID, GIST_OF_PARA, AUDITOR_RECOMMENDATION);
            return "{\"Status\":true,\"Message\":\"" + response + "\"}";            
        }
        [HttpPost]
        public List<ManageObservations> get_observation_text_branches(int ENG_ID = 0, int OBS_ID = 0)
        {
            return dBConnection.GetManagedObservationTextForBranches(ENG_ID, OBS_ID);
        }

        [HttpPost]
        public DraftReportSummaryModel draft_report_summary(int ENG_ID)
        {
            DraftReportSummaryModel resp = new DraftReportSummaryModel();
            string filename = "";
            // filename = dBConnection.CreateAuditReport(ENG_ID);
            resp = dBConnection.GetDraftReportSummary(ENG_ID);
            resp.ReportName = filename;
            return resp;
        }
        [HttpPost]
        public List<ClosingDraftTeamDetailsModel> closing_draft_report_status()
        {
            return dBConnection.GetClosingDraftObservations();
        }
        [HttpPost]
        public List<FadOldParaReportModel> get_fad_paras(int PROCESS_ID = 0, int SUB_PROCESS_ID = 0, int PROCESS_DETAIL_ID = 0)
        {
            return dBConnection.GetFadBranchesParas(PROCESS_ID, SUB_PROCESS_ID, PROCESS_DETAIL_ID);
        }
        [HttpPost]
        public List<ClosingDraftTeamDetailsModel> get_team_details(int ENG_ID = 0)
        {
            return dBConnection.GetClosingDraftTeamDetails(ENG_ID);
        }
        [HttpPost]
        public object close_draft_audit(int ENG_ID)
        {
            string response = "";
            response = dBConnection.CloseDraftAuditReport(ENG_ID);
            return "{\"Status\":true,\"Message\":\"" + response + "\"}";
        }
        [HttpPost]
        public object conclude_draft_audit(int ENG_ID)
        {
            string response = "";
            response = dBConnection.ConcludeDraftAuditReport(ENG_ID);
            return "{\"Status\":true,\"Message\":\"" + response + "\"}";
        }
        [HttpPost]
        public List<LoanCaseModel> Loan_Case_Details(int Loan_case, string LOAN_TYPE = "")
        {
            return dBConnection.GetLoanCaseDetails(Loan_case, LOAN_TYPE);
        }
        [HttpPost]
        public GlHeadSubDetailsModel Glhead_Sub_Details(int GLTYPEID)
        {
            return dBConnection.GetGlheadSubDetails(GLTYPEID);
        }
        [HttpPost]
        public List<DepositAccountModel> GetDepositAccountSubdetails(string b_name)
        {
            return dBConnection.GetDepositAccountSubdetails(b_name);
        }
        [HttpPost]
        public List<DepositAccountCatDetailsModel> GetDepositAccountcatdetails(int catid)
        {
            return dBConnection.GetDepositAccountcatdetails(catid);
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
            return dBConnection.GetExpectedCountOfAuditEntitiesOnCriteria(RISK_ID, SIZE_ID, ENTITY_TYPE_ID, PERIOD_ID, FREQUENCY_ID);
        }
        [HttpPost]
        public bool DeletePendingCriteria(int CID = 0)
        {
            return dBConnection.DeletePendingCriteria(CID);
        }
        [HttpPost]
        public List<AuditeeEntitiesModel> GetAuditeeEntitiesByTypeId(int ENTITY_TYPE_ID = 0)
        {
            return dBConnection.GetAuditeeEntitiesForUpdate(ENTITY_TYPE_ID);
        }
        [HttpPost]
        public List<AuditeeEntitiesModel> GetAISEntities(string ENTITY_ID, string TYPE_ID)
        {
            return dBConnection.GetAISEntities( ENTITY_ID,  TYPE_ID);
        }
        [HttpPost]
        public List<AuditeeEntitiesModel> GetCBASEntities(string E_CODE, string E_NAME)
        {
            return dBConnection.GetCBASEntities(E_CODE,E_NAME);
        }
        [HttpPost]
        public List<AuditeeEntitiesModel> GetERPEntities(string E_CODE, string E_NAME)
        {
            return dBConnection.GetERPEntities(E_CODE,E_NAME);
        }
        [HttpPost]
        public List<AuditeeEntitiesModel> GetHREntities(string E_CODE, string E_NAME)
        {
            return dBConnection.GetHREntities(E_CODE,E_NAME);
        }
        [HttpPost]
        public bool submit_audit_criterias(int PERIOD_ID)
        {
            return dBConnection.SubmitAuditCriteriaForApproval(PERIOD_ID);
        }
        [HttpPost]
        public List<COSORiskModel> GetCOSORiskForDepartment(int PERIOD_ID = 0)
        {
            return dBConnection.GetCOSORiskForDepartment(PERIOD_ID);
        }
        [HttpPost]
        public CAUOMAssignmentResponseModel CAU_OM_assignment(CAUOMAssignmentModel caumodel)
        {
            return dBConnection.CAUOMAssignment(caumodel);            
        }
        [HttpPost]
        public CAUOMAssignmentResponseModel CAU_OM_assignmentAIR(CAUOMAssignmentAIRModel caumodel)
        {
            return dBConnection.CAUOMAssignmentAIR(caumodel);
        }
        [HttpPost]
        public CAUOMAssignmentResponseModel CAU_OM_assignmentPDP(List<CAUOMAssignmentPDPModel> DAC_LIST)
        {
            CAUOMAssignmentResponseModel resp= new CAUOMAssignmentResponseModel();
            foreach (CAUOMAssignmentPDPModel pdp in DAC_LIST)
            {
                resp=dBConnection.CAUOMAssignmentPDP(pdp);
            }

            return resp;
            
        }
        [HttpPost]
        public CAUOMAssignmentResponseModel CAU_OM_assignmentARPSE(List<CAUOMAssignmentARPSEModel> PAC_LIST)
        {
            CAUOMAssignmentResponseModel resp = new CAUOMAssignmentResponseModel();
            foreach (CAUOMAssignmentARPSEModel pdp in PAC_LIST)
            {
                resp = dBConnection.CAUOMAssignmentARPSE(pdp);
            }
            return resp;
        }

        [HttpPost]
        public CAUOMAssignmentModel CAU_get_Pre_Added_OM(string OM_NO, string INS_YEAR)
        {
           
               return dBConnection.CAUGetPreAddedOM(OM_NO,INS_YEAR);
            
        }

        [HttpPost]
        public List<CAUOMAssignmentModel> CAU_Get_OMs()
        {
            return dBConnection.CAUGetAssignedOMs();
        }
        [HttpPost]
        public List<ManageObservations> get_observations(int ENG_ID, int OBS_ID = 0)
        {
            return dBConnection.GetManagedObservations(ENG_ID, OBS_ID);

        }
        [HttpPost]
        public List<ManageObservations> get_observations_draft(int ENG_ID, int OBS_ID = 0)
        {
            return dBConnection.GetManagedDraftObservations(ENG_ID, OBS_ID);

        }
        [HttpPost]
        public List<ManageObservations> get_finalized_observations_draft(int ENG_ID, int OBS_ID = 0)
        {
            return dBConnection.GetFinalizedDraftObservations(ENG_ID, OBS_ID);

        }
        [HttpPost]
        public List<ManageObservations> get_finalized_observations_draft_branch(int ENG_ID, int OBS_ID = 0)
        {
            return dBConnection.GetFinalizedDraftObservationsBranch(ENG_ID, OBS_ID);

        }
        [HttpPost]
        public List<ManageObservations> get_observations_draft_branch(int ENG_ID, int OBS_ID = 0)
        {
            return dBConnection.GetManagedDraftObservationsBranch(ENG_ID, OBS_ID);

        }
        [HttpPost]
        public List<ManageObservations> get_observations_draft_text(int ENG_ID, int OBS_ID = 0)
        {
            return dBConnection.GetManagedDraftObservationsText(ENG_ID, OBS_ID);

        }

        [HttpPost]
        public List<ManageObservations> get_observations_draft_auditee_reply(int ENG_ID, int OBS_ID = 0)
        {
            List<ManageObservations> resp = new List<ManageObservations>();
            ManageObservations m = new ManageObservations();
            m.OBS_ID = OBS_ID;
            m.OBS_REPLY = dBConnection.GetLatestAuditeeResponse(OBS_ID);
            resp.Add(m);
            return resp;
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
        public List<object> get_observation_text(int OBS_ID, int RESP_ID)
        {
            return dBConnection.GetObservationText(OBS_ID, RESP_ID);

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
        public List<OldParasModelCAD> get_old_para_management()
        {
            return dBConnection.GetOldParasManagement();
        }
        [HttpPost]
        public List<OldParasModel> get_legacy_para_for_response()
        {
            return dBConnection.GetOldParasForResponse();
        }
        [HttpPost]
        public List<OldParasModel> get_legacy_settled_paras(int ENTITY_ID=0)
        {
            return dBConnection.GetOldSettledParasForResponse(ENTITY_ID);
        }
        [HttpPost]
        public List<OldParasModel> get_current_paras_for_status_change_request(int ENTITY_ID = 0)
        {
            return dBConnection.GetCurrentParasForStatusChangeRequest(ENTITY_ID);
        }
        [HttpPost]
        public List<OldParasModel> get_current_paras_for_status_change_request_review()
        {
            return dBConnection.GetCurrentParasForStatusChangeRequestReview();
        }
        [HttpPost]
        public List<OldParasModel> get_current_paras_for_status_change_request_authorize()
        {
            return dBConnection.GetCurrentParasForStatusChangeRequestAuthorize();
        }
        [HttpPost]
        public List<OldParasModel> get_manage_legacy_para()
        {
            return dBConnection.GetManageLegacyParas();
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
            return dBConnection.AddOldParasReply(ID, REPLY);
        }
        [HttpPost]
        public bool set_manage_legacy_para_status(int ID, int NEW_STATUS)
        {
            return dBConnection.UpdateOldParasStatus(ID, NEW_STATUS);
        }
        [HttpPost]
        public string add_legacy_para_cad_reply(int ID, int V_CAT_ID, int V_CAT_NATURE_ID, int RISK_ID, string REPLY)
        {
            string response = "";
            response = dBConnection.AddOldParasCADReply(ID, V_CAT_ID, V_CAT_NATURE_ID, RISK_ID, REPLY);
            return "{\"Status\":true,\"Message\":\"" + response + "\"}";

        }
        [HttpPost]
        public string add_legacy_para_cad_compliance(List<OldParaComplianceModel> COMPLIANCE_LIST)
        {
            string response = "";
            foreach (OldParaComplianceModel opc in COMPLIANCE_LIST)
            {
                response += dBConnection.AddOldParasCADCompliance(opc) + "\n";
            }

            return "{\"Status\":true,\"Message\":\"" + response + "\"}";

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
        public List<UserRelationshipModel> getparentrelForDashboardPanel(int ENTITY_REALTION_ID)
        {
            return dBConnection.GetparentrepofficeForDashboardPanel(ENTITY_REALTION_ID);
        }
        [HttpPost]
        public List<UserRelationshipModel> getparentrelForParaPositionReport(int ENTITY_REALTION_ID)
        {
            return dBConnection.GetparentrepofficeForParaPositionReport(ENTITY_REALTION_ID);
        }

        [HttpPost]
        public List<UserRelationshipModel> getpostplace(int E_R_ID)
        {
            return dBConnection.Getchildposting(E_R_ID);
        }
        [HttpPost]
        public List<UserRelationshipModel> getpostplaceForDashboardPanel(int E_R_ID)
        {
            return dBConnection.GetchildpostingForDashboardPanel(E_R_ID);
        }
        [HttpPost]
        public List<UserRelationshipModel> getpostplaceForParaPositionReport(int E_R_ID)
        {
            return dBConnection.GetchildpostingForParaPositionReport(E_R_ID);
        }

        [HttpPost]
        public List<ManageObservations> get_violation_observations(int ENTITY_ID, int VIOLATION_ID)
        {
            return dBConnection.GetViolationObservations(ENTITY_ID, VIOLATION_ID);
        }

        [HttpPost]
        public bool approve_engagement_plan(int ENG_ID)
        {
            return dBConnection.ApproveAuditEngagementPlan(ENG_ID);
        }

        [HttpPost]
        public UserModel get_matched_pp_numbers(string PPNO)
        {
            return dBConnection.GetMatchedPPNumbers(PPNO);
        }

        [HttpPost]
        public bool reject_engagement_plan(int ENG_ID, string COMMENTS)
        {
            return dBConnection.RefferedBackAuditEngagementPlan(ENG_ID, COMMENTS);
        }

        [HttpPost]
        public string rerecommend_engagement_plan(int ENG_ID, int PLAN_ID, int ENTITY_ID, DateTime OP_START_DATE, DateTime OP_END_DATE, DateTime START_DATE, DateTime END_DATE, int TEAM_ID, string COMMENTS)
        {
            string response = "";
            response = dBConnection.RerecommendAuditEngagementPlan(ENG_ID, PLAN_ID, ENTITY_ID, OP_START_DATE, OP_END_DATE, START_DATE, END_DATE, TEAM_ID, COMMENTS);
            return "{\"Status\":true,\"Message\":\"" + response + "\"}";
        }


        [HttpPost]
        public List<LoanCasedocModel> Getloancasedocuments()
        {
            return dBConnection.GetLoanCaseDocuments();
        }
        [HttpPost]
        public List<FunctionalResponsibilityWiseParas> get_functional_responsibility_wise_paras(int PROCESS_ID = 0, int SUB_PROCESS_ID = 0, int PROCESS_DETAIL_ID = 0)
        {
            return dBConnection.GetFunctionalResponsibilityWisePara(PROCESS_ID, SUB_PROCESS_ID, PROCESS_DETAIL_ID);
        }
        [HttpPost]
        public bool divisional_head_remarks_on_functional_legacy_para(int CONCERNED_DEPT_ID = 0, string COMMENTS = "", int REF_PARA_ID = 0)
        {
            return dBConnection.AddDivisionalHeadRemarksOnFunctionalLegacyPara(CONCERNED_DEPT_ID, COMMENTS, REF_PARA_ID);
        }
        [HttpPost]
        public bool menu_pages_updation(int MENU_ID = 0, int[] PAGE_IDS = null)
        {
            if (PAGE_IDS != null)
            {
                foreach (var PAGE_ID in PAGE_IDS)
                {
                    dBConnection.UpdateMenuPagesAssignment(MENU_ID, PAGE_ID);
                }
                return true;
            }
            else
                return false;

        }



        [HttpPost]
        public bool addinpectioncriteria(string fquat = "", string squat = "", string tquat = "", string frquat = "")
        {
            return true;// dBConnection.AddInspectionCriteria(fquat, squat, tquat, frquat);
        }



        [HttpPost]

        public bool add_inspection_team(int teamid = 0, string tname = "", int pop = 0)
        {
            return true;// dBConnection.AddInspectionTeam(teamid, tname, pop);
        }

        [HttpPost]

        public bool Join_inspection_team(int e_id = 0, int t_m_ppno = 0, int e_b = 0)
        {
            return true;// dBConnection.InspectionTeamJoining(e_id, t_m_ppno, e_b);
        }

        [HttpPost]
        public List<JoiningCompletionReportModel> get_joining_completion(int DEPT_ID, DateTime AUDIT_STARTDATE, DateTime AUDIT_ENDDATE)
        {
            return dBConnection.GetJoiningCompletion(DEPT_ID, AUDIT_STARTDATE, AUDIT_ENDDATE);

        }

        [HttpPost]
        public List<AuditPlanCompletionReportModel> get_auditplan_completion(int DEPT_ID)
        {
            return dBConnection.GetauditplanCompletion(DEPT_ID);

        }

        [HttpPost]
        public List<CurrentAuditProgress> get_current_audit_progress(int ENTITY_ID)
        {
            return dBConnection.GetCurrentAuditProgress(ENTITY_ID);

        }

        [HttpPost]
        public List<CurrentActiveUsers> get_active_users()
        {
            return dBConnection.GetCurrentActiveUsers();

        }

        [HttpPost]
        public List<ManageObservations> get_entity_report_paras_branch(int ENG_ID)
        {
            return dBConnection.GetEntityReportParasForBranch(ENG_ID);

        }

        [HttpPost]
        public List<AuditeeOldParasModel> get_assigned_observation_old_paras(int ENTITY_ID = 0)
        {
            return dBConnection.GetAuditeeOldParas(ENTITY_ID);

        }

        [HttpPost]
        public List<AuditeeAddressModel> get_address(int ENT_ID)
        {
            return dBConnection.GetAddress(ENT_ID);

        }
       
        [HttpPost]
        public List<GetFinalReportModel> get_report_paras(int ENG_ID)
        {
            return dBConnection.GetAuditeeParas(ENG_ID);
        }

        [HttpPost]
        public List<AuditChecklistDetailsModel> get_obs_for_pre_concluding(int ENG_ID)
        {
            return dBConnection.GetEntityObservationDetails(ENG_ID);
        }

        [HttpPost]
        public List<GetOldParasBranchComplianceModel> get_old_para_br_compliance()
        {
            return dBConnection.GetOldParasBranchCompliance();
        }
        [HttpPost]
        public List<GetOldParasBranchComplianceModel> get_old_para_br_compliance_ref()
        {
            return dBConnection.GetOldParasBranchComplianceRef();
        }

        [HttpPost]
        public GetOldParasBranchComplianceTextModel get_old_para_br_compliance_text(string REF_P, string PARA_CATEGORY, string REPLY_DATE, string OBS_ID)
        {
            return dBConnection.GetOldParasBranchComplianceText(REF_P, PARA_CATEGORY, REPLY_DATE, OBS_ID);
        }
        [HttpPost]
        public GetOldParasBranchComplianceTextModel get_old_para_compliance_cycle_text(string REF_P, string OBS_ID, string COM_SEQ)
        {
            return dBConnection.GetOldParasComplianceCycleText(REF_P, OBS_ID, COM_SEQ);
        }
        [HttpPost]
        public GetOldParasBranchComplianceTextModel get_old_para_br_compliance_text_ref(string REF_P, string PARA_CATEGORY, string REPLY_DATE, string OBS_ID)
        {
            return dBConnection.GetOldParasBranchComplianceTextRef(REF_P, PARA_CATEGORY, REPLY_DATE, OBS_ID);
        }
        [HttpPost]
        public GetOldParasBranchComplianceTextModel get_old_para_zone_compliance_text(string REF_P, string PARA_CATEGORY, string REPLY_DATE, string OBS_ID)
        {
            return dBConnection.GetOldParasBranchComplianceTextForZone(REF_P, PARA_CATEGORY, REPLY_DATE, OBS_ID);
        }
        [HttpPost]
        public GetOldParasBranchComplianceTextModel get_old_para_zone_compliance_text_ref(string REF_P, string PARA_CATEGORY, string REPLY_DATE, string OBS_ID)
        {
            return dBConnection.GetOldParasBranchComplianceTextForZoneRef(REF_P, PARA_CATEGORY, REPLY_DATE, OBS_ID);
        }


        [HttpPost]
        public GetOldParasBranchComplianceTextModel get_old_para_imp_text(int PID, string REF_P, string PARA_CATEGORY, string REPLY_DATE, string OBS_ID)
        {
            return dBConnection.GetOldParasBranchComplianceTextForImpIncharge(PID,REF_P, PARA_CATEGORY, REPLY_DATE, OBS_ID);
        }

        [HttpPost]
        public GetOldParasBranchComplianceTextModel get_old_para_imp_text_ref(int PID, string REF_P, string PARA_CATEGORY, string REPLY_DATE, string OBS_ID)
        {
            return dBConnection.GetOldParasReferredBackBranchComplianceTextForImpIncharge(PID, REF_P, PARA_CATEGORY, REPLY_DATE, OBS_ID);
        }
        [HttpPost]
        public GetOldParasBranchComplianceTextModel get_old_para_head_az_text(int PID,  string REF_P, string OBS_ID, string PARA_CATEGORY, string REPLY_DATE)
        {
            return dBConnection.GetOldParasBranchComplianceTextForHeadAZ(PID, REF_P, OBS_ID, PARA_CATEGORY, REPLY_DATE);
        }


        [HttpPost]
        public string add_old_para_br_compliance_reply(string Para_ID, int AU_OBS_ID , string Para_Cat, string REPLY, List<AuditeeResponseEvidenceModel> EVIDENCE_LIST, string AUDITED_BY)
        {
            string response = "";
            response = dBConnection.AddOldParasBranchComplianceReply(Para_ID, AU_OBS_ID, Para_Cat, REPLY, EVIDENCE_LIST,AUDITED_BY);
            return "{\"Status\":true,\"Message\":\"" + response + "\"}";
        }
        [HttpPost]
        public List<GetOldParasForComplianceReviewer> get_branch_comp_review()
        {
            return dBConnection.GetOldParasForReviewer();
        }
        [HttpPost]
        public List<GetOldParasForComplianceReviewer> get_branch_comp_review_ref()
        {
            return dBConnection.GetOldParasForReviewerRef();
        }

        [HttpPost]
        public string AddOldParasComplianceReviewer(string Para_ID, string PARA_CAT, string REPLY, string r_status, string OBS_ID, int PARENT_ID, string SEQUENCE, string AUDITED_BY)
        {
            string response = "";
            response = dBConnection.AddOldParasComplianceReviewer(Para_ID, PARA_CAT, REPLY, r_status, OBS_ID, PARENT_ID, SEQUENCE, AUDITED_BY);
            return "{\"Status\":true,\"Message\":\"" + response + "\"}";
        }

        [HttpPost]
        public List<GetOldParasforComplianceSettlement> get_old_para_br_compliance_submission()
        {
            return dBConnection.GetOldParasBranchComplianceSubmission();
        }

        [HttpPost]
        public List<GetOldParasforComplianceSettlement> get_old_para_br_compliance_recommendation()
        {
            return dBConnection.GetComplianceForImpZone();
        }
        [HttpPost]
        public List<GetOldParasforComplianceSettlement> get_old_para_br_compliance_recommendation_ref()
        {
            return dBConnection.GetReferredBackParasComplianceForImpZone();
        }

        [HttpPost]
        public string submit_old_para_br_compliance_status(string OBS_ID, string REFID, string REMARKS, int NEW_STATUS, string PARA_CAT, string SETTLE_INDICATOR, string SEQUENCE, string AUDITED_BY)
        {
            string response = "";
            response = dBConnection.AddOldParasStatusUpdate(OBS_ID, REFID, REMARKS, NEW_STATUS, PARA_CAT, SETTLE_INDICATOR, SEQUENCE, AUDITED_BY);
            return "{\"Status\":true,\"Message\":\"" + response + "\"}";
        }

        [HttpPost]
        public string submit_old_para_br_compliance_status_partially_settle(string OBS_ID, string REFID, string REMARKS, int NEW_STATUS, string PARA_CAT, string SETTLE_INDICATOR, List<ObservationResponsiblePPNOModel> RESPONSIBLES_ARR, string SEQUENCE,string AUDITED_BY)
        {
            string response = "";
            response = dBConnection.AddOldParasStatusPartiallySettle(OBS_ID, REFID, REMARKS, NEW_STATUS, PARA_CAT, SETTLE_INDICATOR, RESPONSIBLES_ARR, SEQUENCE, AUDITED_BY);
            return "{\"Status\":true,\"Message\":\"" + response + "\"}";
        }

        [HttpPost]
        public List<GetOldParasForFinalSettlement> get_old_para_br_compliance_head()
        {
            return dBConnection.GetOldParasForFinalSettlement();
        }       

        [HttpPost]
        public string submit_old_para_compliance_head_status(int PARA_ID, string REMARKS, int NEW_STATUS, string PARA_REF, string PARA_INDICATOR, string PARA_CATEGORY, int AU_OBS_ID, string SEQUENCE, string AUDITED_BY)
        {
            string response = "";
            response = dBConnection.AddOldParasheadStatusUpdate(PARA_ID, REMARKS, NEW_STATUS, PARA_REF, PARA_INDICATOR, PARA_CATEGORY, AU_OBS_ID, SEQUENCE, AUDITED_BY);
            return "{\"Status\":true,\"Message\":\"" + response + "\"}";
        }

        [HttpPost]
        public List<BranchModel> get_zone_Branches(int ZONEID)
        {
            return dBConnection.GetZoneBranches(ZONEID,false);
            
        }
        [HttpPost]
        public List<AuditeeOldParasModel> get_old_paras_for_monitoring(int ENTITY_ID)
        {
            return dBConnection.GetOldParasForMonitoring(ENTITY_ID);
        }

        [HttpPost]
        public string get_para_text(string ref_p)
        {
            return dBConnection.GetParaText(ref_p);
         }

       

        [HttpPost]
        public List<AuditeeOldParasPpnoModel> get_old_paras_for_monitoring_ppno(int ppno)
        {
            return dBConnection.GetOldParasForMonitoringPpno(ppno);
        }

        [HttpPost]
        public List<UserModel> find_users(FindUserModel user)
        {
            return dBConnection.GetAllUsers(user);
        }
        [HttpPost]
        public string get_user_name(string PPNUMBER)
        {
           string response = "";
            response=dBConnection.GetUserName(PPNUMBER);
           return "{\"Status\":true,\"Message\":\"" + response + "\"}";
        }
        [HttpPost]
        public string Add_Old_Para_Change_status(string REFID, int NEW_STATUS, string REMARKS)
        {
            string response = "";
            response = dBConnection.AddChangeStatusRequestForSettledPara(REFID, NEW_STATUS, REMARKS);
            return "{\"Status\":true,\"Message\":\"" + response + "\"}";
        }
        [HttpPost]
        public string Add_Old_Para_Change_status_Review(string REFID, string REMARKS)
        {
            string response = "";
            response = dBConnection.ReviewerAddChangeStatusRequestForSettledPara(REFID, REMARKS);
            return "{\"Status\":true,\"Message\":\"" + response + "\"}";
        }
        [HttpPost]
        public string Add_Old_Para_Change_status_Authorize(string REFID, int NEW_STATUS, string REMARKS)
        {
            string response = "";
            response = dBConnection.AuthorizerAddChangeStatusRequestForSettledPara(REFID, NEW_STATUS, REMARKS);
            return "{\"Status\":true,\"Message\":\"" + response + "\"}";
        }

        [HttpPost]
        public string Add_New_Para_Change_status_Request(string REFID, int NEW_STATUS, string REMARKS)
        {
            string response = "";
            response = dBConnection.AddChangeStatusRequestForCurrentPara(REFID, NEW_STATUS, REMARKS);
            return "{\"Status\":true,\"Message\":\"" + response + "\"}";
        }


        [HttpPost]
        public List<ZoneBranchParaStatusModel> get_zone_brach_para_position(int ENTITY_ID)
        {
            return dBConnection.GetZoneBranchParaPositionStatus(ENTITY_ID);
        }
        [HttpPost]
        public string Add_Authorization_Old_Para_Change_status(string REFID, int NEW_STATUS)
        {
            string response = "";
            response = dBConnection.AddAuthorizeChangeStatusRequestForSettledPara(REFID, NEW_STATUS);
            return "{\"Status\":true,\"Message\":\"" + response + "\"}";
        }
        [HttpPost]
        public List<OldParasAuthorizeModel> get_legacy_settled_paras_autorize()
        {
            return dBConnection.GetOldSettledParasForResponseAuthorize();
        }

        [HttpPost]
        public List<GetOldParasBranchComplianceModel> get_old_para_br_compliance_text_update()
        {
            return dBConnection.GetOldParasBranchComplianceTextupdate();
        }

        [HttpPost]
        public List<GetTeamDetailsModel> GetTeamDetails(int ENG_ID)
        {
            return dBConnection.GetTeamDetails(ENG_ID);
        }




        [HttpPost]
        public List<GetAuditeeParasModel> get_report_status(int ENG_ID)
        {
            return dBConnection.GetAuditeReportStatus(ENG_ID);
        }
        [HttpPost]
        public string submit_pre_concluding(int ENG_ID)
        {
            string response = "";
            response = dBConnection.SubmitPreConcluding(ENG_ID);
            return "{\"Status\":true,\"Message\":\"" + response + "\"}";
        }
        [HttpPost]
        public List<OldParasModel> get_legacy_paras_for_update(int ENTITY_ID, string PARA_REF, int PARA_ID=0)
        {
            return dBConnection.GetLegacyParasForUpdate(ENTITY_ID, PARA_REF, PARA_ID);            
        }
        [HttpPost]
        public List<OldParasModel> get_legacy_paras_for_update_ho(string ENTITY_NAME, string PARA_REF, int PARA_ID = 0)
        {
            return dBConnection.GetLegacyParasForUpdateHO(ENTITY_NAME, PARA_REF, PARA_ID);
        }

        [HttpPost]
        public List<OldParasModel> get_legacy_paras_for_gist_update(int ENTITY_ID, string PARA_REF, int PARA_ID = 0)
        {
            return dBConnection.GetLegacyParasForGistUpdate(ENTITY_ID, PARA_REF, PARA_ID);
        }


        [HttpPost]
        public List<OldParasModel> get_legacy_paras_for_update_FAD(int ENTITY_ID, string PARA_REF, int PARA_ID = 0)
        {
            return dBConnection.GetLegacyParasForUpdateFAD(ENTITY_ID, PARA_REF, PARA_ID);
        }

        [HttpPost]
        public string update_legacy_para_with_responsibilities(AddLegacyParaModel LEGACY_PARA)
        {
            return "{\"Status\":true,\"Message\":\"" + dBConnection.UpdateLegacyParasWithResponsibility(LEGACY_PARA) + "\"}";

        }

        [HttpPost]
        public string update_legacy_para_gist_paraNo(string PARA_REF, string PARA_NO, string GIST_OF_PARA)
        {
            return "{\"Status\":true,\"Message\":\"" + dBConnection.UpdateLegacyParaGistParaNo(PARA_REF, PARA_NO, GIST_OF_PARA) + "\"}";

        }
        //
        [HttpPost]
        public string delete_responsibility_of_legacy_para(string REF_P, int P_ID, int PP_NO)
        {
            return "{\"Status\":true,\"Message\":\"" + dBConnection.DeleteResponsibilityOfLegacyParas(REF_P, P_ID, PP_NO) + "\"}";

        }
       
        [HttpPost]
        public string add_responsibility_to_legacy_para(ObservationResponsiblePPNOModel RESP_PP, string REF_P, int P_ID)
        {
            return "{\"Status\":true,\"Message\":\"" + dBConnection.AddResponsibilityToLegacyParas(RESP_PP, REF_P, P_ID) + "\"}";

        }

        [HttpPost]
        public string add_responsibility_to_legacy_para_fad(ObservationResponsiblePPNOModel RESP_PP, string REF_P, int P_ID)
        {
            return "{\"Status\":true,\"Message\":\"" + dBConnection.AddResponsibilityToLegacyParasFAD(RESP_PP, REF_P, P_ID) + "\"}";

        }


        [HttpPost]
        public string update_legacy_para_with_responsibilities_no_changes_AZ(AddLegacyParaModel LEGACY_PARA)
        {
            return "{\"Status\":true,\"Message\":\"" + dBConnection.UpdateLegacyParasWithResponsibilityNoChangesAZ(LEGACY_PARA) + "\"}";

        }

        [HttpPost]
        public string update_legacy_para_with_responsibilities_no_changes(AddLegacyParaModel LEGACY_PARA)
        {
            return "{\"Status\":true,\"Message\":\"" + dBConnection.UpdateLegacyParasWithResponsibilityNoChanges(LEGACY_PARA) + "\"}";

        }

        [HttpPost]
        public string get_employee_name_from_pp(int PP_NO)
        {
            return "{\"Status\":true,\"Message\":\"" + dBConnection.GetEmployeeNameFromPPNO(PP_NO) + "\"}";

        }

        [HttpPost]
        public string update_legacy_para_with_responsibilities_FAD(AddLegacyParaModel LEGACY_PARA)
        {
            return "{\"Status\":true,\"Message\":\"" + dBConnection.UpdateLegacyParasWithResponsibilityFAD(LEGACY_PARA) + "\"}";

        }

        [HttpPost]
        public List<AuditPlanReportModel> GetFADAuditPlan(int ENT_ID, int Z_ID, int RISK, int SIZE)
        {
            return dBConnection.GetFadAuditPlanReport(ENT_ID, Z_ID, RISK, SIZE);


        }
        [HttpPost]
        public List<FADNewOldParaPerformanceModel> get_fad_new_old_para_performance(int AUDIT_ZONE_ID)
        {
            return dBConnection.GetFADNewOldParaPerformance(AUDIT_ZONE_ID);
        }
        [HttpPost]
        public List<LegacyZoneWiseOldParasPerformanceModel> get_legacy_zone_wise_performance(DateTime? FILTER_DATE)
        {
            return dBConnection.GetLegacyZoneWiseOldParasPerformance(FILTER_DATE);
        }
        [HttpPost]
        public List<LegacyUserWiseOldParasPerformanceModel> get_legacy_user_wise_performance(DateTime? FILTER_DATE)
        {
            return dBConnection.GetLegacyUserWiseOldParasPerformance(FILTER_DATE);
        }
        [HttpPost]
        public List<FADHOUserLegacyParaUserWiseParasPerformanceModel> get_fad_ho_user_legacy_para_user_wise_performance(DateTime? FILTER_DATE)
        {
            return dBConnection.GetFADHOUserLegacyParaUserWiseOldParasPerformance(FILTER_DATE);
        }

        [HttpPost]
        public string delete_legacy_para_responsibility(string PARA_REF, int PARA_ID, int PP_NO)
        {
            return "{\"Status\":true,\"Message\":\"" + dBConnection.DeleteLegacyParaResponsibility(PARA_REF, PARA_ID, PP_NO) + "\"}";
        }

        [HttpPost]
        public List<AuditEntitiesModel> get_auditee_entities_by_entity_type_id(  int ENTITY_TYPE_ID)
        {
            return  dBConnection.GetAuditEntitiesByTypeId(ENTITY_TYPE_ID);
        }

        [HttpPost]
        public string add_new_legacy_para(AddNewLegacyParaModel LEGACY_PARA)
        {
            return "{\"Status\":true,\"Message\":\"" + dBConnection.AddNewLegacyPara(LEGACY_PARA) + "\"}";
        }

        [HttpPost]
        public string refer_back_legacy_para_to_az(string PARA_REF, int PARA_ID)
        {
            return "{\"Status\":true,\"Message\":\"" + dBConnection.ReferBackLegacyPara(PARA_REF, PARA_ID) + "\"}";
        }
        [HttpPost]
        public List<AddNewLegacyParaModel> get_add_legacy_paras_autorize()
        {
            return dBConnection.GetAddedLegacyParaForAuthorize();
        }

        //
        [HttpPost]
        public List<AddNewLegacyParaModel> get_update_gist_paraNo_legacy_paras_autorize()
        {
            return dBConnection.GetUpdatedGistParaOfLegacyParaForAuthorize();
        }

        [HttpPost]
        public string Authorize_Legacy_Para_addition(string PARA_REF)
        {
            return "{\"Status\":true,\"Message\":\"" + dBConnection.AuthorizeLegacyParaAddition(PARA_REF) + "\"}";
        }
        [HttpPost]
        public string Authorize_Legacy_Para_Gist_ParaNo(string PARA_REF, string GIST_OF_PARA, string PARA_NO)
        {
            return "{\"Status\":true,\"Message\":\"" + dBConnection.AuthorizeLegacyParaGistParaNoUpdate(PARA_REF, GIST_OF_PARA, PARA_NO) + "\"}";
        }

        [HttpPost]
        public string Delete_Legacy_Para_addition_request(string PARA_REF)
        {
            return "{\"Status\":true,\"Message\":\"" + dBConnection.DeleteLegacyParaAdditionRequest(PARA_REF) + "\"}";
        }

        [HttpPost]
        public List<AuditeeOldParasModel> get_legacy_report_dropdown_contents(int ENTITY_ID)
        {
            return  dBConnection.GetLegacyParasEntitiesReport(ENTITY_ID) ;
        }

        [HttpPost]
        public string settle_legacy_para_HO(int NEW_STATUS, string PARA_REF, string SETTLEMENT_NOTES )
        {
            return "{\"Status\":true,\"Message\":\"" + dBConnection.SettleLegacyParaHO(NEW_STATUS, PARA_REF, SETTLEMENT_NOTES) + "\"}";
        }
        [HttpPost]
        public string delete_legacy_para_HO( string PARA_REF)
        {
            return "{\"Status\":true,\"Message\":\"" + dBConnection.DeleteLegacyParaHO( PARA_REF) + "\"}";
        }

        [HttpPost]
        public List<FADNewOldParaPerformanceModel> get_relation_legacy_observation_for_dashboard(int ENTITY_ID=0)
        {
            return dBConnection.GetRelationLegacyObservationForDashboard(ENTITY_ID);
        }
        [HttpPost]
        public List<FADNewOldParaPerformanceModel> get_relation_ais_observation_for_dashboard(int ENTITY_ID = 0)
        {
            return dBConnection.GetRelationAISObservationForDashboard(ENTITY_ID);
        }
        [HttpPost]
        public List<FADNewOldParaPerformanceModel> get_relation_observation_for_dashboard(int ENTITY_ID = 0)
        {
            return dBConnection.GetRelationObservationForDashboard(ENTITY_ID);
        }
        [HttpPost]
        public List<FADNewOldParaPerformanceModel> get_functional_responsibility_wise_paras_for_dashboard(int PROCESS_ID = 0, int SUB_PROCESS_ID = 0, int PROCESS_DETAIL_ID = 0, int FUNCTIONAL_ENTITY_ID=0)
        {
            return dBConnection.GetFunctionalResponsibilityWiseParaForDashboard(PROCESS_ID, SUB_PROCESS_ID, PROCESS_DETAIL_ID, FUNCTIONAL_ENTITY_ID);
        }
        [HttpPost]
        public List<FADNewOldParaPerformanceModel> get_functional_responsibility_wise_paras_for_dashboard_ho(int PROCESS_ID = 0, int SUB_PROCESS_ID = 0, int PROCESS_DETAIL_ID = 0, int FUNCTIONAL_ENTITY_ID = 0, int DEPT_ID=0)
        {
            return dBConnection.GetHOFunctionalResponsibilityWiseParaForDashboard(PROCESS_ID, SUB_PROCESS_ID, PROCESS_DETAIL_ID, FUNCTIONAL_ENTITY_ID, DEPT_ID);
        }
        [HttpPost]
        public List<FADNewOldParaPerformanceModel> get_violation_wise_paras_for_dashboard(int PROCESS_ID = 0, int SUB_PROCESS_ID = 0, int PROCESS_DETAIL_ID = 0)
        {
            return dBConnection.GetViolationWiseParaForDashboard(PROCESS_ID, SUB_PROCESS_ID, PROCESS_DETAIL_ID);
        }
        [HttpPost]
        public List<NoEntitiesRiskBasePlan> get_risk_base_plan_for_dashboard()
        {
            return dBConnection.GetEntitiesRiskBasePlanForDashboard();
        }
        [HttpPost]
        public List<FADAuditPerformanceModel> get_audit_performance_for_dashboard()
        {
            return dBConnection.GetAuditPerformanceForDashboard();
        }

        [HttpPost]
        public List<SubCheckListStatus> get_audit_sub_checklist(int PROCESS_ID = 0)
        {
            return dBConnection.GetAuditSubChecklist(PROCESS_ID);
        }
        [HttpPost]
        public string add_audit_sub_checklist(int PROCESS_ID = 0, int ENTITY_TYPE_ID = 0, string HEADING = "")
        {
            return "{\"Status\":true,\"Message\":\"" + dBConnection.AddAuditSubChecklist(PROCESS_ID, ENTITY_TYPE_ID, HEADING) + "\"}";
        }
        [HttpPost]
        public string update_audit_sub_checklist(int PROCESS_ID = 0, int OLD_PROCESS_ID = 0, int SUB_PROCESS_ID=0, string HEADING="", int ENTITY_TYPE_ID = 0)
        {
            return "{\"Status\":true,\"Message\":\"" + dBConnection.UpdateAuditSubChecklist(PROCESS_ID, OLD_PROCESS_ID, SUB_PROCESS_ID, HEADING, ENTITY_TYPE_ID) + "\"}";
        }

        [HttpPost]
        public List<SubProcessUpdateModelForReviewAndAuthorizeModel> get_sub_checklist_comparison_by_Id(int SUB_PROCESS_ID = 0)
        {
            return dBConnection.GetSubChecklistComparisonDetailById(SUB_PROCESS_ID);
        }

        [HttpPost]
        public List<AuditChecklistDetailsModel> get_audit_checklist_detail(int SUB_PROCESS_ID = 0)
        {
            return dBConnection.GetAuditChecklistDetail(SUB_PROCESS_ID);
        }

        [HttpPost]
        public List<AuditChecklistDetailsModel> get_checklist_details_for_sub_process(int SUB_PROCESS_ID = 0)
        {
            return dBConnection.GetChecklistDetailForSubProcess(SUB_PROCESS_ID);
        }
        [HttpPost]
        public List<AuditChecklistDetailsModel> get_ref_audit_checklist_detail()
        {
            return dBConnection.GetReferredBackAuditChecklistDetail();
        }

        [HttpPost]
        public string add_audit_checklist_detail(int PROCESS_ID = 0, int SUB_PROCESS_ID = 0, string HEADING = "", int V_ID=0, int CONTROL_ID=0, int ROLE_ID = 0, int RISK_ID = 0, string ANNEX_CODE="")
        {
            return "{\"Status\":true,\"Message\":\"" + dBConnection.AddAuditChecklistDetail(PROCESS_ID, SUB_PROCESS_ID, HEADING,  V_ID, CONTROL_ID, ROLE_ID ,RISK_ID, ANNEX_CODE ) + "\"}";
        }
        [HttpPost]
        public string update_audit_checklist_detail(int PROCESS_DETAIL_ID = 0, int PROCESS_ID = 0, int SUB_PROCESS_ID = 0, string HEADING = "", int V_ID = 0, int CONTROL_ID = 0, int ROLE_ID = 0, int RISK_ID = 0, string ANNEX_CODE = "")
        {
            return "{\"Status\":true,\"Message\":\"" + dBConnection.UpdateAuditChecklistDetail(PROCESS_DETAIL_ID,PROCESS_ID, SUB_PROCESS_ID, HEADING, V_ID, CONTROL_ID, ROLE_ID, RISK_ID, ANNEX_CODE) + "\"}";
        }

        [HttpPost]
        public List<ParaPositionReportModel> get_para_position_report(int P_ID=0, int C_ID=0)
        {
            return dBConnection.GetParaPositionReport(P_ID, C_ID);
        }

        [HttpPost]
        public List<RepetativeParaModel> get_repetative_paras_for_dashboard(int P_ID = 0, int SP_ID = 0, int PD_ID = 0)
        {
            return dBConnection.GetRepetativeParaForDashboard(P_ID, SP_ID, PD_ID);
        }

        [HttpPost]
        public string add_audit_checklist(string HEADING = "", int ENTITY_TYPE_ID =0)
        {
            return "{\"Status\":true,\"Message\":\"" + dBConnection.AddAuditChecklist(HEADING, ENTITY_TYPE_ID) + "\"}";
        }

        [HttpPost]
        public string update_audit_checklist(int PROCESS_ID = 0,  string HEADING = "", string ACTIVE="")
        {
            return "{\"Status\":true,\"Message\":\"" + dBConnection.UpdateAuditChecklist(PROCESS_ID, HEADING, ACTIVE) + "\"}";
        }        

        [HttpPost]
        public List<AuditeeEntitiesModel> get_entities_parent_ent_type_id(int ENTITY_TYPE_ID = 0)
        {
            return dBConnection.GetEntitiesByParentEntityTypeId(ENTITY_TYPE_ID);
        }

        [HttpPost]
        public List<ParaPositionDetailsModel> get_para_position_details(int ENTITY_ID = 0, int AUDIT_PERIOD=0)
        {
            return dBConnection.GetParaPositionParaDetails(ENTITY_ID, AUDIT_PERIOD);
        }
        [HttpPost]
        public List<ObservationReversalModel> get_engagements_details_for_status_reversal(int ENTITY_ID=0)
        {
            return dBConnection.GetEngagementDetailsForStatusReversal(ENTITY_ID);

        }
        [HttpPost]
        public List<EngagementObservationsForStatusReversalModel> get_observation_details_for_status_reversal(int ENG_ID = 0)
        {
            return dBConnection.GetObservationDetailsForStatusReversal(ENG_ID);

        }
        [HttpPost]
        public string get_compliance_text_auditee(int COMPLIANCE_ID)
        {
            return "{\"Status\":true,\"Message\":\"" + dBConnection.GetComplianceTextAuditee(COMPLIANCE_ID) + "\"}";

        }
        [HttpPost]
        public string get_compliance_history_count_auditee(string REF_P, string OBS_ID)
        {
            return "{\"Status\":true,\"Message\":\"" + dBConnection.GetComplianceHistoryCountAuditee(REF_P, OBS_ID) + "\"}";

        }
        [HttpPost]
        public List<ComplianceHistoryModel> get_compliance_history_auditee(string REF_P, string OBS_ID)
        {
            return dBConnection.GetComplianceHistoryAuditee(REF_P, OBS_ID);

        }
        [HttpPost]
        public string get_compliance_history_count(string REF_P, string OBS_ID)
        {
            return "{\"Status\":true,\"Message\":\"" + dBConnection.GetComplianceHistoryCount(REF_P, OBS_ID) + "\"}";

        }
        [HttpPost]
        public List<ComplianceHistoryModel> get_compliance_history(string REF_P, string OBS_ID)
        {
            return dBConnection.GetComplianceHistory(REF_P, OBS_ID);

        }

        [HttpPost]
        public string get_new_para_text(string OBS_ID)
        {
            return dBConnection.GetNewParaText(OBS_ID);

        }
        [HttpPost]
        public List<RiskProcessDefinition> get_violation_area_for_functional_responsibility_wise_paras(int FUNCTIONAL_ENTITY_ID=0)
        {
            return dBConnection.GetViolationListForDashboard(FUNCTIONAL_ENTITY_ID);

        }
        [HttpPost]
        public List<RiskProcessDefinition> get_sub_violation_area_for_functional_responsibility_wise_paras(int FUNCTIONAL_ENTITY_ID = 0, int PROCESS_ID=0)
        {
            return dBConnection.GetSubViolationListForDashboard(FUNCTIONAL_ENTITY_ID, PROCESS_ID);

        }

        [HttpPost]
        public List<RiskProcessDefinition> get_functional_owner_area_for_functional_responsibility_wise_paras_ho(int ENTITY_ID = 0)
        {
            return dBConnection.GetHOFunctionalListForDashboard(ENTITY_ID);

        }
        [HttpPost]
        public List<RiskProcessDefinition> get_violation_area_for_functional_responsibility_wise_paras_ho(int FUNCTIONAL_ENTITY_ID = 0)
        {
            return dBConnection.GetHOViolationListForDashboard(FUNCTIONAL_ENTITY_ID);

        }
        [HttpPost]
        public List<RiskProcessDefinition> get_sub_violation_area_for_functional_responsibility_wise_paras_ho(int FUNCTIONAL_ENTITY_ID = 0, int PROCESS_ID = 0)
        {
            return dBConnection.GetHOSubViolationListForDashboard(FUNCTIONAL_ENTITY_ID, PROCESS_ID);

        }
        [HttpPost]
        public List<FADNewOldParaPerformanceModel> get_total_para_details_ho(int ENTITY_ID=0)
        {
            return dBConnection.GetTotalParasDetailsHO(ENTITY_ID);

        }
        [HttpPost]
        public List<ObservationReversalModel> get_auditee_engagement_plan(int ENTITY_ID, int PERIOD)
        {
            return dBConnection.GetAuditeeEngagements(ENTITY_ID, PERIOD);

        }
        [HttpPost]
        public List<AuditeeRiskModel> get_auditee_risk(int ENG_ID)
        {
            return dBConnection.GetAuditeeRisk(ENG_ID);

        }

        [HttpPost]
        public List<RiskAssessmentEntTypeModel> get_auditee_risk_for_entity_types(int ENT_TYPE_ID = 0, int PERIOD = 0)
        {
            return dBConnection.GetAuditeeRiskForEntTypes(ENT_TYPE_ID,PERIOD);

        }

        [HttpPost]
        public List<AuditeeRiskModeldetails> get_auditee_risk_details(int ENG_ID)
        {
            return dBConnection.GetAuditeeRiskDetails(ENG_ID);

        }
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}