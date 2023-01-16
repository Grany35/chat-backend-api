using Business.Aspects.Secured;
using Business.Repositories.OperationClaimRepository.Constans;
using Business.Repositories.OperationClaimRepository.Validation;
using Core.Aspects.Caching;
using Core.Aspects.Performance;
using Core.Aspects.Validation;
using Core.Business;
using Core.Utilities.Result.Abstract;
using Core.Utilities.Result.Concrete;
using DataAccess.Abstract;
using Entities.Concrete;

namespace Business.Repositories.OperationClaimRepository
{
    public class OperationClaimManager : IOperationClaimService
    {
        private readonly IOperationClaimDal _operationClaimDal;
        public OperationClaimManager(IOperationClaimDal operationClaimDal)
        {
            _operationClaimDal = operationClaimDal;
        }

        [SecuredAspect()]
        [ValidationAspect(typeof(OperationClaimValidator))]
        //[RemoveCacheAspect("IOperationClaimService.Get")]
        public async Task Add(OperationClaim operationClaim)
        {
            await IsNameExistForAdd(operationClaim.Name);

            await _operationClaimDal.AddAsync(operationClaim);
        }

        [SecuredAspect()]
        [ValidationAspect(typeof(OperationClaimValidator))]
        //[RemoveCacheAspect("IOperationClaimService.Get")]
        public async Task Update(OperationClaim operationClaim)
        {
            await IsNameExistForUpdate(operationClaim);

            await _operationClaimDal.UpdateAsync(operationClaim);
        }

        [SecuredAspect()]
        //[RemoveCacheAspect("IOperationClaimService.Get")]
        public async Task Delete(OperationClaim operationClaim)
        {
            await _operationClaimDal.DeleteAsync(operationClaim);
        }

        //[CacheAspect()]
        [PerformanceAspect()]
        public async Task<List<OperationClaim>> GetList()
        {
            return await _operationClaimDal.GetAllAsync();
        }

        public async Task<OperationClaim> GetById(int id)
        {
            var result = await _operationClaimDal.GetAsync(p => p.Id == id);
            return result;
        }

        public async Task<OperationClaim> GetByIdForUserService(int id)
        {
            var result = await _operationClaimDal.GetAsync(p => p.Id == id);
            return result;
        }

        private async Task IsNameExistForAdd(string name)
        {
            var result = await _operationClaimDal.GetAsync(p => p.Name == name);
            if (result != null)
            {
                throw new BusinessException(OperationClaimMessages.NameIsNotAvaible);
            }
        }

        private async Task IsNameExistForUpdate(OperationClaim operationClaim)
        {
            var currentOperationClaim = await _operationClaimDal.GetAsync(p => p.Id == operationClaim.Id);
            if (currentOperationClaim.Name != operationClaim.Name)
            {
                var result = await _operationClaimDal.GetAsync(p => p.Name == operationClaim.Name);
                if (result != null)
                {
                    throw new BusinessException(OperationClaimMessages.NameIsNotAvaible);
                }
            }
        }
    }
}
