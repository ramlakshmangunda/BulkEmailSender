using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BulkMailSender.Models
{
    public class ServiceResponse<T>
    {
        public T Result { get; set; }
        public bool Success { get; set; }
        public string ErrorCode { get; set; }
        public List<string> Errors { get; set; }
    }
}
