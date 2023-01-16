using Business.Aspects.Secured;
using Business.Repositories.EmailParameterRepository.Constans;
using Business.Repositories.EmailParameterRepository.Validation;
using Core.Aspects.Caching;
using Core.Aspects.Validation;
using Core.Utilities.Result.Abstract;
using Core.Utilities.Result.Concrete;
using DataAccess.Abstract;
using Entities.Concrete;
using System.Net;
using System.Net.Mail;

namespace Business.Repositories.EmailParameterRepository
{
    public class EmailParameterManager : IEmailParameterService
    {
        private readonly IEmailParameterDal _emailParameterDal;

        public EmailParameterManager(IEmailParameterDal emailParameterDal)
        {
            _emailParameterDal = emailParameterDal;
        }

        [SecuredAspect()]
        [ValidationAspect(typeof(EmailParameterValidator))]
        //[RemoveCacheAspect("IEmailParameterService.Get")]
        public async Task Add(EmailParameter emailParameter)
        {
            await _emailParameterDal.AddAsync(emailParameter);

        }

        [SecuredAspect()]
        //[RemoveCacheAspect("IEmailParameterService.Get")]
        public async Task Delete(EmailParameter emailParameter)
        {
            await _emailParameterDal.DeleteAsync(emailParameter);
        }

        public async Task<EmailParameter> GetById(int id)
        {
            return await _emailParameterDal.GetAsync(p => p.Id == id);
        }


        //[CacheAspect()]
        public async Task<List<EmailParameter>> GetList()
        {
            return await _emailParameterDal.GetAllAsync();
        }

        public async Task SendEmail(EmailParameter emailParameter, string body, string subject, string emails)
        {
            using (MailMessage mail = new MailMessage())
            {
                string[] setEmails = emails.Split(",");
                mail.From = new MailAddress(emailParameter.Email);
                foreach (var email in setEmails)
                {
                    mail.To.Add(email);
                }
                mail.Subject = subject;
                mail.Body = body;
                mail.IsBodyHtml = emailParameter.Html;
                //mail.Attachments.Add();
                using (SmtpClient smtp = new SmtpClient(emailParameter.Smtp))
                {
                    smtp.UseDefaultCredentials = false;
                    smtp.Credentials = new NetworkCredential(emailParameter.Email, emailParameter.Password);
                    smtp.EnableSsl = emailParameter.SSL;
                    smtp.Port = emailParameter.Port;
                    await smtp.SendMailAsync(mail);
                }
            }
        }

        [SecuredAspect()]
        [ValidationAspect(typeof(EmailParameterValidator))]
        //[RemoveCacheAspect("IEmailParameterService.Get")]
        public async Task Update(EmailParameter emailParameter)
        {
            await _emailParameterDal.UpdateAsync(emailParameter);
        }
    }
}
