using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AIS.Models;
using Microsoft.AspNetCore.Http;
using System.Text.Json;


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

    }
}
