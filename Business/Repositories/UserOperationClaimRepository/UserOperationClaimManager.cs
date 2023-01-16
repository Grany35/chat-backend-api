using Business.Repositories.OperationClaimRepository;
using Business.Repositories.UserOperationClaimRepository.Constans;
using Business.Repositories.UserOperationClaimRepository.Validation;
using Business.Repositories.UserRepository;
using Core.Aspects.Validation;
using Core.Business;
using Core.Utilities.Result.Abstract;
using Core.Utilities.Result.Concrete;
using DataAccess.Abstract;
using Entities.Concrete;

namespace Business.Repositories.UserOperationClaimRepository
{
    public class UserOperationClaimManager : IUserOperationClaimService
    {
        private readonly IUserOperationClaimDal _userOperationClaimDal;
        private readonly IOperationClaimService _operationClaimService;
        private readonly IUserService _userService;

        public UserOperationClaimManager(IUserOperationClaimDal userOperationClaimDal, IOperationClaimService operationClaimService, IUserService userService)
        {
            _userOperationClaimDal = userOperationClaimDal;
            _operationClaimService = operationClaimService;
            _userService = userService;
        }


        public async Task Delete(UserOperationClaim userOperationClaim)
        {
            await _userOperationClaimDal.DeleteAsync(userOperationClaim);
        }

        public async Task<UserOperationClaim> GetById(int id)
        {
            return await _userOperationClaimDal.GetAsync(p => p.Id == id);
        }

        public async Task<List<UserOperationClaim>> GetList()
        {
            return await _userOperationClaimDal.GetAllAsync();
        }

        [ValidationAspect(typeof(UserOperationClaimValidator))]
        public async Task Update(UserOperationClaim userOperationClaim)
        {
            await IsUserExist(userOperationClaim.UserId);
                await IsOperationClaimExist(userOperationClaim.OperationClaimId);
                await IsOperationSetExistForUpdate(userOperationClaim);

            await _userOperationClaimDal.UpdateAsync(userOperationClaim);
        }

        [ValidationAspect(typeof(UserOperationClaimValidator))]
        public async Task Add(UserOperationClaim userOperationClaim)
        {

            await IsUserExist(userOperationClaim.UserId);
                await IsOperationClaimExist(userOperationClaim.OperationClaimId);
                await IsOperationSetExistForAdd(userOperationClaim);

            await _userOperationClaimDal.AddAsync(userOperationClaim);
        }

        public async Task IsUserExist(int userId)
        {
            var result = await _userService.GetByIdForAuth(userId);
            if (result == null)
            {
                throw new BusinessException(UserOperationClaimMessages.UserNotExist);
            }
        }

        public async Task IsOperationClaimExist(int operationClaimId)
        {
            var result = await _operationClaimService.GetByIdForUserService(operationClaimId);
            if (result == null)
            {
                throw new BusinessException(UserOperationClaimMessages.OperationClaimNotExist);
            }
        }

        public async Task IsOperationSetExistForAdd(UserOperationClaim userOperationClaim)
        {
            var result = await _userOperationClaimDal.GetAsync(p => p.UserId == userOperationClaim.UserId && p.OperationClaimId == userOperationClaim.OperationClaimId);
            if (result != null)
            {
                throw new BusinessException(UserOperationClaimMessages.OperationClaimSetExist);
            }
        }

        private async Task IsOperationSetExistForUpdate(UserOperationClaim userOperationClaim)
        {
            var currentUserOperationClaim = await _userOperationClaimDal.GetAsync(p => p.Id == userOperationClaim.Id);
            if (currentUserOperationClaim.UserId != userOperationClaim.UserId || currentUserOperationClaim.OperationClaimId != userOperationClaim.OperationClaimId)
            {
                var result = await _userOperationClaimDal.GetAsync(p => p.UserId == userOperationClaim.UserId && p.OperationClaimId == userOperationClaim.OperationClaimId);
                if (result != null)
                {
                    throw new BusinessException(UserOperationClaimMessages.OperationClaimSetExist);
                }
            }
        }
    }
}
