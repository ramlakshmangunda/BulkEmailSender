using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BulkMailSender.ViewModels
{
    public class UploadPdfFiles
    {
        public List<IFormFile> PdfFiles { get; set; }
        public int TdsId { get; set; }
    }

    public class UploadExcelFile
    {
        public IFormFile ExcelFile { get; set; }
        public int TdsId { get; set; }
    }
}
