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
            return dBConnection.RecommendProcessTransactionByReviewer(T_ID, COMMENTS);
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
        public string update_observation_text(int OBS_ID, string OBS_TEXT, int PROCESS_ID = 0, int SUBPROCESS_ID = 0, int CHECKLIST_ID = 0)
        {
            string response = "";
            response = dBConnection.UpdateAuditObservationText(OBS_ID, OBS_TEXT, PROCESS_ID, SUBPROCESS_ID, CHECKLIST_ID);
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
        public List<OldParasModel> get_legacy_settled_paras()
        {
            return dBConnection.GetOldSettledParasForResponse();
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
        public List<UserRelationshipModel> getpostplace(int E_R_ID)
        {
            return dBConnection.Getchildposting(E_R_ID);
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
        public List<GetAuditeeParasModel> get_report_paras(int ENG_ID)
        {
            return dBConnection.GetAuditeeParas(ENG_ID);
        }

        [HttpPost]
        public List<GetOldParasBranchComplianceModel> get_old_para_br_compliance()
        {
            return dBConnection.GetOldParasBranchCompliance();
        }

        [HttpPost]
        public GetOldParasBranchComplianceTextModel get_old_para_br_compliance_text(string REF_P)
        {
            return dBConnection.GetOldParasBranchComplianceText(REF_P);
        }

        [HttpPost]
        public string add_old_para_br_compliance_reply(string Para_ID, string REPLY, List<AuditeeResponseEvidenceModel> EVIDENCE_LIST)
        {
            string response = "";
            response = dBConnection.AddOldParasBranchComplianceReply(Para_ID, REPLY, EVIDENCE_LIST);
            return "{\"Status\":true,\"Message\":\"" + response + "\"}";
        }
        [HttpPost]
        public List<GetOldParasForComplianceReviewer> get_branch_comp_review()
        {
            return dBConnection.GetOldParasForReviewer();
        }

        [HttpPost]
        public string AddOldParasComplianceReviewer(string Para_ID, string REPLY, string r_status)
        {
            string response = "";
            response = dBConnection.AddOldParasComplianceReviewer(Para_ID, REPLY, r_status);
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
            return dBConnection.GetOldParasBranchComplianceRecommendation();
        }

        [HttpPost]
        public string submit_old_para_br_compliance_status(int PARA_ID, string REFID, string REMARKS, int NEW_STATUS)
        {
            string response = "";
            response = dBConnection.AddOldParasStatusUpdate(PARA_ID, REFID, REMARKS, NEW_STATUS);
            return "{\"Status\":true,\"Message\":\"" + response + "\"}";
        }
      

        [HttpPost]
        public List<GetOldParasForFinalSettlement> get_old_para_br_compliance_head()
        {
            return dBConnection.GetOldParasForFinalSettlement();
        }
       

        [HttpPost]
        public string submit_old_para_compliance_head_status(int PARA_ID, string REMARKS, int NEW_STATUS)
        {
            string response = "";
            response = dBConnection.AddOldParasheadStatusUpdate(PARA_ID, REMARKS, NEW_STATUS);
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
           // string response = "";
            return dBConnection.GetParaText(ref_p);
            //return "{\"Status\":true,\"Message\":\"" + response + "\"}";

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
        public string Add_Old_Para_Change_status(string REFID, string REMARKS)
        {
            string response = "";
            response = dBConnection.AddChangeStatusRequestForSettledPara(REFID, REMARKS);
            return "{\"Status\":true,\"Message\":\"" + response + "\"}";
        }

        [HttpPost]
        public List<ZoneBranchParaStatusModel> get_zone_brach_para_position(int ENTITY_ID)
        {
            return dBConnection.GetZoneBranchParaPositionStatus(ENTITY_ID);
        }
        [HttpPost]
        public string Add_Authorization_Old_Para_Change_status(string REFID)
        {
            string response = "";
            response = dBConnection.AddAuthorizeChangeStatusRequestForSettledPara(REFID);
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




        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}