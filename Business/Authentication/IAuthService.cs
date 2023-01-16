using Core.Utilities.Result.Abstract;
using Core.Utilities.Security.JWT;
using Entities.Dtos;

namespace Business.Abstract
{
    public interface IAuthService
    {
        Task Register(RegisterAuthDto registerDto);
        Task<AuthResponseDto> Login(LoginAuthDto loginDto);
    }
}
