using Business.Abstract;
using Business.Aspects.Secured;
using Business.Repositories.EmailParameterRepository;
using Business.Repositories.UserRepository.Contans;
using Business.Repositories.UserRepository.Validation;
using Core.Aspects.Caching;
using Core.Aspects.Performance;
using Core.Aspects.Validation;
using Core.Business;
using Core.Utilities.Hashing;
using Core.Utilities.Result.Abstract;
using Core.Utilities.Result.Concrete;
using DataAccess.Abstract;
using Entities.Concrete;
using Entities.Dtos;
using Microsoft.EntityFrameworkCore;

namespace Business.Repositories.UserRepository
{
    public class UserManager : IUserService
    {
        private readonly IUserDal _userDal;
        private readonly IFileService _fileService;
        private readonly IEmailParameterService _emailParameterService;
        private readonly IUserOperationClaimDal _userOperationClaimDal;

        public UserManager(IUserDal userDal, IFileService fileService, IEmailParameterService emailParameterService, IUserOperationClaimDal userOperationClaimDal)
        {
            _userDal = userDal;
            _fileService = fileService;
            _emailParameterService = emailParameterService;
            _userOperationClaimDal = userOperationClaimDal;
        }

        //[RemoveCacheAspect("IUserService.Get")]
        public async Task Add(RegisterAuthDto registerDto)
        {
            string confirmValue = await CreateConfirmValue();

            var user = CreateUser(registerDto);

            user.ConfirmValue = confirmValue;

            await _userDal.AddAsync(user);

            //await SendConfirmUserMail(user.Email);
        }        

        public async Task<string> CreateConfirmValue()
        {
        again:;
            string value = Guid.NewGuid().ToString();
            var result = await _userDal.GetAsync(p => p.ConfirmValue == value);
            if (result != null)
            {
                goto again;
            }

            return value;
        }

        private static User CreateUser(RegisterAuthDto registerDto)
        {
            byte[] passwordHash, paswordSalt;
            HashingHelper.CreatePassword(registerDto.Password, out passwordHash, out paswordSalt);

            User user = new User()
            {
                Email=registerDto.Email,
                Name=registerDto.Name,
                PasswordHash=passwordHash,
                PasswordSalt=paswordSalt,
            };
                
            return user;
        }

        public async Task<User> GetByEmail(string email)
        {
            var result = await _userDal.GetAsync(p => p.Email == email);
            return result;
        }

        [SecuredAspect()]
        [ValidationAspect(typeof(UserValidator))]
        //[RemoveCacheAspect("IUserService.Get")]
        public async Task Update(User user)
        {
            await _userDal.UpdateAsync(user);
        }

        [SecuredAspect()]
        //[RemoveCacheAspect("IUserService.Get")]
        public async Task Delete(User user)
        {
            await _userDal.DeleteAsync(user);
        }

        //[SecuredAspect()]
        //[CacheAspect(60)]
        [PerformanceAspect(3)]
        public async Task<List<User>> GetList()
        {
            return await _userDal.GetAllAsync();
        }

        public async Task<User> GetById(int id)
        {
            return await _userDal.GetAsync(p => p.Id == id);
        }

        public async Task<User> GetByIdForAuth(int id)
        {
            return await _userDal.GetAsync(p => p.Id == id);
        }

        [SecuredAspect()]
        [ValidationAspect(typeof(UserChangePasswordValidator))]
        public async Task ChangePassword(UserChangePasswordDto userChangePasswordDto)
        {
            var user = await _userDal.GetAsync(p => p.Id == userChangePasswordDto.UserId);
            bool result = HashingHelper.VerifyPasswordHash(userChangePasswordDto.CurrentPassword, user.PasswordHash, user.PasswordSalt);
            if (!result)
            {
                throw new BusinessException(UserMessages.WrongCurrentPassword);
            }

            byte[] passwordHash, paswordSalt;
            HashingHelper.CreatePassword(userChangePasswordDto.NewPassword, out passwordHash, out paswordSalt);

            user.PasswordHash = passwordHash;
            user.PasswordSalt = paswordSalt;
            await _userDal.UpdateAsync(user);
        }

