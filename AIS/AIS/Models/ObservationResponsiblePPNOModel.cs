using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace AIS.Models
{
    public class ObservationResponsiblePPNOModel
    {

        public string EMP_NAME { get; set; }
        public string PP_NO { get; set; }
        public string LOAN_CASE { get; set; }
        public string LC_AMOUNT { get; set; }
        public string ACCOUNT_NUMBER { get; set; }
        public string ACC_AMOUNT { get; set; }
    }
}
