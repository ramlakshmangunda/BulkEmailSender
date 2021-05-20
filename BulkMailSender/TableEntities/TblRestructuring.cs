using System;
using System.Collections.Generic;

#nullable disable

namespace BulkMailSender.TableEntities
{
    public partial class TblRestructuring
    {
        public int Rid { get; set; }
        public int? TdsEid { get; set; }
        public string Email { get; set; }
        public string VendorCode { get; set; }
        public string VendorName { get; set; }
        public bool Confirmation { get; set; }
        public string Remarks { get; set; }
        public bool SendCopy { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? CreatedOn { get; set; }
        public string UpdatedBy { get; set; }
        public DateTime? UpdatedOn { get; set; }

        public virtual TblAllTdsEmail TdsE { get; set; }
    }
}
