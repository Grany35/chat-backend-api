using Core.Utilities.Params;
using Core.Utilities.Result.Abstract;
using Entities.Concrete;
using Entities.Dtos;
using Microsoft.AspNetCore.Http;

namespace Business.Repositories.UserRepository
{
    public interface IUserService
    {
        Task Add(RegisterAuthDto authDto);
        Task Update(User user);
        Task ConfirmUser(string confirmValue);
        //Task<IResult> SendConfirmUserMail(string email);
        //Task<IResult> SendForgotPasswordMail(string email);
        Task ChangePassword(UserChangePasswordDto userChangePasswordDto);
        Task CreateANewPassword(CreateANewPasswordDto createANewPasswordDto);
        Task Delete(User user);
        Task<List<UserDto>> GetList(UserParams userParams);
        Task<User> GetByEmail(string email);
        Task<List<OperationClaim>> GetUserOperationClaims(int userId);
        Task<AuthResponseDto> GetById(int id);
        Task<User> GetByIdForAuth(int id);
        Task UpdateUserSettings(UserUpdateDto dto);
        Task<string> UpdateUserPhoto(ProfileImageUpdateDto dto);
    }
}