        public async Task CreateANewPassword(CreateANewPasswordDto createANewPasswordDto)
        {
            var user = await _userDal.GetAsync(p => p.ForgotPasswordValue == createANewPasswordDto.ForgotPasswordValue);

            if (user == null)
                throw new BusinessException(UserMessages.ForgotPasswordValueIsNotValid);


                IsForgotPasswordValueUsed(user);
                //IsForgotPasswordDateEnded(user);

            byte[] passwordHash, paswordSalt;
            HashingHelper.CreatePassword(createANewPasswordDto.NewPassword, out passwordHash, out paswordSalt);

            user.PasswordHash = passwordHash;
            user.PasswordSalt = paswordSalt;
            await _userDal.UpdateAsync(user);
        }        

        public static void IsForgotPasswordValueUsed(User user)
        {            
            if (user.IsForgotPasswordComplete)
                throw new BusinessException(UserMessages.ForgotPasswordValueIsUsed);
        }

        //public static IResult IsForgotPasswordDateEnded(User user)
        //{
        //    DateTime date1 = DateTime.Now;
        //    DateTime date2 = Convert.ToDateTime(user.ForgotPasswordRequestDate);
        //    TimeSpan result = date2 - date1;
        //    var remainMin = Convert.ToInt16(result.Minutes.ToString());
        //    if (remainMin < -5)
        //    {
        //        return new ErrorResult(UserMessages.ForgotPasswordValueTimeIsEnded);
        //    }

        //    return new SuccessResult();
        //}

        public async Task<List<OperationClaim>> GetUserOperationClaims(int userId)
        {
            List<UserOperationClaim> userOperationClaims =await _userOperationClaimDal.GetAllAsync(x => x.UserId == userId,
                include: i => i.Include(i => i.OperationClaim));
            return userOperationClaims.Select(x => new OperationClaim
            {
                Id = x.OperationClaimId,
                Name = x.OperationClaim.Name
            }).ToList();
        }

        public async Task ConfirmUser(string confirmValue)
        {
            var user = await _userDal.GetAsync(p=> p.ConfirmValue == confirmValue);
            if (user.IsConfirm)
            {
                throw new BusinessException(UserMessages.UserAlreadyConfirm);
            }

            user.IsConfirm = true;
            await _userDal.UpdateAsync(user);
        }

        //public async Task<IResult> SendForgotPasswordMail(string email)
        //{
        //    var user = await _userDal.GetAsync(p => p.Email == email);
        //    var result = BusinessRules.Run(CheckForgotPasswordIsRequestActive(user));
        //    if (result != null)
        //    {
        //        return result;
        //    }

        //    string forgotPasswordValue = await CreateForgotPasswordValue();
            

        //    var emailParameter = await _emailParameterService.get();
        //    if (emailParameter != null)
        //    {
        //        user.ForgotPasswordValue = forgotPasswordValue;
        //        user.ForgotPasswordRequestDate = DateTime.Now;
        //        user.IsForgotPasswordComplete = false;
        //        await _userDal.Update(user);


        //        string subject = "Şifre Hatırlatma Maili";
        //        string body = ForgotPasswordEmailHtmlBody(forgotPasswordValue); ;
        //        await _emailParameterService.SendEmail(emailParameter, body, subject, email);
        //    }

        //    return new SuccessResult(UserMessages.ForgotPasswordMailSendSuccessiful);
        //}

        public IResult CheckForgotPasswordIsRequestActive(User user)
        {
            if (!user.IsForgotPasswordComplete)
            {
                if (user.ForgotPasswordRequestDate != null)
                {
                    DateTime date1 = DateTime.Now;
                    DateTime date2 = Convert.ToDateTime(user.ForgotPasswordRequestDate);
                    TimeSpan result = date2 - date1;
                    var remainMin = Convert.ToInt16(result.Minutes.ToString());
                    if (remainMin >= -5)
                    {
                        return new ErrorResult(UserMessages.AlreadySendForgotPasswordMail);
                    }
                }
            }

            return new SuccessResult();
        }

