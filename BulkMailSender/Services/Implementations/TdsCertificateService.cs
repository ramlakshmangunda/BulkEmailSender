using AutoMapper;
using BulkMailSender.Models;
using BulkMailSender.Services.Interfaces;
using BulkMailSender.TableEntities;
using BulkMailSender.ViewModels;
using Microsoft.Extensions.Configuration;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Microsoft.Data.SqlClient;
using System.Data;
using System.ComponentModel.DataAnnotations;

namespace BulkMailSender.Services.Implementations
{
    public class TdsCertificateService : ITdsCertificateService
    {
        private readonly BekoDBContext _dBContext;
        private readonly IMapper _mapper;
        private readonly IConfiguration _configuration;
        private string _Host;
        private string _From;
        private string _UserName;
        private string _Password;
        private int _Port;
        public TdsCertificateService(BekoDBContext DBContext, IMapper mapper, IConfiguration configuration)
        {
            _dBContext = DBContext;
            _mapper = mapper;
            _configuration = configuration;
        }

        //public SqlConnection Connection
        //{
        //    get
        //    {
        //        return new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));
        //    }
        //}
        public async Task<ResponseModel> AddNewTdsCertificate(CreateTdsViewModel ViewModel)
        {
            try
            {
                // using (var context = new BekoDBContext())
                //ViewModel.TdsEmailBody = _dBContext.TblTdsCertificates.Where(f => f.TdsId == 23).Select(s => s.TdsEmailBody).FirstOrDefault();

                using IDbConnection db = new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));
                using (var command = db.CreateCommand())
                {
                    command.CommandText = "SELECT (('TXN'+left(CONVERT([varchar](36),newid()),(4)))+right(CONVERT([varchar](36),newid()),(4)))";
                    db.Open();
                    using (var result = command.ExecuteReader())
                    {
                        while (result.Read())
                        {
                            ViewModel.TdsTxnId = result[0].ToString();
                        }
                    }
                    db.Close();
                }

