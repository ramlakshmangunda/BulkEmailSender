using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BulkMailSender.ViewModels
{
    public class CreateTdsViewModel
    {
        public string TdsTxnId { get; set; }
        public string TdsTitle { get; set; }
        public string TdsSubject { get; set; }
        public string TdsEmailFrom { get; set; }
        public string TdsEmailCc { get; set; }
        public string TdsEmailBody { get; set; }
        public DateTime? TdsCreatedOn { get; set; }
        public bool? IsIndividualEmailBody { get; set; }
    }
}
