using Core.Utilities.Result.Abstract;
using Entities.Concrete;

namespace Business.Repositories.EmailParameterRepository
{
    public interface IEmailParameterService
    {
        Task Add(EmailParameter emailParameter);
        Task Update(EmailParameter emailParameter);
        Task Delete(EmailParameter emailParameter);
        Task<List<EmailParameter>> GetList();
        Task<EmailParameter> GetById(int id);
        Task SendEmail(EmailParameter emailParameter, string body, string subject, string emails);
    }
}
