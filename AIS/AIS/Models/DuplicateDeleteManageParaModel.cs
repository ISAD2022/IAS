using System;

namespace AIS.Models
    {
    public class DuplicateDeleteManageParaModel
        {
        public int DId { get; set; }


        public int OldParaId { get; set; }


        public int NewParaId { get; set; }


        public string EntityId { get; set; }
        public string EntityName { get; set; }


        public string EntityCode { get; set; }
        public string ParaGist { get; set; }
        public string AuditPeriod { get; set; }


        public string AuditedBy { get; set; }


        public string ParaNo { get; set; }


        public string ParaStatus { get; set; }


        public string Ind { get; set; }


        public string Risk { get; set; }


        public string Instances { get; set; }


        public string Amount { get; set; }


        public string Annex { get; set; }


        public string AddedBy { get; set; }


        public DateTime AddedOn { get; set; }


        public bool AuthorizedStatus { get; set; }


        public string AuthorizedBy { get; set; }


        public DateTime? AuthorizedOn { get; set; }


        public string Remarks { get; set; }

        }
    }
