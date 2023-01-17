using Core.Utilities.Result.Abstract;
using Entities.Concrete;
using Entities.Dtos;

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
        Task<List<User>> GetList();
        Task<User> GetByEmail(string email);
        Task<List<OperationClaim>> GetUserOperationClaims(int userId);
        Task<AuthResponseDto> GetById(int id);
        Task<User> GetByIdForAuth(int id);
    }
}
