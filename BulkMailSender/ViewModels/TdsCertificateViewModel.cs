using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BulkMailSender.ViewModels
{
    public class TdsCertificateViewModel
    {
        public int TdsId { get; set; }
        public string TdsTitle { get; set; }
        public string TdsSubject { get; set; }
        public string TdsEmailFrom { get; set; }
        public string TdsEmailCc { get; set; }
        public string TdsEmailBody { get; set; }
        public bool? TdsCompleted { get; set; }
        public string TdsExcelName { get; set; }
        public string TdsExcelUrl { get; set; }
        public string TdsCreatedBy { get; set; }
        public DateTime? TdsCreatedOn { get; set; }
        public string TdsUpdatedBy { get; set; }
        public DateTime? TdsUpdatedOn { get; set; }
    }
}
