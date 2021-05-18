using System;
using System.Collections.Generic;

#nullable disable

namespace BulkMailSender.TableEntities
{
    public partial class TblAllTdsEmail
    {
        public int MailId { get; set; }
        public int TdsId { get; set; }
        public string TdsUserName { get; set; }
        public string TdsMailId { get; set; }
        public string TdsPdfName { get; set; }
        public string TdsPdfUrl { get; set; }
        public bool TdsIsMailSended { get; set; }
        public DateTime CreatedOn { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? UpdatedOn { get; set; }
        public string UpdatedBy { get; set; }
        public string IndividualEmailBody { get; set; }
        public string RestructuringKey { get; set; }

        public virtual TblTdsCertificate Tds { get; set; }
    }
}
