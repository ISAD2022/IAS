using AIS.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;


namespace AIS.Controllers
    {

    public class ApiCallsController : Controller
        {
        private readonly ILogger<ApiCallsController> _logger;

        private readonly SessionHandler sessionHandler;
        private readonly DBConnection dBConnection;
        private readonly IWebHostEnvironment _hostingEnvironment;

        public ApiCallsController(ILogger<ApiCallsController> logger, SessionHandler _sessionHandler, DBConnection _dbCon, IWebHostEnvironment hostingEnvironment)
            {
            _logger = logger;
            sessionHandler = _sessionHandler;
            dBConnection = _dbCon;
            _hostingEnvironment = hostingEnvironment;
            }

        [HttpPost]
        public async Task<IActionResult> upload_post_compliance_evidences(List<IFormFile> files)
            {
            // Directory path where files will be stored
            var uploadPath = Path.Combine(_hostingEnvironment.WebRootPath, "Audit_Evidences");

            // Ensure the directory exists
            if (!Directory.Exists(uploadPath))
                {
                Directory.CreateDirectory(uploadPath);
                }

            foreach (var file in files)
                {
                if (file.Length > 0)
                    {
                    var fileName = Path.GetFileName(file.FileName);
                    var filePath = Path.Combine(uploadPath, fileName);

                    // Save the file to the specified directory
                    using (var stream = new FileStream(filePath, FileMode.Create))
                        {
                        await file.CopyToAsync(stream);
                        }
                    }
                }

            return Ok(new { Message = "Files uploaded successfully!" });
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
        public string authorize_sub_process_by_authorizer(int T_ID, string COMMENTS)
            {
            return "{\"Status\":true,\"Message\":\"" + dBConnection.AuthorizeSubProcessByAuthorizer(T_ID, COMMENTS) + "\"}";

            }
        [HttpPost]
        public string reffered_back_sub_process_by_authorizer(int T_ID, string COMMENTS)
            {
            return "{\"Status\":true,\"Message\":\"" + dBConnection.RefferedBackSubProcessByAuthorizer(T_ID, COMMENTS) + "\"}";

            }

        [HttpPost]
        public string recommend_process_transaction_by_reviewer(int T_ID, string COMMENTS, int PROCESS_DETAIL_ID = 0, int SUB_PROCESS_ID = 0, string HEADING = "", int V_ID = 0, int CONTROL_ID = 0, int ROLE_ID = 0, int RISK_ID = 0, string ANNEX_CODE = "")
            {
            return "{\"Status\":true,\"Message\":\"" + dBConnection.RecommendProcessTransactionByReviewer(T_ID, COMMENTS, PROCESS_DETAIL_ID, SUB_PROCESS_ID, HEADING, V_ID, CONTROL_ID, ROLE_ID, RISK_ID, ANNEX_CODE) + "\"}";

            }

        [HttpPost]
        public string reffered_back_process_transaction_by_reviewer(int T_ID, string COMMENTS)
            {
            return "{\"Status\":true,\"Message\":\"" + dBConnection.RefferedBackProcessTransactionByReviewer(T_ID, COMMENTS) + "\"}";
            }
        [HttpPost]
        public string authorize_process_transaction_by_authorizer(int T_ID, string COMMENTS)
            {
            return "{\"Status\":true,\"Message\":\"" + dBConnection.AuthorizeProcessTransactionByAuthorizer(T_ID, COMMENTS) + "\"}";

            }
        [HttpPost]
        public string reffered_back_process_transaction_by_authorizer(int T_ID, string COMMENTS)
            {
            return "{\"Status\":true,\"Message\":\"" + dBConnection.RefferedBackProcessTransactionByAuthorizer(T_ID, COMMENTS) + "\"}";

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
        public string save_observations(List<ListObservationModel> LIST_OBS, int ENG_ID, int S_ID, int V_CAT_ID = 0, int V_CAT_NATURE_ID = 0, int OTHER_ENTITY_ID = 0)
            {

            string responses = "";
            foreach (ListObservationModel m in LIST_OBS)
                {
                ObservationModel ob = new ObservationModel();
                ob.HEADING = m.HEADING;
                ob.SUBCHECKLIST_ID = S_ID;
                ob.ANNEXURE_ID = m.ANNEXURE_ID;
                ob.CHECKLISTDETAIL_ID = Convert.ToInt32(m.ID.Split("obs_")[1]);
                ob.V_CAT_ID = V_CAT_ID;
                ob.V_CAT_NATURE_ID = V_CAT_NATURE_ID;
                ob.ENGPLANID = ENG_ID;
                ob.REPLYDATE = DateTime.Today.AddDays(m.DAYS);
                ob.OBSERVATION_TEXT = m.MEMO;
                ob.SEVERITY = m.RISK;
                ob.NO_OF_INSTANCES = m.NO_OF_INSTANCES;
                ob.OTHER_ENTITY_ID = OTHER_ENTITY_ID;
                ob.RESPONSIBLE_PPNO = m.RESPONSIBLE_PPNO;
                ob.AMOUNT_INVOLVED = m.AMOUNT_INVOLVED;
                ob.STATUS = 1;
                responses += dBConnection.SaveAuditObservation(ob);

                }
            return "{\"Status\":true,\"Message\":\"" + responses + "\"}";
            }
        [HttpPost]
        public string save_observations_cau(List<ListObservationModel> LIST_OBS, int ENG_ID = 0, int BRANCH_ID = 0, int SUB_CHECKLISTID = 0, int CHECKLIST_ID = 0, string ANNEXURE_ID = "")
            {
            string responses = "";
            foreach (ListObservationModel m in LIST_OBS)
                {
                ObservationModel ob = new ObservationModel();
                ob.SUBCHECKLIST_ID = SUB_CHECKLISTID;
                ob.CHECKLISTDETAIL_ID = CHECKLIST_ID;
                ob.ANNEXURE_ID = ANNEXURE_ID;
                ob.ENGPLANID = ENG_ID;
                ob.REPLYDATE = DateTime.Today.AddDays(m.DAYS);
                ob.OBSERVATION_TEXT = m.MEMO;
                ob.HEADING = m.HEADING;
                ob.SEVERITY = m.RISK;
                ob.BRANCH_ID = BRANCH_ID;
                ob.AMOUNT_INVOLVED = m.AMOUNT_INVOLVED;
                ob.NO_OF_INSTANCES = m.NO_OF_INSTANCES;
                ob.RESPONSIBLE_PPNO = m.RESPONSIBLE_PPNO;
                ob.STATUS = 1;
                responses += dBConnection.SaveAuditObservationCAU(ob);
                }
            return "{\"Status\":true,\"Message\":\"" + responses + "\"}";
            }


        [HttpPost]
        public async Task<bool> reply_observation(ObservationResponseModel or, string SUBFOLDER)
            {
            return await dBConnection.ResponseAuditObservation(or, SUBFOLDER);
            }
        [HttpPost]
        public string update_observation_text(int OBS_ID, string OBS_TEXT, int PROCESS_ID = 0, int SUBPROCESS_ID = 0, int CHECKLIST_ID = 0, string OBS_TITLE = "", int RISK_ID = 0, int ANNEXURE_ID = 0)
            {
            string response = "";
            response = dBConnection.UpdateAuditObservationText(OBS_ID, OBS_TEXT, PROCESS_ID, SUBPROCESS_ID, CHECKLIST_ID, OBS_TITLE, RISK_ID, ANNEXURE_ID);
            return "{\"Status\":true,\"Message\":\"" + response + "\"}";
            }
        [HttpPost]
        public string update_observation_status(int OBS_ID, int NEW_STATUS_ID, string DRAFT_PARA_NO, int RISK_ID, string AUDITOR_COMMENT)
            {
            string response = "";

            if (NEW_STATUS_ID == 4)
                if (RISK_ID != 3)
                    return "{\"Status\":false,\"Message\":\"Only Low Risk para can be settled by Team Lead\"}";

            response = dBConnection.UpdateAuditObservationStatus(OBS_ID, NEW_STATUS_ID, DRAFT_PARA_NO, AUDITOR_COMMENT);

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
        public List<ManageAuditParasModel> get_observations_for_manage_paras(int ENTITY_ID = 0, int OBS_ID = 0)
            {
            return dBConnection.GetObservationsForMangeAuditParas(ENTITY_ID, OBS_ID);
            }
        [HttpPost]
        public List<ManageAuditParasModel> get_observations_for_manage_paras_auth()
            {
            return dBConnection.GetObservationsForMangeAuditParasForAuthorization();
            }
        [HttpPost]
        public List<ManageAuditParasModel> get_proposed_changes_in_manage_paras_auth(int PARA_ID)
            {
            return dBConnection.GetProposedChangesInManageParasAuth(PARA_ID);
            }
        [HttpPost]
        public string update_para_for_manage_audit_paras(ManageAuditParasModel pm)
            {
            string response = "";
            response = dBConnection.UpdateAuditObservationStatus(pm);
            return "{\"Status\":true,\"Message\":\"" + response + "\"}";
            }
        [HttpPost]
        public string referredback_para_for_manage_audit_paras(ManageAuditParasModel pm)
            {
            string response = "";
            response = dBConnection.ReferredBackAuditObservationStatus(pm);
            return "{\"Status\":true,\"Message\":\"" + response + "\"}";
            }

        [HttpPost]
        public string authorize_para_for_manage_audit_paras(ManageAuditParasModel pm)
            {
            string response = "";
            response = dBConnection.AuthorizedAuditObservationStatus(pm);
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
        public List<AuditeeResponseEvidenceModel> get_responded_obs_evidences(int OBS_ID = 0)
            {
            return dBConnection.GetRespondedObservationEvidences(OBS_ID);
            }

        [HttpPost]
        public List<ObservationTextModel> get_details_for_manage_observations_text(int OBS_ID = 0, string INDICATOR = "")
            {
            return dBConnection.GetManagedAllObservationsText(OBS_ID, INDICATOR);
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
        public string add_observation_gist_and_recommendation(int OBS_ID = 0, string GIST_OF_PARA = "", string AUDITOR_RECOMMENDATION = "")
            {
            string response = "";
            response = dBConnection.AddObservationGistAndRecommendation(OBS_ID, GIST_OF_PARA, AUDITOR_RECOMMENDATION);
            return "{\"Status\":true,\"Message\":\"" + response + "\"}";
            }
        [HttpPost]
        public List<ManageObservations> get_observation_text_branches(int OBS_ID = 0)
            {
            return dBConnection.GetManagedObservationTextForBranches(OBS_ID);
            }

        [HttpPost]
        public List<ObservationResponsiblePPNOModel> get_observation_responsible_ppnos(int OBS_ID)
            {
            return dBConnection.GetObservationResponsiblePPNOs(OBS_ID);
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
        public List<ClosingDraftTeamDetailsModel> closing_draft_report_status(int ENG_ID = 0)
            {
            return dBConnection.GetClosingDraftObservations(ENG_ID);
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
        public List<LoanCaseModel> Loan_Case_Details(int Loan_case, string LOAN_TYPE = "", int ENG_ID = 0)
            {
            return dBConnection.GetLoanCaseDetails(Loan_case, LOAN_TYPE, ENG_ID);
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
        public List<GlHeadDetailsModel> GetIncomeExpenceDetails(int b_id, int ENG_ID)
            {
            return dBConnection.GetIncomeExpenceDetails(b_id, ENG_ID);
            }
        [HttpPost]
        public int GetAuditEntitiesCount(int CRITERIA_ID)
            {
            return dBConnection.GetExpectedCountOfAuditEntitiesOnCriteria(CRITERIA_ID);
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
            return dBConnection.GetAISEntities(ENTITY_ID, TYPE_ID);
            }
        [HttpPost]
        public List<AuditeeEntitiesModel> GetCBASEntities(string E_CODE, string E_NAME)
            {
            return dBConnection.GetCBASEntities(E_CODE, E_NAME);
            }
        [HttpPost]
        public List<AuditeeEntitiesModel> GetERPEntities(string E_CODE, string E_NAME)
            {
            return dBConnection.GetERPEntities(E_CODE, E_NAME);
            }
        [HttpPost]
        public List<AuditeeEntitiesModel> GetHREntities(string E_CODE, string E_NAME)
            {
            return dBConnection.GetHREntities(E_CODE, E_NAME);
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
            CAUOMAssignmentResponseModel resp = new CAUOMAssignmentResponseModel();
            foreach (CAUOMAssignmentPDPModel pdp in DAC_LIST)
                {
                resp = dBConnection.CAUOMAssignmentPDP(pdp);
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
            return dBConnection.CAUGetPreAddedOM(OM_NO, INS_YEAR);

            }

        [HttpPost]
        public List<CAUOMAssignmentModel> CAU_Get_OMs()
            {
            return dBConnection.CAUGetAssignedOMs();
            }
        [HttpPost]
        public List<ObservationSummaryModel> get_observations_summary_for_selected_entity(int ENG_ID)
            {
            return dBConnection.GetManagedObservationsSummaryForSelectedEntity(ENG_ID);

            }

        [HttpPost]
        public List<ObservationRevisedModel> get_observations_for_selected_entity(int ENG_ID)
            {
            return dBConnection.GetManagedObservationsForSelectedEntity(ENG_ID);

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
        public List<OldParasModel> get_legacy_settled_paras(int ENTITY_ID = 0)
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
        public List<LoanCasedocModel> Getloancasedocuments(int ENG_ID)
            {
            return dBConnection.GetLoanCaseDocuments(ENG_ID);
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
        public List<PreConcludingModel> get_obs_for_pre_concluding(int ENG_ID)
            {
            return dBConnection.GetEntityObservationDetails(ENG_ID);
            }

        [HttpPost]
        public List<GetOldParasBranchComplianceModel> get_paras_for_compliance_by_auditee()
            {
            return dBConnection.GetParasForComplianceByAuditee();
            }
        [HttpPost]
        public List<GetOldParasBranchComplianceModel> get_paras_for_review_compliance_by_auditee()
            {
            return dBConnection.GetParasForReviewComplianceByAuditee();
            }

        [HttpPost]
        public List<SettledPostCompliancesModel> get_settled_post_compliances_for_monitoring(string MONTH_NAME, string YEAR)
            {
            return dBConnection.GetSettledPostCompliancesForMonitoring(MONTH_NAME, YEAR);
            }
        [HttpPost]
        public List<GetOldParasBranchComplianceModel> get_old_para_br_compliance_ref()
            {
            return dBConnection.GetOldParasBranchComplianceRef();
            }

        [HttpPost]
        public GetOldParasBranchComplianceTextModel get_para_compliance_text(int OLD_PARA_ID = 0, int NEW_PARA_ID = 0, string INDICATOR = "")
            {
            return dBConnection.GetParaComplianceText(OLD_PARA_ID, NEW_PARA_ID, INDICATOR);
            }
        [HttpPost]
        public GetOldParasBranchComplianceTextModel get_old_para_compliance_cycle_text(string COM_ID, string C_CYCLE)
            {
            return dBConnection.GetOldParasComplianceCycleText(COM_ID, C_CYCLE);
            }
        [HttpPost]
        public AuditeeResponseEvidenceModel get_post_compliance_evidence_data(string FILE_ID)
            {
            return dBConnection.GetPostComplianceEvidenceData(FILE_ID);
            }
        [HttpPost]
        public AuditeeResponseEvidenceModel get_cau_paras_post_compliance_evidence_data(string FILE_ID)
            {
            return dBConnection.GetCAUParasPostComplianceEvidenceData(FILE_ID);
            }

        [HttpPost]
        public AuditeeResponseEvidenceModel get_auditee_evidence_data(string FILE_ID)
            {
            return dBConnection.GetAuditeeEvidenceData(FILE_ID);
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
            return dBConnection.GetOldParasBranchComplianceTextForImpIncharge(PID, REF_P, PARA_CATEGORY, REPLY_DATE, OBS_ID);
            }

        [HttpPost]
        public GetOldParasBranchComplianceTextModel get_old_para_imp_text_ref(int PID, string REF_P, string PARA_CATEGORY, string REPLY_DATE, string OBS_ID)
            {
            return dBConnection.GetOldParasReferredBackBranchComplianceTextForImpIncharge(PID, REF_P, PARA_CATEGORY, REPLY_DATE, OBS_ID);
            }
        [HttpPost]
        public GetOldParasBranchComplianceTextModel get_old_para_head_az_text(int PID, string REF_P, string OBS_ID, string PARA_CATEGORY, string REPLY_DATE)
            {
            return dBConnection.GetOldParasBranchComplianceTextForHeadAZ(PID, REF_P, OBS_ID, PARA_CATEGORY, REPLY_DATE);
            }


        [HttpPost]
        public async Task<string> submit_post_audit_compliance(string OLD_PARA_ID, int NEW_PARA_ID, string INDICATOR, string COMPLIANCE, string COMMENTS, List<AuditeeResponseEvidenceModel> EVIDENCE_LIST, string SUBFOLDER)
            {
            string response = await dBConnection.SubmitPostAuditCompliance(OLD_PARA_ID, NEW_PARA_ID, INDICATOR, COMPLIANCE, COMMENTS, EVIDENCE_LIST, SUBFOLDER);
            return "{\"Status\":true,\"Message\":\"" + response + "\"}";
            }

        [HttpPost]
        public string submit_post_audit_compliance_review(string OLD_PARA_ID, int NEW_PARA_ID, string INDICATOR, string COMPLIANCE, string COMMENTS, List<AuditeeResponseEvidenceModel> EVIDENCE_LIST)
            {
            string response = "";
            response = dBConnection.SubmitPostAuditComplianceReview(OLD_PARA_ID, NEW_PARA_ID, INDICATOR, COMPLIANCE, COMMENTS, EVIDENCE_LIST);
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
        public string submit_old_para_br_compliance_status_partially_settle(string OBS_ID, string REFID, string REMARKS, int NEW_STATUS, string PARA_CAT, string SETTLE_INDICATOR, List<ObservationResponsiblePPNOModel> RESPONSIBLES_ARR, string SEQUENCE, string AUDITED_BY, string PARA_TEXT)
            {
            string response = "";
            response = dBConnection.AddOldParasStatusPartiallySettle(OBS_ID, REFID, REMARKS, NEW_STATUS, PARA_CAT, SETTLE_INDICATOR, RESPONSIBLES_ARR, SEQUENCE, AUDITED_BY, PARA_TEXT);
            return "{\"Status\":true,\"Message\":\"" + response + "\"}";
            }

        [HttpPost]
        public List<GetOldParasForFinalSettlement> get_old_para_br_compliance_head()
            {
            return dBConnection.GetOldParasForFinalSettlement();
            }

        [HttpPost]
        public string submit_old_para_compliance_head_status(int PARA_ID, string REMARKS, int NEW_STATUS, string PARA_REF, string PARA_INDICATOR, string PARA_CATEGORY, int AU_OBS_ID, string SEQUENCE, string AUDITED_BY, string ENTITY_ID)
            {
            string response = "";
            response = dBConnection.AddOldParasheadStatusUpdate(PARA_ID, REMARKS, NEW_STATUS, PARA_REF, PARA_INDICATOR, PARA_CATEGORY, AU_OBS_ID, SEQUENCE, AUDITED_BY, ENTITY_ID);
            return "{\"Status\":true,\"Message\":\"" + response + "\"}";
            }

        [HttpPost]
        public List<BranchModel> get_zone_Branches(int ZONEID)
            {
            return dBConnection.GetZoneBranches(ZONEID, false);

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
        public string get_all_para_text(string REF_P, string OBS_ID, string PARA_CATEGORY)
            {
            return dBConnection.GetAllParaText(REF_P, OBS_ID, PARA_CATEGORY);
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
            response = dBConnection.GetUserName(PPNUMBER);
            return "{\"Status\":true,\"Message\":\"" + response + "\"}";
            }
        [HttpPost]
        public string Add_Old_Para_Change_status(string REFID, string OBS_ID, string INDICATOR, int NEW_STATUS, string REMARKS)
            {
            string response = "";
            response = dBConnection.AddChangeStatusRequestForSettledPara(REFID, OBS_ID, INDICATOR, NEW_STATUS, REMARKS);
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
        public string Add_Authorization_Old_Para_Change_status(string REFID, string OBS_ID, string INDICATOR)
            {
            string response = "";
            response = dBConnection.AddAuthorizeChangeStatusRequestForSettledPara(REFID, OBS_ID, INDICATOR);
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
        public string update_audit_para_for_finalization(int OBS_ID, string ANNEX_ID, string PROCESS_ID, int SUB_PROCESS_ID, int PROCESS_DETAIL_ID, int RISK_ID, string GIST_OF_PARA, string TEXT_PARA, string AMOUNT_INV, string NO_INST)
            {
            string response = "";
            response = dBConnection.UpdateAuditParaForFinalization(OBS_ID, ANNEX_ID, PROCESS_ID, SUB_PROCESS_ID, PROCESS_DETAIL_ID, RISK_ID, GIST_OF_PARA, TEXT_PARA, AMOUNT_INV, NO_INST);
            return "{\"Status\":true,\"Message\":\"" + response + "\"}";
            }

        [HttpPost]
        public string update_audit_para_for_finalization_ho(int OBS_ID, string VIOLATION_ID, int VIOLATION_NATURE_ID, int RISK_ID, string GIST_OF_PARA, string TEXT_PARA)
            {
            string response = "";
            response = dBConnection.UpdateAuditParaForFinalizationHO(OBS_ID, VIOLATION_ID, VIOLATION_NATURE_ID, RISK_ID, GIST_OF_PARA, TEXT_PARA);
            return "{\"Status\":true,\"Message\":\"" + response + "\"}";
            }
        [HttpPost]
        public List<OldParasModel> get_legacy_paras_for_update(int ENTITY_ID, string PARA_REF, int PARA_ID = 0)
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
        public UserModel get_employee_name_from_pp(int PP_NO)
            {
            return dBConnection.GetEmployeeNameFromPPNO(PP_NO);

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
        public List<AuditEntitiesModel> get_auditee_entities_by_entity_type_id(int ENTITY_TYPE_ID)
            {
            return dBConnection.GetAuditEntitiesByTypeId(ENTITY_TYPE_ID);
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
            return dBConnection.GetLegacyParasEntitiesReport(ENTITY_ID);
            }

        [HttpPost]
        public string settle_legacy_para_HO(int NEW_STATUS, string PARA_REF, string SETTLEMENT_NOTES)
            {
            return "{\"Status\":true,\"Message\":\"" + dBConnection.SettleLegacyParaHO(NEW_STATUS, PARA_REF, SETTLEMENT_NOTES) + "\"}";
            }
        [HttpPost]
        public string delete_legacy_para_HO(string PARA_REF)
            {
            return "{\"Status\":true,\"Message\":\"" + dBConnection.DeleteLegacyParaHO(PARA_REF) + "\"}";
            }

        [HttpPost]
        public List<FADNewOldParaPerformanceModel> get_relation_legacy_observation_for_dashboard(int ENTITY_ID = 0)
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
        public List<FunctionalResponsibilitiesWiseParasModel> get_functional_responsibility_wise_paras_for_dashboard(int FUNCTIONAL_ENTITY_ID = 0)
            {
            return dBConnection.GetFunctionalResponsibilityWiseParaForDashboard(FUNCTIONAL_ENTITY_ID);
            }
        [HttpPost]
        public List<FADNewOldParaPerformanceModel> get_functional_responsibility_wise_paras_for_dashboard_ho(int PROCESS_ID = 0, int SUB_PROCESS_ID = 0, int PROCESS_DETAIL_ID = 0, int FUNCTIONAL_ENTITY_ID = 0, int DEPT_ID = 0)
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
        public List<AuditPerformanceChartDashboardModel> get_audit_performance_chart_for_dashboard()
            {
            return dBConnection.GetAuditPerformanceChartForDashboard();
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
        public string add_audit_sub_checklist(int PROCESS_ID = 0, int ENTITY_TYPE_ID = 0, string HEADING = "", string RISK_SEQUENCE = "", string RISK_WEIGHTAGE = "")
            {
            return "{\"Status\":true,\"Message\":\"" + dBConnection.AddAuditSubChecklist(PROCESS_ID, ENTITY_TYPE_ID, HEADING, RISK_SEQUENCE, RISK_WEIGHTAGE) + "\"}";
            }
        [HttpPost]
        public string update_audit_sub_checklist(int PROCESS_ID = 0, int OLD_PROCESS_ID = 0, int SUB_PROCESS_ID = 0, string HEADING = "", int ENTITY_TYPE_ID = 0, string RISK_SEQUENCE = "", string RISK_WEIGHTAGE = "")
            {
            return "{\"Status\":true,\"Message\":\"" + dBConnection.UpdateAuditSubChecklist(PROCESS_ID, OLD_PROCESS_ID, SUB_PROCESS_ID, HEADING, ENTITY_TYPE_ID, RISK_SEQUENCE, RISK_WEIGHTAGE) + "\"}";
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
        public List<AuditChecklistDetailsModel> get_audit_checklist_detail_for_remove_duplicate(int SUB_PROCESS_ID = 0)
            {
            return dBConnection.GetAuditChecklistDetailForRemoveDuplicate(SUB_PROCESS_ID);
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
        public string add_audit_checklist_detail(int PROCESS_ID = 0, int SUB_PROCESS_ID = 0, string HEADING = "", int V_ID = 0, int CONTROL_ID = 0, int ROLE_ID = 0, int RISK_ID = 0, string ANNEX_CODE = "")
            {
            return "{\"Status\":true,\"Message\":\"" + dBConnection.AddAuditChecklistDetail(PROCESS_ID, SUB_PROCESS_ID, HEADING, V_ID, CONTROL_ID, ROLE_ID, RISK_ID, ANNEX_CODE) + "\"}";
            }
        [HttpPost]
        public string update_audit_checklist_detail(int PROCESS_DETAIL_ID = 0, int PROCESS_ID = 0, int SUB_PROCESS_ID = 0, string HEADING = "", int V_ID = 0, int CONTROL_ID = 0, int ROLE_ID = 0, int RISK_ID = 0, string ANNEX_CODE = "")
            {
            return "{\"Status\":true,\"Message\":\"" + dBConnection.UpdateAuditChecklistDetail(PROCESS_DETAIL_ID, PROCESS_ID, SUB_PROCESS_ID, HEADING, V_ID, CONTROL_ID, ROLE_ID, RISK_ID, ANNEX_CODE) + "\"}";
            }

        [HttpPost]
        public List<ParaPositionReportModel> get_para_position_report(int P_ID = 0, int C_ID = 0)
            {
            return dBConnection.GetParaPositionReport(P_ID, C_ID);
            }

        [HttpPost]
        public List<RepetativeParaModel> get_repetative_paras_for_dashboard(int P_ID = 0, int SP_ID = 0, int PD_ID = 0)
            {
            return dBConnection.GetRepetativeParaForDashboard(P_ID, SP_ID, PD_ID);
            }

        [HttpPost]
        public string add_audit_checklist(string HEADING = "", int ENTITY_TYPE_ID = 0, string RISK_SEQUENCE = "", string RISK_WEIGHTAGE = "")
            {
            return "{\"Status\":true,\"Message\":\"" + dBConnection.AddAuditChecklist(HEADING, ENTITY_TYPE_ID, RISK_SEQUENCE, RISK_WEIGHTAGE) + "\"}";
            }

        [HttpPost]
        public string update_audit_checklist(int PROCESS_ID = 0, string HEADING = "", string ACTIVE = "", string RISK_SEQUENCE = "", string RISK_WEIGHTAGE = "")
            {
            return "{\"Status\":true,\"Message\":\"" + dBConnection.UpdateAuditChecklist(PROCESS_ID, HEADING, ACTIVE, RISK_SEQUENCE, RISK_WEIGHTAGE) + "\"}";
            }

        [HttpPost]
        public List<AuditeeEntitiesModel> get_entities_parent_ent_type_id(int ENTITY_TYPE_ID = 0)
            {
            return dBConnection.GetEntitiesByParentEntityTypeId(ENTITY_TYPE_ID);
            }

        [HttpPost]
        public List<ParaPositionDetailsModel> get_para_position_details(int ENTITY_ID = 0, int AUDIT_PERIOD = 0)
            {
            return dBConnection.GetParaPositionParaDetails(ENTITY_ID, AUDIT_PERIOD);
            }
        [HttpPost]
        public List<ObservationStatusReversalModel> get_engagement_status_for_reversal(int ENG_ID = 0)
            {
            return dBConnection.GetEngagementReversalStatus(ENG_ID);

            }
        [HttpPost]
        public List<ObservationReversalModel> get_engagements_details_for_status_reversal(int ENTITY_ID = 0)
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
        public List<PostComplianceHistoryModel> get_compliance_history(string COM_ID)
            {
            return dBConnection.GetComplianceHistory(COM_ID);

            }

        [HttpPost]
        public List<ComplianceHistoryModel> get_settled_para_compliance_history(string REF_P, string OBS_ID)
            {
            return dBConnection.GetSettledParaComplianceHistory(REF_P, OBS_ID);

            }

        [HttpPost]
        public string get_new_para_text(string OBS_ID)
            {
            return dBConnection.GetNewParaText(OBS_ID);

            }
        [HttpPost]
        public List<RiskProcessDefinition> get_violation_area_for_functional_responsibility_wise_paras(int FUNCTIONAL_ENTITY_ID = 0)
            {
            return dBConnection.GetViolationListForDashboard(FUNCTIONAL_ENTITY_ID);

            }
        [HttpPost]
        public List<RiskProcessDefinition> get_sub_violation_area_for_functional_responsibility_wise_paras(int FUNCTIONAL_ENTITY_ID = 0, int PROCESS_ID = 0)
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
        public List<FADNewOldParaPerformanceModel> get_total_para_details_ho(int ENTITY_ID = 0)
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
            return dBConnection.GetAuditeeRiskForEntTypes(ENT_TYPE_ID, PERIOD);

            }

        [HttpPost]
        public List<AuditeeRiskModeldetails> get_auditee_risk_details(int ENG_ID)
            {
            return dBConnection.GetAuditeeRiskDetails(ENG_ID);

            }
        [HttpPost]
        public string add_new_user(FindUserModel user)
            {
            return "{\"Status\":true,\"Message\":\"" + dBConnection.AddNewUser(user) + "\"}";

            }

        [HttpPost]
        public List<RoleActivityLogModel> get_role_activity_log(int ROLE_ID, int DEPT_ID, int AZ_ID)
            {
            return dBConnection.GetRoleActivityLog(ROLE_ID, DEPT_ID, AZ_ID);

            }
        [HttpPost]
        public List<RoleActivityLogModel> get_user_activity_log(int PP_NO)
            {
            return dBConnection.GetUserActivityLog(PP_NO);

            }

        #region BAC API CALLS

        [HttpPost]
        public List<BACAgendaModel> get_bac_agenda(int MEETING_NO)
            {
            return dBConnection.GetBACAgenda(MEETING_NO);

            }
        [HttpPost]
        public List<BACAgendaModel> get_bac_meeting_summary(int MEETING_NO)
            {
            return dBConnection.GetBACAMeetingSummary(MEETING_NO);

            }

        [HttpPost]
        public List<BACAgendaActionablesSummaryModel> get_bac_agenda_actionables_consolidate_summary()
            {
            return dBConnection.GetBACAgendaActionablesConsolidatedSummary();
            }
        [HttpPost]
        public List<BACAgendaActionablesSummaryModel> get_bac_agenda_actionables_summary()
            {
            return dBConnection.GetBACAgendaActionablesSummary();
            }

        [HttpPost]
        public List<BACAgendaActionablesModel> get_bac_agenda_actionables(string STATUS)
            {
            return dBConnection.GetBACAgendaActionables(STATUS);

            }
        [HttpPost]
        public List<BACAgendaActionablesModel> get_bac_agenda_actionables_meeting_no(string STATUS, string MEETING_NO)
            {
            return dBConnection.GetBACAgendaActionablesWithMeetingNo(STATUS, MEETING_NO);

            }
        [HttpPost]
        public List<BACCIAAnalysisModel> get_bac_analysis(int PROCESS_ID)
            {
            return dBConnection.GetBACCIAAnalysis(PROCESS_ID);

            }
        #endregion
        [HttpPost]
        public List<EntityWiseObservationModel> get_reporting_wise_observations()
            {
            return dBConnection.GetReportingOfficeWiseObservations();

            }

        [HttpPost]
        public List<EntityWiseObservationModel> get_entity_wise_observations()
            {
            return dBConnection.GetEntityWiseObservations();
            }

        [HttpPost]
        public List<AnnexWiseObservationModel> get_annex_wise_observations()
            {
            return dBConnection.GetAnnexureWiseObservations();

            }
        [HttpPost]
        public List<FunctionalAnnexureWiseObservationModel> get_entity_wise_observation_detail(int ENTITY_ID)
            {
            return dBConnection.GetEntityWiseObservationDetail(ENTITY_ID);

            }

        [HttpPost]
        public List<FunctionalAnnexureWiseObservationModel> get_functional_observations(int ANNEX_ID, int ENTITY_ID)
            {
            return dBConnection.GetFunctionalObservations(ANNEX_ID, ENTITY_ID);

            }
        [HttpPost]
        public string get_functional_observation_text(int PARA_ID, string PARA_CATEGORY)
            {
            return dBConnection.GetFunctionalObservationText(PARA_ID, PARA_CATEGORY);

            }

        [HttpPost]
        public List<FunctionalAnnexureWiseObservationModel> get_analysis_detail_paras(int PROCESS_ID)
            {
            return dBConnection.GetAnalysisDetailPara(PROCESS_ID);

            }
        [HttpPost]
        public List<FunctionalAnnexureWiseObservationModel> get_functional_resp_detail_paras(int PROCESS_ID)
            {
            return dBConnection.GetFunctionalRespDetailPara(PROCESS_ID);

            }

        [HttpPost]
        public List<FunctionalAnnexureWiseObservationModel> get_analysis_summary_paras(int PROCESS_ID)
            {
            return dBConnection.GetAnalysisSummaryPara(PROCESS_ID);

            }


        [HttpPost]
        public List<FunctionalAnnexureWiseObservationModel> get_functional_resp_summary_paras(int PROCESS_ID)
            {
            return dBConnection.GetFunctionalRespSummaryPara(PROCESS_ID);

            }

        [HttpPost]
        public List<BranchModel> get_zone_Branches_for_Annexure_Assignment(int ENTITY_ID)
            {
            return dBConnection.GetZoneBranchesForAnnexureAssignment(ENTITY_ID);

            }

        [HttpPost]
        public List<AllParaForAnnexureAssignmentModel> get_all_paras_for_annexure_assignment(int ENTITY_ID)
            {
            return dBConnection.GetAllParasForAnnexureAssignment(ENTITY_ID);
            }
        [HttpPost]
        public string assign_annexure_with_para(string OBS_ID, string REF_P, string ANNEX_ID, string PARA_CATEGORY)
            {
            return "{\"Status\":true,\"Message\":\"" + dBConnection.AssignAnnexureWithPara(OBS_ID, REF_P, ANNEX_ID, PARA_CATEGORY) + "\"}";
            }

        [HttpPost]
        public string merge_duplicate_process(string MAIN_PROCESS_ID, List<string> MERGE_PROCESS_IDs)
            {
            string resp = "";
            foreach (string ID in MERGE_PROCESS_IDs)
                {
                resp += dBConnection.MergeDuplicateProcesses(MAIN_PROCESS_ID, ID) + "</br>";
                }
            return "{\"Status\":true,\"Message\":\"" + resp + "\"}";

            }

        [HttpPost]
        public string merge_duplicate_sub_process(string MAIN_PROCESS_ID, string MAIN_SUB_PROCESS_ID, List<string> MERGE_SUB_PROCESS_IDs)
            {
            string resp = "";
            foreach (string ID in MERGE_SUB_PROCESS_IDs)
                {
                resp += dBConnection.MergeDuplicateSubProcesses(MAIN_PROCESS_ID, MAIN_SUB_PROCESS_ID, ID) + "</br>";
                }
            return "{\"Status\":true,\"Message\":\"" + resp + "\"}";

            }


        [HttpPost]
        public string merge_duplicate_checklists(string MAIN_CHECKLIST_ID, List<string> MERGE_CHECKLIST_IDs)
            {
            foreach (string ID in MERGE_CHECKLIST_IDs)
                {
                dBConnection.MergeDuplicateChecklists(MAIN_CHECKLIST_ID, ID);
                }
            return "{\"Status\":true,\"Message\":\"Duplicates merged successfully\"}";

            }

        [HttpPost]
        public List<MergeDuplicateProcessModel> get_duplicate_Processes(int PROCESS_ID)
            {
            return dBConnection.GetDuplicateProcesses(PROCESS_ID);
            }
        [HttpPost]
        public List<MergeDuplicateProcessModel> get_duplicate_Sub_Processes(int SUB_PROCESS_ID)
            {
            return dBConnection.GetDuplicateSubProcesses(SUB_PROCESS_ID);
            }


        [HttpPost]
        public List<MergeDuplicateChecklistModel> get_duplicate_checklists(int PROCESS_ID)
            {
            return dBConnection.GetDuplicateChecklists(PROCESS_ID);
            }
        [HttpPost]
        public MergeDuplicateChecklistModel get_duplicate_checklists_count(int PROCESS_ID)
            {
            return dBConnection.GetDuplicateChecklistsCount(PROCESS_ID);
            }
        [HttpPost]
        public string authorize_merge_duplicate_process(int PROCESS_ID, List<int> AUTH_P_IDS)
            {
            string resp = "";
            foreach (int ID in AUTH_P_IDS)
                {
                resp += dBConnection.AuthorizeMergeDuplicateProcesses(PROCESS_ID, ID) + "</br>";
                }
            return "{\"Status\":true,\"Message\":\"" + resp + "\"}";

            }
        [HttpPost]
        public string authorize_merge_duplicate_sub_process(int SUB_PROCESS_ID, List<int> AUTH_S_P_IDS)
            {
            string resp = "";
            foreach (int ID in AUTH_S_P_IDS)
                {
                resp += dBConnection.AuthorizeMergeDuplicateSubProcesses(SUB_PROCESS_ID, ID) + "</br>";
                }
            return "{\"Status\":true,\"Message\":\"" + resp + "\"}";

            }
        [HttpPost]
        public string authorize_merge_duplicate_checklists(int PROCESS_ID)
            {
            return "{\"Status\":true,\"Message\":\"" + dBConnection.AuthorizeMergeDuplicateChecklists(PROCESS_ID) + "\"}";
            }

        [HttpPost]
        public string update_observation_status_for_reversal(List<int> OBS_IDS, int NEW_STATUS_ID, int ENG_ID)
            {
            string resp = "";
            foreach (int ID in OBS_IDS)
                {
                resp += dBConnection.UpdateObservationStatusForReversal(ID, NEW_STATUS_ID, ENG_ID) + "<br />";
                }
            return "{\"Status\":true,\"Message\":\"" + resp + "\"}";
            }

        [HttpPost]
        public List<SettledParasMonitoringModel> get_settled_paras_for_monitoring(int ENTITY_ID)
            {
            return dBConnection.GetSettledParasForMonitoring(ENTITY_ID);
            }
        [HttpPost]
        public string submit_settled_para_compliance_comments(string REF_P, string OBS_ID, string COMMENTS)
            {
            return "{\"Status\":true,\"Message\":\"" + dBConnection.SaveSettledParaCompliacne(REF_P, OBS_ID, COMMENTS) + "\"}";

            }

        [HttpPost]
        public List<StatusWiseComplianceModel> get_status_wise_compliance(string AUDITEE_ID, string START_DATE, string END_DATE, string RELATION_CHECK)
            {
            return dBConnection.GetStatusWiseCompliance(AUDITEE_ID, START_DATE, END_DATE, RELATION_CHECK);
            }
        [HttpPost]
        public List<AdminNewUsersAIS> admin_get_new_users()
            {
            return dBConnection.AdminNewUsersInAIS();
            }

        [HttpPost]
        public List<AuditParaReconsillation> get_audit_para_reconsillation()
            {
            return dBConnection.GetAuditParaRensillation();
            }
        [HttpPost]
        public List<HREntitiesModel> get_hr_entities_for_admin_panel_entity_addition(string ENTITY_NAME, string ENTITY_CODE)
            {
            return dBConnection.GetHREntitiesForAdminPanelEntityAddition(ENTITY_NAME, ENTITY_CODE);
            }


        [HttpPost]
        public List<AISEntitiesModel> get_ais_entities_for_admin_panel_entity_addition(string ENTITY_NAME, string ENTITY_CODE, int ENT_TYPE_ID = 0)
            {
            return dBConnection.GetAISEntitiesForAdminPanelEntityAddition(ENTITY_NAME, ENTITY_CODE, ENT_TYPE_ID);
            }


        [HttpPost]
        public string update_ais_entity_for_admin_panel_entity_addition(string ENTITY_ID, string ENTITY_NAME, string ENTITY_CODE, string AUDITABLE, string AUDIT_BY_ID, string ENTITY_TYPE_ID, string ENT_DESC, string STATUS)
            {
            return "{\"Status\":true,\"Message\":\"" + dBConnection.UpdateAISEntityForAdminPanelEntityAddition(ENTITY_ID, ENTITY_NAME, ENTITY_CODE, AUDITABLE, AUDIT_BY_ID, ENTITY_TYPE_ID, ENT_DESC, STATUS) + "\"}";

            }

        [HttpPost]
        public string add_ais_entity_for_admin_panel_entity_addition(string ENTITY_NAME, string ENTITY_CODE, string AUDITABLE, string AUDIT_BY_ID, string ENTITY_TYPE_ID, string ENT_DESC, string STATUS)
            {
            return "{\"Status\":true,\"Message\":\"" + dBConnection.AddAISEntityForAdminPanelEntityAddition(ENTITY_NAME, ENTITY_CODE, AUDITABLE, AUDIT_BY_ID, ENTITY_TYPE_ID, ENT_DESC, STATUS) + "\"}";

            }

        [HttpPost]
        public List<EntityMappingForEntityAddition> get_ais_entity_existing_mapping_for_admin_panel_entity_addition(string ENTITY_ID)
            {
            return dBConnection.GetAISEntityMappingForAdminPanelEntityAddition(ENTITY_ID);

            }

        [HttpPost]
        public string add_ais_entity_mapping_for_admin_panel_entity_addition(string P_ENTITY_ID, string ENTITY_ID, string RELATION_TYPE_ID)
            {
            return "{\"Status\":true,\"Message\":\"" + dBConnection.AddAISEntityMappingForAdminPanelEntityAddition(P_ENTITY_ID, ENTITY_ID, RELATION_TYPE_ID) + "\"}";

            }


        [HttpPost]
        public string update_ais_entity_mapping_for_admin_panel_entity_addition(string P_ENTITY_ID, string ENTITY_ID, string RELATION_TYPE_ID)
            {
            return "{\"Status\":true,\"Message\":\"" + dBConnection.UpdateAISEntityMappingForAdminPanelEntityAddition(P_ENTITY_ID, ENTITY_ID, RELATION_TYPE_ID) + "\"}";

            }



        [HttpPost]
        public List<AuditPlanEngDetailReport> get_audit_plan_engagement_detailed_report(string AUDITED_BY, string PERIOD_ID)
            {
            return dBConnection.GetAuditPlanEngagementDetailedReport(AUDITED_BY, PERIOD_ID);

            }

        [HttpPost]
        public List<LoanCaseFileDetailsModel> Get_Working_Paper_Loan_Cases(string ENGID)
            {
            return dBConnection.GetWorkingPaperLoanCases(ENGID);

            }

        [HttpPost]
        public string Add_Working_Paper_Loan_Cases(string ENGID, string LCNUMBER, string LCAMOUNT, DateTime DISBDATE, string LCAT, string OBS, string PARA_NO)
            {
            return "{\"Status\":true,\"Message\":\"" + dBConnection.AddWorkingPaperLoanCases(ENGID, LCNUMBER, LCAMOUNT, DISBDATE, LCAT, OBS, PARA_NO) + "\"}";

            }

        [HttpPost]
        public List<VoucherCheckingDetailsModel> Get_Working_Paper_Voucher_Checking(string ENGID)
            {
            return dBConnection.GetWorkingPaperVoucherChecking(ENGID);

            }

        [HttpPost]
        public string Add_Working_Paper_Voucher_Checking(string ENGID, string VNUMBER, string OBS, string PARA_NO)
            {
            return "{\"Status\":true,\"Message\":\"" + dBConnection.AddWorkingVoucherChecking(ENGID, VNUMBER, OBS, PARA_NO) + "\"}";

            }
        [HttpPost]
        public List<AccountOpeningDetailsModel> Get_Working_Paper_Account_Opening(string ENGID)
            {
            return dBConnection.GetWorkingPaperAccountOpening(ENGID);

            }

        [HttpPost]
        public string Add_Working_Paper_Account_Opening(string ENGID, string VNUMBER, string ANATURE, string OBS, string PARA_NO)
            {
            return "{\"Status\":true,\"Message\":\"" + dBConnection.AddWorkingAccountOpening(ENGID, VNUMBER, ANATURE, OBS, PARA_NO) + "\"}";

            }

        [HttpPost]
        public List<FixedAssetsDetailsModel> Get_Working_Paper_Fixed_Assets(string ENGID)
            {
            return dBConnection.GetWorkingPaperFixedAssets(ENGID);

            }

        [HttpPost]
        public string Add_Working_Paper_Fixed_Assets(string ENGID, string A_NAME, string PHY_EX, string FAR, string DIFF, string REM)
            {
            return "{\"Status\":true,\"Message\":\"" + dBConnection.AddWorkingFixedAssets(ENGID, A_NAME, PHY_EX, FAR, DIFF, REM) + "\"}";

            }


        [HttpPost]
        public List<CashCountDetailsModel> Get_Working_Paper_Cash_Counter(string ENGID)
            {
            return dBConnection.GetWorkingPaperCashCounter(ENGID);

            }

        [HttpPost]
        public string Add_Working_Paper_Cash_Counter(string ENGID, string DVAULT, string NOVAULT, string TOTVAULT, string DSR, string NOSR, string TOTSR, string DIFF)
            {
            return "{\"Status\":true,\"Message\":\"" + dBConnection.AddWorkingCashCounter(ENGID, DVAULT, NOVAULT, TOTVAULT, DSR, NOSR, TOTSR, DIFF) + "\"}";

            }
        [HttpPost]
        public List<AnnexureExerciseStatus> Get_Annexure_Exercise_Status()
            {
            return dBConnection.GetAnnexureExerciseStatus();

            }

        [HttpPost]
        public string update_new_user_admin_panel(List<int> PPNOArr)
            {
            string resp = "";
            foreach (int ppno in PPNOArr)
                {
                resp = dBConnection.UpdateNewUsersAdminPanel(ppno);
                }

            return "{\"Status\":true,\"Message\":\"" + resp + "\"}";
            }

        [HttpPost]
        public List<UserRoleDetailAdminPanelModel> admin_get_user_details(string DESIGNATION_CODE)
            {
            return dBConnection.GetUserDetailAdminPanel(DESIGNATION_CODE);
            }
        [HttpPost]
        public List<ComplianceSummaryModel> get_compliance_summary(int ENTITY_ID)
            {
            return dBConnection.GetComplianceSummary(ENTITY_ID);
            }
        [HttpPost]
        public List<EntitiesShiftingDetailsModel> get_entity_shifting_details(string ENTITY_ID = "")
            {
            return dBConnection.GetEntityShiftingDetails(ENTITY_ID);
            }
        [HttpPost]
        public List<AuditEntitiesModel> get_entity_types()
            {
            return dBConnection.GetEntityTypes();
            }

        [HttpPost]
        public string update_entity_types(AuditEntitiesModel ENTITY_MODEL)
            {
            return "{\"Status\":true,\"Message\":\"" + dBConnection.UpdateEntityTypes(ENTITY_MODEL) + "\"}";
            }
        [HttpPost]
        public List<AuditEntityRelationsModel> get_entity_relations()
            {
            return dBConnection.GetEntityRelations();
            }
        [HttpPost]
        public List<EntitiesMappingModel> get_entities_mapping(string ENT_ID, string P_TYPE, string C_TYPE, string RELATION_TYPE, string IND)
            {
            return dBConnection.GetEntitiesMapping(ENT_ID, P_TYPE, C_TYPE, RELATION_TYPE, IND);
            }
        [HttpPost]
        public List<EntitiesMappingModel> get_entities_mapping_reporting(string ENT_ID, string P_TYPE, string C_TYPE, string RELATION_TYPE, string IND)
            {
            return dBConnection.GetEntitiesMappingReporting(ENT_ID, P_TYPE, C_TYPE, RELATION_TYPE, IND);
            }
        [HttpPost]
        public List<EntitiesMappingModel> get_entities_of_parent_child(string P_TYPE_ID, string C_TYPE_ID)
            {
            return dBConnection.GetParentChildEntities(P_TYPE_ID, C_TYPE_ID);
            }
        [HttpPost]
        public string submit_entity_shifting_from_admin_panel(string FROM_ENT_ID, string TO_ENT_ID, string CIR_REF, DateTime CIR_DATE, string CIR)
            {
            return "{\"Status\":true,\"Message\":\"" + dBConnection.SubmitEntityShiftingFromAdminPanel(FROM_ENT_ID, TO_ENT_ID, CIR_REF, CIR_DATE, CIR) + "\"}";
            }
        [HttpPost]
        public string submit_entity_conv_to_islamic_from_admin_panel(string FROM_ENT_ID, string TO_ENT_ID)
            {
            return "{\"Status\":true,\"Message\":\"" + dBConnection.SubmitEntityConvToIslamicFromAdminPanel(FROM_ENT_ID, TO_ENT_ID) + "\"}";
            }
        [HttpPost]
        public List<GroupWiseUsersCountModel> get_group_wise_users_count()
            {
            return dBConnection.GetGroupWiseUsersCount();
            }
        [HttpPost]
        public List<GroupWisePagesModel> get_group_wise_pages(string GROUP_ID)
            {
            return dBConnection.GetGroupWisePages(GROUP_ID);
            }

        [HttpPost]
        public string add_compliance_flow(string ID, string ENTITY_TYPE_ID, string GROUP_ID, string PREV_GROUP_ID, string NEXT_GROUP_ID, string COMP_UP_STATUS, string COMP_DOWN_STATUS)
            {
            return "{\"Status\":true,\"Message\":\"" + dBConnection.AddComplianceFlow(ID, ENTITY_TYPE_ID, GROUP_ID, PREV_GROUP_ID, NEXT_GROUP_ID, COMP_UP_STATUS, COMP_DOWN_STATUS) + "\"}";
            }

        [HttpPost]
        public string update_compliance_flow(string ID, string ENTITY_TYPE_ID, string GROUP_ID, string PREV_GROUP_ID, string NEXT_GROUP_ID)
            {
            return "{\"Status\":true,\"Message\":\"" + dBConnection.UpdateComplianceFlow(ID, ENTITY_TYPE_ID, GROUP_ID, PREV_GROUP_ID, NEXT_GROUP_ID) + "\"}";
            }
        [HttpPost]
        public List<ComplianceFlowModel> get_compliance_flow_by_entity_type(int ENTITY_TYPE_ID = 0, int GROUP_ID = 0)
            {
            return dBConnection.GetComplianceFlowByEntityType(ENTITY_TYPE_ID, GROUP_ID);
            }


        [HttpPost]
        public List<DepttWiseOutstandingParasModel> get_outstanding_paras_for_entity_type_id(string ENTITY_TYPE_ID)
            {
            return dBConnection.GetOutstandingParasForEntityTypeId(ENTITY_TYPE_ID);
            }

        [HttpPost]
        public List<AuditTeamModel> get_team_memeber_details_for_post_changes_team_eng_reversal(int AUDITED_BY_DEPT)
            {
            return dBConnection.GetAuditTeams(0, AUDITED_BY_DEPT);
            }

        [HttpPost]
        public string submit_new_team_id_for_post_changes_team_eng_reversal(int TEAM_ID, int ENG_ID, int AUDITED_BY_ID, string TEAM_NAME)
            {
            return "{\"Status\":true,\"Message\":\"" + dBConnection.SubmitNewTeamIdForPostChangesTeamEngReversal(TEAM_ID, ENG_ID, AUDITED_BY_ID, TEAM_NAME) + "\"}";
            }

        [HttpPost]
        public string audit_engagement_status_reversal(int ENG_ID, int NEW_STATUS_ID, int PLAN_ID, string COMMENTS)
            {
            return "{\"Status\":true,\"Message\":\"" + dBConnection.AuditEngagementStatusReversal(ENG_ID, NEW_STATUS_ID, PLAN_ID, COMMENTS) + "\"}";
            }
        [HttpPost]
        public string audit_engagement_obs_status_reversal(int ENG_ID, int NEW_STATUS_ID, List<int> OBS_IDS)
            {
            return "{\"Status\":true,\"Message\":\"" + dBConnection.AuditEngagementObsStatusReversal(ENG_ID, NEW_STATUS_ID, OBS_IDS) + "\"}";
            }


        [HttpPost]
        public List<ObservationNumbersModel> get_observation_numbers_for_status_reversal(int OBS_ID)
            {
            return dBConnection.GetObservationNumbersForStatusReversal(OBS_ID);
            }

        [HttpPost]
        public string update_observation_numbers_for_status_reversal(ObservationNumbersModel onum)
            {
            return "{\"Status\":true,\"Message\":\"" + dBConnection.UpdateObservationNumbersForStatusReversal(onum) + "\"}";
            }
        [HttpPost]
        public string update_engagement_dates_for_status_reversal(int ENG_ID, DateTime START_DATE, DateTime END_DATE)
            {
            return "{\"Status\":true,\"Message\":\"" + dBConnection.UpdateEngagementDatesForStatusReversal(ENG_ID, START_DATE, END_DATE) + "\"}";
            }
        [HttpPost]
        public List<HRDesignationWiseRoleModel> get_hr_designation_wise_roles()
            {
            return dBConnection.GetHRDesignationWiseRoles();
            }

        [HttpPost]
        public string add_hr_designation_wise_role_assignment(int ASSIGNMENT_ID, int DESIGNATION_ID, int GROUP_ID, string SUB_ENTITY_NAME)
            {
            return "{\"Status\":true,\"Message\":\"" + dBConnection.AddHRDesignationWiseRoleAssignment(ASSIGNMENT_ID, DESIGNATION_ID, GROUP_ID, SUB_ENTITY_NAME) + "\"}";
            }

        [HttpPost]
        public string update_hr_designation_wise_role_assignment(int ASSIGNMENT_ID, int DESIGNATION_ID, int GROUP_ID, string SUB_ENTITY_NAME)
            {
            return "{\"Status\":true,\"Message\":\"" + dBConnection.UpdateHRDesignationWiseRoleAssignment(ASSIGNMENT_ID, DESIGNATION_ID, GROUP_ID, SUB_ENTITY_NAME) + "\"}";
            }

        public List<ManageObservationModel> get_maange_obs_status()
            {
            return dBConnection.GetManageObservationStatus();
            }

        [HttpPost]
        public string add_manage_observatiton_status(ManageObservationModel OBS_STATUS_MODEL)
            {
            return "{\"Status\":true,\"Message\":\"" + dBConnection.AddManageObservationStatus(OBS_STATUS_MODEL) + "\"}";
            }
        [HttpPost]
        public string update_manage_observatiton_status(ManageObservationModel OBS_STATUS_MODEL)
            {
            return "{\"Status\":true,\"Message\":\"" + dBConnection.UpdateManageObservationStatus(OBS_STATUS_MODEL) + "\"}";
            }
        [HttpPost]
        public List<ManageEntAuditDeptModel> get_manage_ent_audit_dept()
            {
            return dBConnection.GetManageEntityAuditDept();
            }
        public string add_manage_entities_audit_department(ManageEntAuditDeptModel ENT_AUD_DEPT_MODEL)
            {
            return "{\"Status\":true,\"Message\":\"" + dBConnection.AddManageEntityAuditDepartment(ENT_AUD_DEPT_MODEL) + "\"}";
            }
        [HttpPost]
        public string update_manage_entities_audit_department(ManageEntAuditDeptModel ENT_AUD_DEPT_MODEL)
            {
            return "{\"Status\":true,\"Message\":\"" + dBConnection.UpdateEntityAuditDepartment(ENT_AUD_DEPT_MODEL) + "\"}";
            }

        [HttpPost]
        public List<LoanDetailReportModel> get_loan_detail_report(int ENT_ID, int GLSUBID, int STATUSID, DateTime START_DATE, DateTime END_DATE)
            {
            return dBConnection.GetLoanDetailsReport(ENT_ID, GLSUBID, STATUSID, START_DATE, END_DATE);
            }

        [HttpPost]
        public List<LoanDetailReportModel> get_cnic_loan_detail_report(string CNIC)
            {
            return dBConnection.GetCNICLoanDetailsReport(CNIC);
            }
        [HttpPost]
        public List<DefaultHisotryLoanDetailReportModel> get_default_cnic_loan_detail_report(string CNIC, string LOAN_DISB_ID)
            {
            return dBConnection.GetDefaultCNICLoanDetailsReport(CNIC, LOAN_DISB_ID);
            }

        [HttpPost]
        public List<AuditeeEntitiesModel> get_region_zone_office(int RGM_ID)
            {
            return dBConnection.GetRBHList(RGM_ID);
            }

        [HttpPost]
        public List<AuditPeriodModel> audit_periods(int dept_code = 0, int AUDIT_PERIOD_ID = 0)
            {
            return dBConnection.GetAuditPeriods(dept_code, AUDIT_PERIOD_ID);
            }

        [HttpPost]
        public string add_audit_period(AddAuditPeriodModel auditPeriod)
            {
            AuditPeriodModel apm = new AuditPeriodModel();
            apm.STATUS_ID = 1;
            apm.DESCRIPTION = auditPeriod.DESCRIPTION;
            apm.START_DATE = DateTime.ParseExact(auditPeriod.STARTDATE, "MM/dd/yyyy", null);
            apm.END_DATE = DateTime.ParseExact(auditPeriod.ENDDATE, "MM/dd/yyyy", null);
            return "{\"Status\":true,\"Message\":\"" + dBConnection.AddAuditPeriod(apm) + "\"}";


            }

        [HttpPost]
        public string update_audit_period(AuditPeriodModel auditPeriod)
            {
            return "{\"Status\":true,\"Message\":\"" + dBConnection.UpdateAuditPeriod(auditPeriod) + "\"}";
            }
        [HttpPost]
        public List<SubMenuModel> get_sub_menu_for_admin_panel(int M_ID)
            {
            return dBConnection.GetSubMenusForAdminPanel(M_ID);
            }

        [HttpPost]
        public string add_sub_menu_for_admin_panel(SubMenuModel sm)
            {
            return "{\"Status\":true,\"Message\":\"" + dBConnection.AddSubMenuForAdminPanel(sm) + "\"}";
            }
        [HttpPost]
        public string update_sub_menu_for_admin_panel(SubMenuModel sm)
            {
            return "{\"Status\":true,\"Message\":\"" + dBConnection.UpdateSubMenuForAdminPanel(sm) + "\"}";
            }
        [HttpPost]
        public List<MenuPagesAssignmentModel> get_menu_pages_for_admin_panel(int M_ID, int SM_ID)
            {
            return dBConnection.GetMenuPagesForAdminPanel(M_ID, SM_ID);
            }
        [HttpPost]
        public string add_menu_page_for_admin_panel(MenuPagesAssignmentModel mPage)
            {
            return "{\"Status\":true,\"Message\":\"" + dBConnection.AddMenuPageForAdminPanel(mPage) + "\"}";
            }
        [HttpPost]
        public string update_menu_page_for_admin_panel(MenuPagesAssignmentModel mPage)
            {
            return "{\"Status\":true,\"Message\":\"" + dBConnection.UpdateMenuPageForAdminPanel(mPage) + "\"}";
            }

        [HttpPost]
        public List<DraftDSAGuidelines> get_draft_dsa_guidelines()
            {
            return dBConnection.GetDraftDSAGuidelines();
            }
        [HttpPost]
        public string draft_dsa(int OBS_ID, List<string> RESP_LIST, List<string> GID_LIST, string DSA_CONTENT)
            {
            string resp = "";
            foreach (string PPNO in RESP_LIST)
                {
                List<Object> outResp = new List<object>();
                outResp = dBConnection.DraftDSA(OBS_ID, PPNO, DSA_CONTENT);
                resp += "<p>" + outResp[0].ToString() + "</p>";
                foreach (string GID in GID_LIST)
                    {
                    dBConnection.AddDraftDSAGuideline(outResp[1].ToString(), GID);
                    }

                }
            return "{\"Status\":true,\"Message\":\"" + resp + "\"}";
            }

        [HttpPost]
        public string update_compliance_office(List<int> ENT_ID_ARR, int AUD_ID, string COMP_ID)
            {
            string res = "";
            if (ENT_ID_ARR.Count > 0)
                {
                foreach (int ENT_ID in ENT_ID_ARR)
                    {
                    res = dBConnection.UpdateComplianceUnit(ENT_ID, AUD_ID, COMP_ID);
                    }
                }

            return "{\"Status\":true,\"Message\":\"" + res + "\"}";

            }
        [HttpPost]
        public List<GISTWiseReportParas> get_report_para_by_gist_keyword(string GIST)
            {
            return dBConnection.GetAuditReportParaByGistKeyword(GIST);
            }
        [HttpPost]
        public List<AnnexureModel> get_annexures()
            {
            return dBConnection.GetAnnexuresForChecklistDetail();
            }

        [HttpPost]
        public string add_annexure(string ANNEX_CODE = "", int PROCESS_ID = 0, int FUNCTION_OWNER_ID = 0, string HEADING = "", int RISK_ID = 0, string MAX_NUMBER = "", string GRAVITY = "", string WEIGHTAGE = "")
            {
            return "{\"Status\":true,\"Message\":\"" + dBConnection.AddAnnexure(ANNEX_CODE, HEADING, PROCESS_ID, FUNCTION_OWNER_ID, RISK_ID, MAX_NUMBER, GRAVITY, WEIGHTAGE) + "\"}";
            }
        [HttpPost]
        public string update_annexure(int ANNEX_ID = 0, int PROCESS_ID = 0, int FUNCTION_OWNER_ID = 0, string HEADING = "", int RISK_ID = 0, string MAX_NUMBER = "", string GRAVITY = "", string WEIGHTAGE = "")
            {
            return "{\"Status\":true,\"Message\":\"" + dBConnection.UpdateAnnexure(ANNEX_ID, HEADING, PROCESS_ID, FUNCTION_OWNER_ID, RISK_ID, MAX_NUMBER, GRAVITY, WEIGHTAGE) + "\"}";
            }
        [HttpPost]
        public string generate_traditional_risk_rating_of_engagement(int ENG_ID)
            {
            return "{\"Status\":true,\"Message\":\"" + dBConnection.GenerateTraditionalRiskRatingofEngagement(ENG_ID) + "\"}";
            }

        [HttpPost]
        public List<TraditionalRiskRatingModel> view_traditional_risk_rating_of_engagement(int ENG_ID)
            {
            return dBConnection.ViewTraditionalRiskRatingofEngagement(ENG_ID);
            }
        [HttpPost]
        public string generate_annexure_risk_rating_of_engagement(int ENG_ID)
            {
            return "{\"Status\":true,\"Message\":\"" + dBConnection.GenerateAnnexureRiskRatingofEngagement(ENG_ID) + "\"}";
            }

        [HttpPost]
        public List<TraditionalRiskRatingModel> view_annexure_risk_rating_of_engagement(int ENG_ID)
            {
            return dBConnection.ViewAnnexureRiskRatingofEngagement(ENG_ID);
            }
        [HttpPost]
        public List<RiskRatingModelForBranchesWorking> get_risk_rating_model_for_branches_working(int ENG_ID)
            {
            return dBConnection.GetRiskRatingModelForBranchesWorking(ENG_ID);
            }
        [HttpPost]
        public List<ComplianceHierarchyModel> get_compliance_hierarchy()
            {
            return dBConnection.GetComplianceHierarchies();
            }
        [HttpPost]
        public List<ComplianceProgressReportModel> get_compliance_progress_report(string ROLE_TYPE)
            {
            return dBConnection.GetComplianceProgressReport(ROLE_TYPE);
            }
        [HttpPost]
        public List<ComplianceProgressReportDetailModel> get_compliance_progress_report_details(string ROLE_TYPE, string PP_NO)
            {
            return dBConnection.GetComplianceProgressReportDetails(ROLE_TYPE, PP_NO);
            }
        [HttpPost]
        public string add_compliance_hierarchy(int ENTITY_ID, string REVIEWER_PP, string AUTHORIZER_PP)
            {
            return "{\"Status\":true,\"Message\":\"" + dBConnection.AddComplianceHierarchy(ENTITY_ID, REVIEWER_PP, AUTHORIZER_PP) + "\"}";
            }
        [HttpPost]
        public string update_compliance_hierarchy(int ENTITY_ID, string REVIEWER_PP, string AUTHORIZER_PP, string COMPLIANCE_KEY)
            {
            return "{\"Status\":true,\"Message\":\"" + dBConnection.UpdateComplianceHierarchy(ENTITY_ID, REVIEWER_PP, AUTHORIZER_PP, COMPLIANCE_KEY) + "\"}";
            }

        [HttpPost]
        public List<SettledParasModel> get_settled_paras_for_compliance_report(int ENTITY_TYPE_ID, DateTime? DATE_FROM, DateTime? DATE_TO)
            {
            return dBConnection.GetSettledParasForComplianceReport(ENTITY_TYPE_ID, DATE_FROM, DATE_TO);
            }
        [HttpPost]
        public List<SettledParasModel> get_post_compliance_settlement_report()
            {
            return new List<SettledParasModel>(); //dBConnection.GetSettledParasForComplianceReport();
            }

        [HttpPost]
        public List<ComplianceOSParasModel> get_paras_for_compliance_summary_report()
            {
            return dBConnection.GetParasForComplianceSummaryReport();
            }

        [HttpPost]
        public List<EngPlanDelayAnalysisReportModel> get_engagement_plan_delay_analysis_report()
            {
            return dBConnection.GetEngagementPlanDelayAnalysisReport();
            }
        [HttpPost]
        public List<CAUParaForComplianceModel> get_cau_paras_for_compliance()
            {
            return dBConnection.GetCAUParasForPostCompliance();
            }
        [HttpPost]
        public List<UserRelationshipModel> get_parent_relationship_for_CAU(int ENTITY_REALTION_ID)
            {
            return dBConnection.GetParentRelationshipForCAU(ENTITY_REALTION_ID);
            }
        [HttpPost]
        public List<UserRelationshipModel> get_child_relationship_for_CAU(int E_R_ID)
            {
            return dBConnection.GetChildRelationshipForCAU(E_R_ID);
            }
        [HttpPost]
        public string submit_cau_para_to_branch(string COM_ID, string BR_ENT_ID, string CAU_COMMENTS)
            {
            return "{\"Status\":true,\"Message\":\"" + dBConnection.SubmitCAUParaToBranch(COM_ID, BR_ENT_ID, CAU_COMMENTS) + "\"}";
            }
        [HttpPost]
        public ParaTextModel get_cau_para_to_branch_para_text(string COM_ID, string INDICATOR)
            {
            return dBConnection.GetCAUParaToBranchParaText(COM_ID, INDICATOR);
            }
        [HttpPost]
        public List<CAUParaForComplianceModel> get_cau_paras_for_compliance_submitted_to_branch()
            {
            return dBConnection.GetCAUParasForPostComplianceSubmittedToBranch();
            }
        [HttpPost]
        public async Task<string> submit_cau_para_by_branch(string COM_ID, string TEXT_ID, string BR_COMMENTS)
            {
            return "{\"Status\":true,\"Message\":\"" + await dBConnection.SubmitCAUParaByBranch(COM_ID, TEXT_ID, BR_COMMENTS) + "\"}";
            }
        [HttpPost]
        public List<CAUParaForComplianceModel> get_cau_paras_for_compliance_for_review()
            {
            return dBConnection.GetCAUParasForPostComplianceForReview();
            }
        [HttpPost]
        public List<AuditeeResponseEvidenceModel> get_cau_paras_evidences_for_compliance_for_review(string TEXT_ID)
            {
            return dBConnection.GetCAUAllComplianceEvidence(TEXT_ID);
            }

        [HttpPost]
        public List<FADMonthlyReviewParasModel> get_fad_monthly_review_paras_for_entity_type_id(string ENT_TYPE_ID, DateTime? S_DATE, DateTime? E_DATE)
            {
            return dBConnection.GetFADMonthlyReviewParasForEntityTypeId(ENT_TYPE_ID, S_DATE, E_DATE);
            }
        [HttpPost]
        public List<SpecialAuditPlanModel> get_saved_special_audit_plans()
            {
            return dBConnection.GetSaveSpecialAuditPlan();
            }
        [HttpPost]
        public string add_special_audit_plan(string NATURE, string PERIOD, string ENTITY_ID, string NO_DAYS, string PLAN_ID, string INDICATOR)
            {
            return "{\"Status\":true,\"Message\":\"" + dBConnection.AddSpecialAuditPlan(NATURE, PERIOD, ENTITY_ID, NO_DAYS, PLAN_ID, INDICATOR) + "\"}";
            }
        [HttpPost]
        public string delete_special_audit_plan(string PLAN_ID, string INDICATOR)
            {
            return "{\"Status\":true,\"Message\":\"" + dBConnection.DeleteSpecialAuditPlan(PLAN_ID, INDICATOR) + "\"}";
            }
        [HttpPost]
        public string submit_special_audit_plan(string PLAN_ID, string INDICATOR)
            {
            return "{\"Status\":true,\"Message\":\"" + dBConnection.SubmitSpecialAuditPlan(PLAN_ID, INDICATOR) + "\"}";
            }
        [HttpPost]
        public string referred_back_special_audit_plan(string PLAN_ID, string INDICATOR)
            {
            return "{\"Status\":true,\"Message\":\"" + dBConnection.SubmitSpecialAuditPlan(PLAN_ID, INDICATOR) + "\"}";
            }
        [HttpPost]
        public string approve_special_audit_plan(string PLAN_ID, string INDICATOR)
            {
            return "{\"Status\":true,\"Message\":\"" + dBConnection.SubmitSpecialAuditPlan(PLAN_ID, INDICATOR) + "\"}";
            }

        [HttpPost]
        public List<DuplicateDeleteManageParaModel> get_duplicate_paras_for_authorize()
            {
            return dBConnection.GetDuplicateParasForAuthorization();
            }
        [HttpPost]
        public string request_delete_duplicate_para(int NEW_PARA_ID = 0, int OLD_PARA_ID = 0, string INDICATOR = "", string REMARKS = "")
            {
            return "{\"Status\":true,\"Message\":\"" + dBConnection.RequestDeleteDuplicatePara(NEW_PARA_ID, OLD_PARA_ID, INDICATOR, REMARKS) + "\"}";
            }
        [HttpPost]
        public string reject_delete_duplicate_para(int D_ID = 0)
            {
            return "{\"Status\":true,\"Message\":\"" + dBConnection.RejectDeleteDuplicatePara(D_ID) + "\"}";
            }
        [HttpPost]
        public string authorize_delete_duplicate_para(int D_ID = 0)
            {
            return "{\"Status\":true,\"Message\":\"" + dBConnection.AuthDeleteDuplicatePara(D_ID) + "\"}";
            }
        [HttpPost]
        public List<ObservationResponsiblePPNOModel> get_responsible_person_list(int PARA_ID, string INDICATOR)
            {
            return dBConnection.GetResponsiblePersonsList(PARA_ID, INDICATOR);
            }

        [HttpPost]
        public List<SeriousFraudulentObsGMDetails> get_serious_entities_details(string INDICATOR, int PARENT_ENT_ID, string ANNEX_IND)
            {
            return dBConnection.GetSeriousFraudulentObsGMDetails(INDICATOR, PARENT_ENT_ID, ANNEX_IND);
            }
        [HttpPost]
        public string add_responsible_to_observation(int NEW_PARA_ID, int OLD_PARA_ID, string INDICATOR, ObservationResponsiblePPNOModel RESPONSIBLE)
            {
            return "{\"Status\":true,\"Message\":\"" + dBConnection.AddResponsiblePersonsToObservation(NEW_PARA_ID, OLD_PARA_ID, INDICATOR, RESPONSIBLE) + "\"}";

            }
        [HttpPost]
        public string submit_dsa_to_auditee(int ENTITY_ID, int OBS_ID, int ENG_ID, List<RespDSAModel> RespDSAModel)
            {
            string out_resp = "";
            foreach (RespDSAModel rm in RespDSAModel)
                {
                out_resp += dBConnection.SubmitDSAToAuditee(ENTITY_ID, OBS_ID, ENG_ID, rm.RESP_PP_NO, rm.RESP_ROW_ID) + "<br/>";
                }
            return "{\"Status\":true,\"Message\":\"" + out_resp + "\"}";

            }
        [HttpPost]
        public List<DraftDSAList> get_draft_dsa_list()
            {
            return dBConnection.GetDraftDSAList();
            }
        [HttpPost]
        public DSAContentModel get_dsa_content(int DSA_ID)
            {
            return dBConnection.GetDraftDSAContent(DSA_ID);
            }
        [HttpPost]

        //SVP AZ ACTION
        public string submit_dsa_to_head_fad(int DSA_ID)
            {
            return "{\"Status\":true,\"Message\":\"" + dBConnection.SubmitDSAToHeadFAD(DSA_ID) + "\"}";

            }
        [HttpPost]

        //SVP AZ ACTION
        public string update_dsa_heading(int DSA_ID, string DSA_HEADING)
            {
            return "{\"Status\":true,\"Message\":\"" + dBConnection.UpdateDSAHeading(DSA_ID, DSA_HEADING) + "\"}";

            }
        [HttpPost]

        //HEAD FAD ACTION
        public string reffered_back_by_head_fad(int DSA_ID)
            {
            return "{\"Status\":true,\"Message\":\"" + dBConnection.ReferredBackDSAByHeadFad(DSA_ID) + "\"}";

            }
        [HttpPost]
        public string submit_dsa_to_dpd(int DSA_ID)
            {
            return "{\"Status\":true,\"Message\":\"" + dBConnection.SubmitDSAToDPD(DSA_ID) + "\"}";

            }
        [HttpPost]

        //SVP DPD ACTION
        public string reffered_back_by_dpd(int DSA_ID)
            {
            return "{\"Status\":true,\"Message\":\"" + dBConnection.ReferredBackDSAByDPD(DSA_ID) + "\"}";

            }
        [HttpPost]
        public string acknowledge_dsa_by_dpd(int DSA_ID)
            {
            return "{\"Status\":true,\"Message\":\"" + dBConnection.AcknowledgeDSA(DSA_ID) + "\"}";

            }

        [HttpPost]
        public List<LoanCaseDetailModel> get_lc_details(int LC_NO, int BR_CODE)
            {
            return dBConnection.GetLoanCaseDetailsWithBRCode(LC_NO, BR_CODE);
            }


        [HttpPost]
        public ObservationModel get_obs_details_by_id(int OBS_ID)
            {
            return dBConnection.GetObservationDetailsById(OBS_ID);
            }
        [HttpPost]
        public ObservationModel get_obs_details_by_id_pre_con(int OBS_ID)
            {
            return dBConnection.GetObservationDetailsByIdForPreConcluding(OBS_ID);
            }
        [HttpPost]
        public ObservationModel get_obs_details_by_id_pre_con_ho(int OBS_ID)
            {
            return dBConnection.GetObservationDetailsByIdForPreConcludingHO(OBS_ID);
            }
        [HttpPost]
        public ObservationModel get_obs_details_by_id_ho(int OBS_ID)
            {
            return dBConnection.GetObservationDetailsByIdHO(OBS_ID);
            }

        [HttpPost]
        public string update_gm_office(int GM_OFF_ID, int ENTITY_ID)
            {
            return "{\"Status\":true,\"Message\":\"" + dBConnection.UpdateGMOffice(GM_OFF_ID, ENTITY_ID) + "\"}";

            }
        [HttpPost]
        public string update_reporting_line(int REP_OFF_ID, int ENTITY_ID)
            {
            return "{\"Status\":true,\"Message\":\"" + dBConnection.UpdateReportingLine(REP_OFF_ID, ENTITY_ID) + "\"}";

            }

        [HttpPost]
        public string update_gm_reporting_line_office(List<int> ENT_ID_ARR, int GM_OFF_ID, int REP_OFF_ID)
            {
            string res = "";
            if (ENT_ID_ARR.Count > 0)
                {
                foreach (int ENT_ID in ENT_ID_ARR)
                    {
                    res = dBConnection.UpdateGMAndReportingLineOffice(ENT_ID, GM_OFF_ID, REP_OFF_ID);
                    }
                }
            return "{\"Status\":true,\"Message\":\"GM Office and Reporting Line Updated Successfully\"}";

            }

        [HttpPost]
        public async Task<string> upload_audit_report(int ENG_ID)
            {
            string response = await dBConnection.UploadAuditReport(ENG_ID);
            return "{\"Status\":true,\"Message\":\"" + response + "\"}";
            }

        [HttpPost]
        public List<FinalAuditReportModel> get_audit_reports(int ENG_ID)
            {
            return dBConnection.GetAuditReports(ENG_ID);
            }

        [HttpPost]
        public AuditeeResponseEvidenceModel get_audit_report_content(string FILE_ID)
            {
            return dBConnection.GetAuditReportContent(FILE_ID);
            }

        //
        [HttpPost]
        public FinalAuditReportModel get_check_report_exist_for_engId(int ENG_ID)
            {
            return dBConnection.GetCheckAuditReportExisits(ENG_ID);
            }

        [HttpPost]
        public string create_engagement_sample_data(int ENG_ID)
            {
            return "{\"Status\":true,\"Message\":\"" + dBConnection.CreateSampleDataAfterEngagementApproval(ENG_ID) + "\"}";
            }

        [HttpPost]
        public List<BiometSamplingModel> get_biomet_sampling_details(int ENG_ID)
            {
            return dBConnection.GetBiometSamplingDetails(ENG_ID);
            }
        [HttpPost]

        public List<ExecptionAccountReportModel> Get_exception_account_report(int ENG_ID,  int RPT_ID)
            {
            return dBConnection.Getexceptionaccountreport(ENG_ID, RPT_ID);
            }
        [HttpPost]
        public List<AccountTransactionSampleModel> get_biomet_account_transaction_sampling_details(int ENG_ID, string AC_NO)
            {
            return dBConnection.GetBiometAccountTransactionSamplingDetails(ENG_ID, AC_NO);
            }
        [HttpPost]
        public List<AccountDocumentBiometSamplingModel> get_biomet_account_documents_sampling_details(string AC_NO)
            {
            return dBConnection.GetBiometAccountDocumentsSamplingDetails(AC_NO);
            }
        [HttpPost]
        public List<YearWiseOutstandingObservationsModel> get_year_wise_outstanding_observations(int ENTITY_ID)
            {
            return dBConnection.GetYearWiseOutstandingParas(ENTITY_ID);
            }

        [HttpPost]
        public List<AuditeeOldParasModel> get_year_wise_outstanding_observations_detials(int ENTITY_ID, int AUDIT_PERIOD)
            {
            return dBConnection.GetYearWiseOutstandingParasDetails(ENTITY_ID, AUDIT_PERIOD);
            }

        [HttpPost]
        public List<ListOfSamplesModel> get_list_of_samples(int ENG_ID)
            {
            return dBConnection.GetListOfSamples(ENG_ID);
            }

        [HttpPost]
        public List<ListOfReportsModel> get_list_of_reports(int ENG_ID)
            {
            return dBConnection.GetListOfreports(ENG_ID);
            }

        [HttpPost]
        public List<LoanCaseSampleModel> get_loan_samples(string INDICATOR, int STATUS_ID, int ENG_ID, int SAMPLE_ID)
            {
            return dBConnection.GetLoanSamples(INDICATOR, STATUS_ID, ENG_ID, SAMPLE_ID);
            }
        [HttpPost]
        public List<LoanCaseSampleDocumentsModel> get_loan_documents(int ENG_ID, string LOAN_DISB_ID)
            {
            return dBConnection.GetLoanSamplesDocuments(ENG_ID, LOAN_DISB_ID);
            }
        [HttpPost]
        public List<LoanCaseSampleDocumentsModel> get_loan_document_data(int IMAGE_ID)
            {
            return dBConnection.GetLoanSamplesDocumentData(IMAGE_ID);
            }
        [HttpPost]
        public List<LoanCaseSampleTransactionsModel> get_sample_loan_transactions(int ENG_ID, string LOAN_DISB_ID)
            {
            return dBConnection.GetLoanSamplesTransactions(ENG_ID, LOAN_DISB_ID);
            }

        [HttpPost]
        public List<ParaTextSearchModel> get_para_text_in_audit_report(string SEARCH_KEYWORD)
            {
            return dBConnection.GetAuditParasByText(SEARCH_KEYWORD);
            }
        [HttpPost]
        public string regenerate_sample_of_loans(int ENG_ID, int LOAN_SAMPLE_ID)
            {
            return "{\"Status\":true,\"Message\":\"" + dBConnection.RegenerateSampleofLoan(ENG_ID, LOAN_SAMPLE_ID) + "\"}";
            }

        [HttpPost]
        public List<YearWiseAllParasModel> get_year_wise_all_audit_paras(string AUDIT_PERIOD)
            {
            return dBConnection.GetYearWiseAllParas(AUDIT_PERIOD);
            }
<<<<<<< HEAD
        [HttpPost]
        public List<CDMSMasterTransactionModel> get_CDMS_master_transactions(string ENTITY_ID, DateTime START_DATE, DateTime END_DATE, string CNIC_NO, string ACC_NO)
            {
            return dBConnection.GetCDMSMasterTransactions(ENTITY_ID, START_DATE, END_DATE, CNIC_NO, ACC_NO);
            }
=======

>>>>>>> parent of 068d39aa (Biomet and MASTER CDMS Transaction)
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
            {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
            }
        }
    }
