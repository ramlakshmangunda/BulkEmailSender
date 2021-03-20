using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BulkMailSender.Models
{
    public class EmailSendResponse
    {
        public int StatusCode { get; set; }
        public string ResponseMessage { get; set; }
    }
}
