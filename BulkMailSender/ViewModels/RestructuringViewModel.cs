using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BulkMailSender.ViewModels
{
    public class RestructuringViewModel
    {
        public string Email { get; set; }
        public string VendorCode { get; set; }
        public string VendorName { get; set; }
       // public bool Confirmation { get; set; }
        public string Remarks { get; set; }
       // public bool SendCopy { get; set; }

        public string Confirmation { get; set; }
        public string SendCopy { get; set; }
        public DateTime? RespondedDate { get; set; }
    }
}
