using Core.Utilities.Result.Abstract;
using Entities.Concrete;

namespace Business.Repositories.OperationClaimRepository
{
    public interface IOperationClaimService
    {
        Task Add(OperationClaim operationClaim);
        Task Update(OperationClaim operationClaim);
        Task Delete(OperationClaim operationClaim);
        Task<List<OperationClaim>> GetList();
        Task<OperationClaim> GetById(int id);
        Task<OperationClaim> GetByIdForUserService(int id);
    }
}
