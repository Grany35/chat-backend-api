using Core.Utilities.Result.Abstract;
using Entities.Concrete;

namespace Business.Repositories.UserOperationClaimRepository
{
    public interface IUserOperationClaimService
    {
        Task Add(UserOperationClaim userOperationClaim);
        Task Update(UserOperationClaim userOperationClaim);
        Task Delete(UserOperationClaim userOperationClaim);
        Task<List<UserOperationClaim>> GetList();
        Task<UserOperationClaim> GetById(int id);
    }
}
