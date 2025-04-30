using AIS.Models;


namespace AIS
    {

    public class SQLParams
        {
        public SQLParams()
            {

            }
        public string GetCriteriaIdQueryFromParams(int RISK_ID, int SIZE_ID, int ENTITY_TYPE_ID, int PERIOD_ID, int FREQUENCY_ID)
            {
            return "select c.ID from t_audit_criteria c where c.entity_typeid=" + ENTITY_TYPE_ID + " and c.auditperiodid= " + PERIOD_ID + " and c.size_id=" + SIZE_ID + " and c.risk_id=" + RISK_ID + " and c.frequency_id=" + FREQUENCY_ID;

            }

        public string GetCriteriaEntitiesQueryFromParams(int RISK_ID, int SIZE_ID, int ENTITY_TYPE_ID, int PERIOD_ID, int FREQUENCY_ID)
            {
            return "select c.NO_OF_ENTITY from t_audit_criteria c where c.entity_typeid=" + ENTITY_TYPE_ID + " and c.auditperiodid= " + PERIOD_ID + " and c.size_id=" + SIZE_ID + " and c.risk_id=" + RISK_ID + " and c.frequency_id=" + FREQUENCY_ID;

            }
        public string GetCriteriaLatestRemarksQueryFromParams(int CID)
            {
            return "select remarks from T_AUDIT_CRITERIA_LOG l where l.c_id=" + CID + " order by l.id desc FETCH NEXT 1 ROWS ONLY";
            }
        public string GetDivisionQueryFromParams()
            {
            return "select * from t_auditee_entities d WHERE d.type_id=3 order by d.CODE asc";

            }
        public string GetDepartmentQueryFromParams(int div_code = 0, bool sessionCheck = false, SessionModel loggedInUser = null)
            {
            string query = "";

            if (loggedInUser.UserGroupID != 1)
                {
                if (sessionCheck)
                    {
                    query = query + " and e.entity_id=" + loggedInUser.UserEntityID;

                    }
                }
            if (div_code == 0)
                return "select mp.parent_id as DIVISIONID, mp.entity_id as ID , mp.c_name as NAME, mp.child_code as CODE ,mp.status as ISACTIVE, mp.p_name as DIV_NAME, mp.auditedby as AUDITED_BY_DEPID from t_auditee_entities e, t_auditee_entities_maping mp where e.entity_id = mp.parent_id and e.type_id IN (3) and mp.entity_id is not null" + query;
            else
                return "select mp.parent_id as DIVISIONID, mp.entity_id as ID , mp.c_name as NAME, mp.child_code as CODE ,mp.status as ISACTIVE, mp.p_name as DIV_NAME, mp.auditedby as AUDITED_BY_DEPID from t_auditee_entities e, t_auditee_entities_maping mp where mp.entity_id is not null and e.type_id IN (3) and e.entity_id = mp.parent_id and e.entity_id = " + div_code + query;
            }
        public string GetDepartmentNameByIdQueryFromParams(int deptId)
            {
            return "Select dep.NAME FROM t_auditee_entities dep WHERE dep.ENTITY_ID = " + deptId;

            }


        }
    }
