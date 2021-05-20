using BulkMailSender.Models;
using BulkMailSender.ViewModels;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace BulkMailSender.Services.Interfaces
{
    public interface ITdsCertificateService
    {
        Task<ResponseModel> AddNewTdsCertificate(CreateTdsViewModel ViewModel);
        Task<List<TdsCertificateViewModel>> GetAllTdsCertificates();
        Task<ResponseModel> UpdateTdsCertificate(TdsCertificateViewModel ViewModel);
        Task<ResponseModel> UploadPDFfiles(UploadPdfFiles uploadPdfFiles);
        Task<ResponseModel> UploadExcelFile(UploadExcelFile uploadExcelFile);
        Task<ResponseModel> TdsCertificateSendMails(int? TdsId);
        Task<List<AllTdsEmailViewModel>> GetAllEmailsList(int? TdsId);
        Task<ResponseModel> DeleteTdsEmail(int? MailId);
        Task<ResponseModel> DeletePdfFile(int? MailId);
        Task<ResponseModel> UpdateTDSEmailData(EmailUpdateModel model);
        Task<List<RestructuringViewModel>> VoltasRestructuringExportExcel();
    }
}
