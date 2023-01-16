using Business.Abstract;
using Business.Aspects.Secured;
using Business.Repositories.UserRepository;
using Business.ValidationRules.FluentValidation;
using Core.Aspects.Validation;
using Core.Business;
using Core.Utilities.Hashing;
using Core.Utilities.Result.Abstract;
using Core.Utilities.Result.Concrete;
using Core.Utilities.Security.JWT;
using Entities.Concrete;
using Entities.Dtos;

namespace Business.Authentication
{
    public class AuthManager : IAuthService
    {
        private readonly IUserService _userService;
        private readonly ITokenHandler _tokenHandler;

        public AuthManager(IUserService userService, ITokenHandler tokenHandler)
        {
            _userService = userService;
            _tokenHandler = tokenHandler;
        }
        
        public async Task<Token> Login(LoginAuthDto loginDto)
        {
            var user = await _userService.GetByEmail(loginDto.Email);
            if (user == null)
                throw new BusinessException("Kullanıcı maili sistemde bulunamadı!");

            //if (!user.IsConfirm)
            //    return new ErrorDataResult<Token>("Kullanıcı maili onaylanmamış!");

            var result = HashingHelper.VerifyPasswordHash(loginDto.Password, user.PasswordHash, user.PasswordSalt);
            List<OperationClaim> operationClaims = await _userService.GetUserOperationClaims(user.Id);

            if (result)
            {
                Token token = new();
                token = _tokenHandler.CreateToken(user, operationClaims);
                return  token;
            }
            throw new BusinessException("Kullanıcı maili ya da şifre bilgisi yanlış");
        }

        [ValidationAspect(typeof(AuthValidator))]
        public async Task Register(RegisterAuthDto registerDto)
        {
            await CheckIfEmailExists(registerDto.Email);

             await _userService.Add(registerDto);
        }

        private async Task CheckIfEmailExists(string email)
        {
            var list = await _userService.GetByEmail(email);
            if (list != null)
            {
                throw new BusinessException("Bu mail adresi daha önce kullanılmış");
            }
        }
    }
}
