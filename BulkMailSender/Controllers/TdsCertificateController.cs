using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BulkMailSender.Models;
using BulkMailSender.Services.Interfaces;
using BulkMailSender.ViewModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace BulkMailSender.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TdsCertificateController : ControllerBase
    {
        private readonly ITdsCertificateService _tdsService;

        public TdsCertificateController(ITdsCertificateService tdsService)
        {
            _tdsService = tdsService;
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
    }
}
