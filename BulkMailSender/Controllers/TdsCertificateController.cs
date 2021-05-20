using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using BulkMailSender.Models;
using BulkMailSender.Services.Interfaces;
using BulkMailSender.ViewModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using OfficeOpenXml;

namespace BulkMailSender.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TdsCertificateController : ControllerBase
    {
        private readonly ITdsCertificateService _tdsService;
        private readonly IConfiguration _configurations;

        public TdsCertificateController(ITdsCertificateService tdsService, IConfiguration configurations)
        {
            _tdsService = tdsService;
            _configurations = configurations;
        }

        [HttpPost("AddNewTdsCertificate")]
        [ProducesResponseType(200, Type = typeof(ServiceResponse<ResponseModel>))]
        public async Task<IActionResult> AddNewTdsCertificate(CreateTdsViewModel ViewModel)
        {
            try
            {
                if (ViewModel == null)
                {
                    return BadRequest();
                }
                var Create = await _tdsService.AddNewTdsCertificate(ViewModel);
                return Ok(Create);
            }
            catch (Exception Ex)
            {

                throw Ex;
            }
        }

        [HttpGet("GetAllTdsCertificates")]
        [ProducesResponseType(200, Type = typeof(ServiceResponse<List<TdsCertificateViewModel>>))]
        public async Task<IActionResult> GetAllTdsCertificates()
        {
            try
            {
                var Create = await _tdsService.GetAllTdsCertificates();
                return Ok(Create);
            }
            catch (Exception Ex)
            {

                throw Ex;
            }
        }

        [HttpPut("UpdateTdsCertificate")]
        [ProducesResponseType(200, Type = typeof(ServiceResponse<ResponseModel>))]
        public async Task<IActionResult> UpdateTdsCertificate(TdsCertificateViewModel ViewModel)
        {
            try
            {
                if (ViewModel == null)
                {
                    return BadRequest();
                }
                var Create = await _tdsService.UpdateTdsCertificate(ViewModel);
                return Ok(Create);
            }
            catch (Exception Ex)
            {

                throw Ex;
            }
        }

        [HttpPost("UploadPDFfiles")]
        [ProducesResponseType(200, Type = typeof(ServiceResponse<ResponseModel>))]
        public async Task<IActionResult> UploadPDFfiles([FromForm] UploadPdfFiles uploadPdfFiles)
        {
            try
            {
                if (uploadPdfFiles == null)
                {
                    return BadRequest();
                }
                var Create = await _tdsService.UploadPDFfiles(uploadPdfFiles);
                return Ok(Create);
            }
            catch (Exception Ex)
            {

                throw Ex;
            }
        }

        [HttpPost("UploadExcelFile")]
        [ProducesResponseType(200, Type = typeof(ServiceResponse<ResponseModel>))]
        public async Task<IActionResult> UploadExcelFile([FromForm] UploadExcelFile uploadExcelFile)
        {
            try
            {
                if (uploadExcelFile == null)
                {
                    return BadRequest();
                }
                var Create = await _tdsService.UploadExcelFile(uploadExcelFile);
                return Ok(Create);
            }
            catch (Exception Ex)
            {

                throw Ex;
            }
        }

        [HttpGet("TdsCertificateSendMails/{TdsId}")]
        [ProducesResponseType(200, Type = typeof(ServiceResponse<ResponseModel>))]
        public async Task<IActionResult> TdsCertificateSendMails(int? TdsId)
        {
            try
            {
                if (TdsId == 0)
                {
                    return BadRequest();
                }
                var Create = await _tdsService.TdsCertificateSendMails(TdsId);
                return Ok(Create);
            }
            catch (Exception Ex)
            {

                throw Ex;
            }
        }

        [HttpGet("GetAllEmailsList/{TdsId}")]
        [ProducesResponseType(200, Type = typeof(ServiceResponse<List<AllTdsEmailViewModel>>))]
        public async Task<IActionResult> GetAllEmailsList(int? TdsId)
        {
            try
            {
                if (TdsId==0 || TdsId==null)
                {
                    return BadRequest();
                }
                var Create = await _tdsService.GetAllEmailsList(TdsId);
                return Ok(Create);
            }
            catch (Exception Ex)
            {

                throw Ex;
            }
        }

        [HttpDelete("DeleteTdsEmail/{MailId}")]
        [ProducesResponseType(200, Type = typeof(ServiceResponse<ResponseModel>))]
        public async Task<IActionResult> DeleteTdsEmail(int? MailId)
        {
            try
            {
                if (MailId == 0 || MailId == null)
                {
                    return BadRequest();
                }
                var Delete = await _tdsService.DeleteTdsEmail(MailId);
                return Ok(Delete);
            }
            catch (Exception Ex)
            {

                throw Ex;
            }
        }

        [HttpDelete("DeletePdfFile/{MailId}")]
        [ProducesResponseType(200, Type = typeof(ServiceResponse<ResponseModel>))]
        public async Task<IActionResult> DeletePdfFile(int? MailId)
        {
            try
            {
                if (MailId == 0 || MailId == null)
                {
                    return BadRequest();
                }
                var Delete = await _tdsService.DeletePdfFile(MailId);
                return Ok(Delete);
            }
            catch (Exception Ex)
            {

                throw Ex;
            }
        }

        [HttpPut("UpdateTDSEmailData")]
        [ProducesResponseType(200, Type = typeof(ServiceResponse<ResponseModel>))]
        public async Task<IActionResult> UpdateTDSEmailData(EmailUpdateModel model)
        {
            try
            {
                if (model==null)
                {
                    return BadRequest();
                }
                var Update = await _tdsService.UpdateTDSEmailData(model);
                return Ok(Update);
            }
            catch (Exception Ex)
            {

                throw Ex;
            }
        }

        #region Export Excel
        [HttpGet("VoltasRestructuringExportExcel")]
        [ProducesResponseType(200, Type = typeof(List<RestructuringViewModel>))]
        public async Task<IActionResult> VoltasRestructuringExportExcel()
        {
            try
            {
                var ExportExcel = await _tdsService.VoltasRestructuringExportExcel();
                var Datatable = ConvertToDataTable<RestructuringViewModel>(ExportExcel);
                DataSet ds = new DataSet();
                ds.Tables.Add(Datatable);
                GenerateExcel(ds);
                return Ok("Excel saved in server,Please download.");
            }
            catch (Exception Ex)
            {
                throw Ex;
            }
        }

        static DataTable ConvertToDataTable<T>(List<T> models)
        {
            // creating a data table instance and typed it as our incoming model   
            // as I make it generic, if you want, you can make it the model typed you want.  
            DataTable dataTable = new DataTable(typeof(T).Name);

            //Get all the properties of that model  
            PropertyInfo[] Props = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance);

            // Loop through all the properties              
            // Adding Column name to our datatable  
            foreach (PropertyInfo prop in Props)
            {
                //Setting column names as Property names    
                dataTable.Columns.Add(prop.Name);
            }
            // Adding Row and its value to our dataTable  
            foreach (T item in models)
            {
                var values = new object[Props.Length];
                for (int i = 0; i < Props.Length; i++)
                {
                    //inserting property values to datatable rows    
                    values[i] = Props[i].GetValue(item, null);
                }
                // Finally add value to datatable    
                dataTable.Rows.Add(values);
            }
            return dataTable;
        }
        
        private void GenerateExcel(DataSet dataSet)
        {
            using (ExcelPackage pck = new ExcelPackage())
            {
                foreach (DataTable dataTable in dataSet.Tables)
                {
                    ExcelWorksheet workSheet = pck.Workbook.Worksheets.Add(dataTable.TableName);
                    workSheet.Cells["A1"].LoadFromDataTable(dataTable, true);
                }
                var basePath = _configurations.GetValue<string>("ExcelDownloadPaths:VoltasRestructureExcelPath");

                var Filepath = basePath + "VoltasRestructure" + ".xlsx";

                if (System.IO.File.Exists(Filepath))
                {
                    System.IO.File.Delete(Filepath);
                }

                if (!Directory.Exists(basePath))
                    Directory.CreateDirectory(basePath);

                pck.SaveAs(new FileInfo(Filepath));
            }
        }

        #endregion
    }
}