                var Mapdata = _mapper.Map<TblTdsCertificate>(ViewModel);
                await _dBContext.AddAsync<TblTdsCertificate>(Mapdata);
                await _dBContext.SaveChangesAsync();
                return new ResponseModel { StatusCode = Convert.ToInt32(HttpStatusCode.Created), ResponseMessage = "NewTdsCertificate created" };
            }
            catch (Exception Ex)
            {
                throw Ex;
            }
        }
        public async Task<List<TdsCertificateViewModel>> GetAllTdsCertificates()
        {
            try
            {
                var GetAll = await _dBContext.TblTdsCertificates.ToListAsync();
                var Mapdata = _mapper.Map<List<TdsCertificateViewModel>>(GetAll);
                return Mapdata;
            }
            catch (Exception Ex)
            {
                throw Ex;
            }
        }

        public async Task<ResponseModel> UpdateTdsCertificate(TdsCertificateViewModel ViewModel)
        {
            try
            {
                var GetData = _dBContext.TblTdsCertificates.Where(f => f.TdsId == ViewModel.TdsId).FirstOrDefault();
                _mapper.Map<TdsCertificateViewModel, TblTdsCertificate>(ViewModel, GetData);
                _dBContext.Update<TblTdsCertificate>(GetData);
                await _dBContext.SaveChangesAsync();
                return new ResponseModel { StatusCode = Convert.ToInt32(HttpStatusCode.Created), ResponseMessage = "Updated successfully" };
            }
            catch (Exception Ex)
            {
                throw Ex;
            }
        }

        public async Task<ResponseModel> UploadPDFfiles(UploadPdfFiles uploadPdfFiles)
        {
            try
            {
                var GetTds = _dBContext.TblTdsCertificates.Where(f => f.TdsId == uploadPdfFiles.TdsId).FirstOrDefault();

                if (GetTds == null)
                {
                    return new ResponseModel { StatusCode = 200, ResponseMessage = "No Tds Certificates found" };
                }
                string filePath = _configuration.GetSection("KeyValue").GetSection("PhysicalPath").Value;

                filePath = filePath + GetTds.TdsTxnId + "";

                if (!Directory.Exists(filePath))
                {
                    Directory.CreateDirectory(filePath);
                }

                if (uploadPdfFiles.PdfFiles.Count > 0)
                {
                    foreach (var item in uploadPdfFiles.PdfFiles)
                    {
                        //upload files to wwwroot
                        var fileName = Path.GetFileName(item.FileName);
                        //judge if it is pdf file
                        string ext = Path.GetExtension(item.FileName);
                        if (ext.ToLower() != ".pdf")
                        {
                            break;
                        }
                        //var filePath = Path.Combine(_hostingEnv.WebRootPath, "images", fileName);

                        string NewfilePath = Path.Combine(filePath, item.FileName);

                        if (System.IO.File.Exists(NewfilePath))
                            System.IO.File.Delete(NewfilePath);

                        using (var fileStream = new FileStream(NewfilePath, FileMode.Create))
                        {
                            await item.CopyToAsync(fileStream);
                        }
                    }
                    return new ResponseModel { StatusCode = 200, ResponseMessage = "Success" };
                }
                return new ResponseModel { StatusCode = 404, ResponseMessage = "No pdfs or Excel found" };
            }
            catch (Exception)
            {
                throw new Exception();
            }
        }

        public async Task<ResponseModel> UploadExcelFile(UploadExcelFile uploadExcelFile)
        {
            try
            {
                var GetTds = _dBContext.TblTdsCertificates.Where(f => f.TdsId == uploadExcelFile.TdsId).Include(f => f.TblAllTdsEmails).FirstOrDefault();

                if (GetTds == null)
                {
                    return new ResponseModel { StatusCode = 200, ResponseMessage = "No Tds Certificates found" };
                }
                //var MailsList=await _dBContext.TblAllTdsEmails.Where(f => f.TdsId == uploadExcelFile.TdsId).ToListAsync();

                if (GetTds.TblAllTdsEmails.Count != 0)
                {
                    _dBContext.RemoveRange(GetTds.TblAllTdsEmails);
                    await _dBContext.SaveChangesAsync();
                }

                string filePath = _configuration.GetSection("KeyValue").GetSection("PhysicalPath").Value;

                string fileVirtualPathQP = _configuration.GetSection("KeyValue").GetSection("QPVirtualPath").Value;

                filePath = filePath + GetTds.TdsTxnId + "";

                if (!Directory.Exists(filePath))
                {
                    Directory.CreateDirectory(filePath);
                }
                if (uploadExcelFile.ExcelFile != null)
                {

                    var ExcelfileName = Path.GetFileName(uploadExcelFile.ExcelFile.FileName);

                    string ExcelfilePath = Path.Combine(filePath, uploadExcelFile.ExcelFile.FileName);

                    if (System.IO.File.Exists(ExcelfilePath))
                        System.IO.File.Delete(ExcelfilePath);

                    var list = new List<AllTdsEmailViewModel>();

                    List<string> InValidMailsList = new List<string>();

                    using (var fileStream = new FileStream(ExcelfilePath, FileMode.Create))
                    {
                        await uploadExcelFile.ExcelFile.CopyToAsync(fileStream);

                        using (var package = new ExcelPackage(fileStream))
                        {
                            ExcelWorksheet worksheet = package.Workbook.Worksheets[0];

                            for (int rowNum = 1; rowNum <= worksheet.Dimension.End.Row; rowNum++)
                            {
                                var rowCells = from cell in worksheet.Cells
                                               where (cell.Start.Row == rowNum)
                                               select cell;

                                if (!rowCells.Any(cell => cell.Value != null))
                                {
                                    worksheet.DeleteRow(rowNum);
                                }

                            }

                            var rowCount = worksheet.Dimension.Rows;

                            for (int row = 2; row <= rowCount; row++)
                            {
                                if (worksheet.Cells[row, 2].Value == null)
                                { break; }

                                string NewEmail = worksheet.Cells[row, 2].Value.ToString().Trim();

                                bool ValidateEmail = await IsValidEmail(NewEmail);

                                if (ValidateEmail == false)
                                {
                                    InValidMailsList.Add(NewEmail);
                                    continue;
                                }
                                if (!list.Exists(x => x.TdsMailId == NewEmail))
                                {
                                    var NewGuid = Guid.NewGuid().ToString();
                                    var NewEmailBody = GetTds.TdsEmailBody.Replace("InsertAPIKey", "" + NewGuid + "");
                                    list.Add(new AllTdsEmailViewModel
                                    {
                                        TdsUserName = worksheet.Cells[row, 1].Value.ToString().Trim(),
                                        TdsMailId = NewEmail,
                                        TdsPdfName = (worksheet.Cells[row, 3].Value ==  null ? null :  worksheet.Cells[row, 3].Value.ToString().Trim()),
                                        TdsId = uploadExcelFile.TdsId,
                                        TdsPdfUrl =(worksheet.Cells[row, 3].Value == null ?  null : fileVirtualPathQP + "/" + GetTds.TdsTxnId + "/" + worksheet.Cells[row, 3].Value.ToString().Trim() + ".pdf" ),
                                        IndividualEmailBody= GetTds.IsIndividualEmailBody == true ? NewEmailBody : null,
                                        RestructuringKey= GetTds.IsIndividualEmailBody == true ? NewGuid : null,
                                    });
                                }
                            }
                        }
                    }
                    if (list.Count() > 0)
                    {
                        var mapdata = _mapper.Map<List<TblAllTdsEmail>>(list);
                        await _dBContext.AddRangeAsync(mapdata);
                        await _dBContext.SaveChangesAsync();
                    }


                    string responseUrlL = fileVirtualPathQP + "/" + GetTds.TdsTxnId + "/" + uploadExcelFile.ExcelFile.FileName;

                    GetTds.TdsExcelName = ExcelfileName; GetTds.TdsExcelUrl = responseUrlL;
                    _dBContext.Update<TblTdsCertificate>(GetTds);
                    await _dBContext.SaveChangesAsync();

                    if (InValidMailsList.Count > 0)
                    {
                        return new ResponseModel { StatusCode = 200, ResponseMessage = "Excel uploaded successfull.Some Email ids from the excel are invalid,please recheck and upload with valid emails again" };
                    }
                }


                return new ResponseModel { StatusCode = 200, ResponseMessage = "Success" };
            }
            catch (Exception)
            {
                throw new Exception();
            }
        }
        public async Task<ResponseModel> TdsCertificateSendMails(int? TdsId)
        {
            try
            {
                var GetTds = await _dBContext.TblTdsCertificates.Where(f => f.TdsId == TdsId).FirstOrDefaultAsync();

                var GetEmails = await _dBContext.TblAllTdsEmails.Where(f => f.TdsId == TdsId).ToListAsync();

                if (GetTds == null)
                {
                    return new ResponseModel { ResponseMessage = "No Tds found", StatusCode = Convert.ToInt32(HttpStatusCode.NotFound) };
                }

                if (GetEmails.Count == 0)
                {
                    return new ResponseModel { ResponseMessage = "No Tds Emails found", StatusCode = Convert.ToInt32(HttpStatusCode.NotFound) };
                }

                if (GetTds.TdsCompleted == true)
                {
                    return new ResponseModel { ResponseMessage = "Maild already sended for this TDS", StatusCode = Convert.ToInt32(HttpStatusCode.IMUsed) };
                }

                GetEmails?.RemoveAll(f => f.TdsIsMailSended == true);

                string PhysicalPath = _configuration.GetSection("KeyValue").GetSection("PhysicalPath").Value;

                var SMTPData = _configuration.GetSection("SMTP");

                if (SMTPData == null)
                {
                    return new ResponseModel { ResponseMessage = "SMTP Not found", StatusCode = Convert.ToInt32(HttpStatusCode.NotFound) };
                }

                _Host = SMTPData.GetSection("Host").Value;

                _From = GetTds.TdsEmailFrom;

                _UserName = SMTPData.GetSection("MailUserName").Value;

                _Password = SMTPData.GetSection("MailPassword").Value;

                _Port = Convert.ToInt32(SMTPData.GetSection("Port").Value);

                SmtpClient smtpClient = new SmtpClient(_Host, _Port);

                smtpClient.Credentials = new System.Net.NetworkCredential(_UserName, _Password);

                smtpClient.EnableSsl = false;

                MailMessage mailMessage = new MailMessage();

                mailMessage.IsBodyHtml = true;

                mailMessage.From = new MailAddress(_From);

                mailMessage.Subject = GetTds.TdsSubject;

                if (!string.IsNullOrEmpty(GetTds.TdsEmailCc))
                {
                    mailMessage.CC.Add(GetTds.TdsEmailCc);
                }

                //List<int> AllTdsIds = new List<int>();

                if (GetTds.TblAllTdsEmails.Count > 0)
                {
                    foreach (var item in GetEmails)
                    {
                        System.Net.Mail.Attachment attachment = null;

                        mailMessage.Body = GetTds.IsIndividualEmailBody==true? item .IndividualEmailBody.ToString() : GetTds.TdsEmailBody.ToString();

                        mailMessage.To.Add(item.TdsMailId);

                        if (File.Exists(PhysicalPath + GetTds.TdsTxnId + "\\" + item.TdsPdfName + ".pdf"))
                        {
                            attachment = new System.Net.Mail.Attachment(PhysicalPath + GetTds.TdsTxnId + "\\" + item.TdsPdfName + ".pdf");

                            mailMessage.Attachments.Add(attachment);
                        }

                        smtpClient.Send(mailMessage);

                        item.TdsIsMailSended = true; item.UpdatedBy = "Administrator"; item.UpdatedOn = DateTime.Now;

                        _dBContext.Update<TblAllTdsEmail>(item);

                        await _dBContext.SaveChangesAsync();

                        //AllTdsIds.Add(item.MailId);

                        WriteLog(mailMessage.To[0] + ":" + mailMessage.Body, GetTds.TdsTxnId);

                        mailMessage.To.Remove(mailMessage.To[0]);

                        if (attachment != null)
                        {
                            mailMessage.Attachments.Remove(attachment);
                        }
                    }

                    //if (AllTdsIds.Count == 0)
                    //{
                    //    return new ResponseModel { ResponseMessage = "No respective attachements or mails not found", StatusCode = Convert.ToInt32(HttpStatusCode.NotFound) };
                    //}

                    if (_dBContext.TblAllTdsEmails.Where(f => f.TdsId == TdsId && f.TdsIsMailSended==false).Count()==0)
                    {
                        GetTds.TdsCompleted = true; GetTds.TdsUpdatedOn = DateTime.Now; GetTds.TdsUpdatedBy = "Administrator";

                        _dBContext.Update<TblTdsCertificate>(GetTds);

                        await _dBContext.SaveChangesAsync();
                    }
                }
                return new ResponseModel { ResponseMessage = "Mails sented successfully...", StatusCode = Convert.ToInt32(HttpStatusCode.Created) };

            }
            catch (Exception Ex)
            {

                throw Ex;
            }
        }
        public static void WriteLog(string strLog, string UniqueName)
        {
            StreamWriter log;
            FileStream fileStream = null;
            DirectoryInfo logDirInfo = null;
            FileInfo logFileInfo;
            strLog = Regex.Replace(strLog, "<.*?>", String.Empty);

            string logFilePath = "D:\\BulkMailLogs\\" + DateTime.Now.ToString("dd-MM-yyyy") + "\\";
            logFilePath = logFilePath + UniqueName + "." + "txt";
            logFileInfo = new FileInfo(logFilePath);
            logDirInfo = new DirectoryInfo(logFileInfo.DirectoryName);
            if (!logDirInfo.Exists) logDirInfo.Create();
            if (!logFileInfo.Exists)
            {
                fileStream = logFileInfo.Create();
            }
            else
            {
                fileStream = new FileStream(logFilePath, FileMode.Append);
            }
            log = new StreamWriter(fileStream);
            log.WriteLine(strLog);
            logFilePath = "";
            log.Close();
        }

        public async Task<List<AllTdsEmailViewModel>> GetAllEmailsList(int? TdsId)
        {
            try
            {
                var GetList = await _dBContext.TblAllTdsEmails.Where(f => f.TdsId == TdsId).ToListAsync();
                var mapdata = _mapper.Map<List<AllTdsEmailViewModel>>(GetList);
                return mapdata;
            }
            catch (Exception Ex)
            {
                throw Ex;
            }
        }

        public async Task<ResponseModel> DeleteTdsEmail(int? MailId)
        {
            try
            {
                var GetUserMail = await _dBContext.TblAllTdsEmails.Where(f => f.MailId == MailId).Include(f => f.Tds).FirstOrDefaultAsync();

                if (GetUserMail == null)
                {
                    return new ResponseModel { ResponseMessage = "No emails found for the TDS", StatusCode = Convert.ToInt32(HttpStatusCode.NotFound) };
                }

                if (GetUserMail.TdsIsMailSended == true)
                {
                    return new ResponseModel { ResponseMessage = "Deletion failed due to mail already sented", StatusCode = Convert.ToInt32(HttpStatusCode.Forbidden) };
                }

                _dBContext.Remove(GetUserMail);

                await _dBContext.SaveChangesAsync();

                int? Count = _dBContext.TblAllTdsEmails.Where(f => f.TdsId == GetUserMail.Tds.TdsId && f.TdsPdfName == GetUserMail.TdsPdfName)?.Count() ?? 0;

                string PhysicalPath = _configuration.GetSection("KeyValue").GetSection("PhysicalPath").Value;

                if (Count == 1)
                {
                    if (File.Exists(PhysicalPath + GetUserMail.Tds.TdsTxnId + "\\" + GetUserMail.TdsPdfName + ".pdf"))
                    {
                        System.IO.File.Delete(PhysicalPath + GetUserMail.Tds.TdsTxnId + "\\" + GetUserMail.TdsPdfName + ".pdf");
                    }
                }
                return new ResponseModel { ResponseMessage = "Deleted successfully", StatusCode = Convert.ToInt32(HttpStatusCode.Created) };
            }
            catch (Exception Ex)
            {

                throw Ex;
            }
        }
        public async Task<ResponseModel> DeletePdfFile(int? MailId)
        {
            try
            {
                var GetTds = await _dBContext.TblAllTdsEmails.Where(f => f.MailId == MailId).Include(f => f.Tds).FirstOrDefaultAsync();

                if (GetTds == null)
                {
                    return new ResponseModel { ResponseMessage = "No emails found for the TDS", StatusCode = Convert.ToInt32(HttpStatusCode.NotFound) };
                }

                if (GetTds.TdsIsMailSended == true)
                {
                    return new ResponseModel { ResponseMessage = "Pdf deletion failed due to mail already sented", StatusCode = Convert.ToInt32(HttpStatusCode.Forbidden) };
                }

                int? Count = _dBContext.TblAllTdsEmails.Where(f => f.TdsId == GetTds.Tds.TdsId && f.TdsPdfName == GetTds.TdsPdfName)?.Count() ?? 0;

                string PhysicalPath = _configuration.GetSection("KeyValue").GetSection("PhysicalPath").Value;

                if (Count == 1)
                {
                    if (File.Exists(PhysicalPath + GetTds.Tds.TdsTxnId + "\\" + GetTds.TdsPdfName + ".pdf"))
                    {
                        System.IO.File.Delete(PhysicalPath + GetTds.Tds.TdsTxnId + "\\" + GetTds.TdsPdfName + ".pdf");
                    }
                }

                GetTds.UpdatedBy = "Administrator";
                GetTds.UpdatedOn = DateTime.Now;
                GetTds.TdsPdfName = null;
                GetTds.TdsPdfUrl = null;
                _dBContext.Update(GetTds);
                await _dBContext.SaveChangesAsync();
                return new ResponseModel { ResponseMessage = "PDF deleted and respective mail(s) updated", StatusCode = Convert.ToInt32(HttpStatusCode.NotFound) };
            }
            catch (Exception Ex)
            {

                throw Ex;
            }
        }
        
        public async Task<ResponseModel> UpdateTDSEmailData(EmailUpdateModel model)
        {
            try
            {
                var Getdata = await _dBContext.TblAllTdsEmails.Where(f => f.MailId == model.MailId).Include(f=>f.Tds).FirstOrDefaultAsync();

                if (Getdata==null)
                {
                    return new ResponseModel { ResponseMessage = "Mailing Tds Not Found", StatusCode = Convert.ToInt32(HttpStatusCode.NotFound) };
                }

                if (Getdata.TdsIsMailSended == true)
                {
                    return new ResponseModel { ResponseMessage = "Update failed due to Mail already sended", StatusCode = Convert.ToInt32(HttpStatusCode.IMUsed) };
                }
                _mapper.Map<EmailUpdateModel, TblAllTdsEmail>(model, Getdata);

                if (Getdata.TdsPdfName!=null)
                {
                    string filePath = _configuration.GetSection("KeyValue").GetSection("PhysicalPath").Value;

                    string fileVirtualPathQP = _configuration.GetSection("KeyValue").GetSection("QPVirtualPath").Value;

                    Getdata.TdsPdfUrl = fileVirtualPathQP + "/" + Getdata.Tds?.TdsTxnId + "/" + Getdata.TdsPdfName.ToString().Trim() + ".pdf";
                }

                 Getdata.UpdatedBy = "Administrator";Getdata.UpdatedOn = DateTime.Now;

                _dBContext.Update<TblAllTdsEmail>(Getdata);

                await _dBContext.SaveChangesAsync();

                return new ResponseModel { ResponseMessage = "Updated succesfully", StatusCode = Convert.ToInt32(HttpStatusCode.Created) };
            }
            catch (Exception Ex)
            {

                throw Ex;
            }
        }

        public async Task<bool> IsValidEmail(string email)
        {
            try
            {
                Regex rx = new Regex(
            @"^[-!#$%&'*+/0-9=?A-Z^_a-z{|}~](\.?[-!#$%&'*+/0-9=?A-Z^_a-z{|}~])*@[a-zA-Z](-?[a-zA-Z0-9])*(\.[a-zA-Z](-?[a-zA-Z0-9])*)+$");
                return rx.IsMatch(email);
            }
            catch (FormatException)
            {
                return false;
            }
        }
    }
}
