using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BulkMailSender.Models
{
    public class EmailUpdateModel
    {
        public int MailId { get; set; }
        public string TdsUserName { get; set; }
        public string TdsMailId { get; set; }
        public string TdsPdfName { get; set; }
        public string TdsPdfUrl { get; set; }
        public DateTime? UpdatedOn { get; set; }
        public string UpdatedBy { get; set; }
    }
}