        public async Task<string> CreateForgotPasswordValue()
        {
        again:;
            string value = Guid.NewGuid().ToString();
            var result = await _userDal.GetAsync(p => p.ForgotPasswordValue == value);
            if (result != null)
            {
                goto again;
            }

            return value;
        }

        public string ForgotPasswordEmailHtmlBody(string forgotPasswordValue)
        {
            string css = "{text - decoration: underline!important}";
            string body = $"<!doctype html><html lang='en-US'><head><meta content = 'text/html; charset=utf-8' http - equiv = 'Content-Type'/>    <title> Şifre Yenileme İsteği </title><meta name = 'description' content = 'Şifre Yenileme İsteği.'><style type = 'text/css'> a:hover {css}  </style></head><body marginheight = '0' topmargin = '0' marginwidth = '0' style = 'margin: 0px; background-color: #f2f3f8;' leftmargin = '0'><!--100 % body table--><table cellspacing = '0' border = '0' cellpadding = '0' width = '100%' bgcolor = '#f2f3f8' style = '@import url(https://fonts.googleapis.com/css?family=Rubik:300,400,500,700|Open+Sans:300,400,600,700); font-family: 'Open Sans', sans-serif;'><tr><td><table style = 'background-color: #f2f3f8; max-width:670px;  margin:0 auto;' width = '100%' border = '0' align = 'center' cellpadding = '0' cellspacing = '0'><tr><td style = 'height:80px;' > &nbsp;</td></tr>                   <tr><td style = 'text-align:center;'></td></tr><tr> <td style = 'height:20px;' > &nbsp;</td></tr><tr><td><table width = '95%' border = '0' align = 'center' cellpadding = '0' cellspacing = '0' style = 'max-width:670px;background:#fff; border-radius:3px; text-align:center;-webkit-box-shadow:0 6px 18px 0 rgba(0,0,0,.06);-moz-box-shadow:0 6px 18px 0 rgba(0,0,0,.06);box-shadow:0 6px 18px 0 rgba(0,0,0,.06);'><tr><td style = 'height:40px;' > &nbsp;</td></tr><tr><td style = 'padding:0 35px;'><h1 style = 'color:#1e1e2d; font-weight:500; margin:0;font-size:32px;font-family:'Rubik',sans-serif;'> Şifrenizi yenilemek için talepte bulundunuz</h1><span style = 'display:inline-block; vertical-align:middle; margin:29px 0 26px; border-bottom:1px solid #cecece; width:100px;'></span>  <p style = 'color:#455056; font-size:15px;line-height:24px; margin:0;'>Güvenlik sebebiyle eski şifrenizi burada gösteremiyoruz. Yeni şifre oluşturmak için 5 dakika içerisinde aşağıdaki linke tıklayarak açılacak sayfadan yeni şifrenizi belirleyebilirsiniz</p><a href='https://www.sitem.com/pasword/reset/{forgotPasswordValue}' style = 'background:#20e277;text-decoration:none !important; font-weight:500; margin-top:35px; color:#fff;text-transform:uppercase; font-size:14px;padding:10px 24px;display:inline-block;border-radius:50px;'> Şifreyi Sıfırla </a></td></tr><tr><td style = 'height:40px;'> &nbsp;</td></tr></table></td><tr><td style = 'height:20px;'> &nbsp;</td></tr><tr><td style = 'text-align:center;'><p style = 'font-size:14px; color:rgba(69, 80, 86, 0.7411764705882353); line-height:18px; margin:0 0 0;' > </p></td></tr><tr><td style = 'height:80px;'>&nbsp;</td></tr></table></td></tr></table><!--/ 100 % body table--></body></html>";

            return body;
        }
        public string ConfirmUserHtmlBody(string confirmValue)
        {
            string css = "{text - decoration: underline!important}";
            string body = $"<!doctype html><html lang='en-US'><head><meta content = 'text/html; charset=utf-8' http - equiv = 'Content-Type'/>    <title > Kullanıcı Onaylama </title><meta name = 'description' content = 'Kullanıcı Onaylama.'><style type = 'text/css'> a:hover {css}  </style></head><body marginheight = '0' topmargin = '0' marginwidth = '0' style = 'margin: 0px; background-color: #f2f3f8;' leftmargin = '0'><!--100 % body table--><table cellspacing = '0' border = '0' cellpadding = '0' width = '100%' bgcolor = '#f2f3f8' style = '@import url(https://fonts.googleapis.com/css?family=Rubik:300,400,500,700|Open+Sans:300,400,600,700); font-family: 'Open Sans', sans-serif;'><tr><td><table style = 'background-color: #f2f3f8; max-width:670px;  margin:0 auto;' width = '100%' border = '0' align = 'center' cellpadding = '0' cellspacing = '0'><tr><td style = 'height:80px;' > &nbsp;</td></tr><tr><td style = 'text-align:center;'></td></tr><tr> <td style = 'height:20px;' > &nbsp;</td></tr><tr><td><table width = '95%' border = '0' align = 'center' cellpadding = '0' cellspacing = '0' style = 'max-width:670px;background:#fff; border-radius:3px; text-align:center;-webkit-box-shadow:0 6px 18px 0 rgba(0,0,0,.06);-moz-box-shadow:0 6px 18px 0 rgba(0,0,0,.06);box-shadow:0 6px 18px 0 rgba(0,0,0,.06);'><tr><td style = 'height:40px;'>&nbsp;</td></tr><tr><td style = 'padding:0 35px;'><h1 style = 'color:#1e1e2d; font-weight:500; margin:0;font-size:32px;font-family:'Rubik',sans-serif;'>Kullanıcı Onaylama Maili</h1><span style = 'display:inline-block; vertical-align:middle; margin:29px 0 26px; border-bottom:1px solid #cecece; width:100px;'></span> <p style = 'color:#455056; font-size:15px;line-height:24px; margin:0;'>Kullanıcı kaydınızı doğrulamak için aşağıdaki linke tıklayarak kullanıcı kaydını aktif edebilirsiniz</p><a href='https://www.sitem.com/user/confirm/{confirmValue}' style = 'background:#20e277;text-decoration:none !important; font-weight:500; margin-top:35px; color:#fff;text-transform:uppercase; font-size:14px;padding:10px 24px;display:inline-block;border-radius:50px;'> Kullanıcı Onayla </a></td></tr><tr><td style = 'height:40px;'> &nbsp;</td></tr></table></td><tr><td style = 'height:20px;'> &nbsp;</td></tr><tr><td style = 'text-align:center;'><p style = 'font-size:14px; color:rgba(69, 80, 86, 0.7411764705882353); line-height:18px; margin:0 0 0;' > </p></td></tr><tr><td style = 'height:80px;'>&nbsp;</td></tr></table></td></tr></table><!--/ 100 % body table--></body></html>";

            return body;
        }

        //public async Task<IResult> SendConfirmUserMail(string email)
        //{
        //    var user = await _userDal.GetAsync(p => p.Email == email);
        //    if (user == null)            
        //        return new ErrorResult(UserMessages.UserNotFound);            

        //    if (user.IsConfirm)
        //        return new ErrorResult(UserMessages.UserAlreadyConfirm);

        //    var emailParameter = await _emailParameterService.GetFirst();
        //    if (emailParameter != null)
        //    {                
        //        string subject = "Kullanıcı Onaylama Maili";
        //        string body = ConfirmUserHtmlBody(user.ConfirmValue);
        //        await _emailParameterService.SendEmail(emailParameter, body, subject, email);
        //    }

        //    return new SuccessResult(UserMessages.ConfirmUserMailSendSuccessiful);
        //}
    }
}
