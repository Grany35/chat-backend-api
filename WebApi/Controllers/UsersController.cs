﻿using Business.Repositories.UserRepository;
using Entities.Concrete;
using Entities.Dtos;
using Microsoft.AspNetCore.Mvc;

namespace WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly IUserService _userService;

        public UsersController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpGet("[action]")]
        public async Task<IActionResult> GetList()
        {
            var result = await _userService.GetList();
            return Ok(result);
        }


        [HttpGet("[action]/{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var result = await _userService.GetById(id);

            return Ok(result);
        }

        [HttpPost("[action]")]
        public async Task<IActionResult> Update(User user)
        {
            await _userService.Update(user);
            return NoContent();
        }

        [HttpPost("[action]")]
        public async Task<IActionResult> Delete(User user)
        {
            await _userService.Delete(user);
            return NoContent();
        }

        //[HttpGet("[action]/{email}")]
        //public async Task<IActionResult> SendConfirmUserMail(string email)
        //{
        //    var result = await _userService.SendConfirmUserMail(email);
        //    if (result.Success)
        //    {
        //        return Ok(result);
        //    }
        //    return BadRequest(result.Message);
        //}

        //[HttpGet("[action]/{email}")]
        //public async Task<IActionResult> SendForgotPasswordMail(string email)
        //{
        //    var result = await _userService.SendForgotPasswordMail(email);
        //    if (result.Success)
        //    {
        //        return Ok(result);
        //    }
        //    return BadRequest(result.Message);
        //}

        [HttpGet("[action]/{confirmValue}")]
        public async Task<IActionResult> ConfirmUser(string confirmValue)
        {
            await _userService.ConfirmUser(confirmValue);
            return NoContent();
        }

        [HttpPost("[action]")]
        public async Task<IActionResult> CreateANewPassword(CreateANewPasswordDto createANewPasswordDto)
        {
            await _userService.CreateANewPassword(createANewPasswordDto);
            return NoContent();
        }

        [HttpPost("[action]")]
        public async Task<IActionResult> ChangePassword(UserChangePasswordDto userChangePasswordDto)
        {
            await _userService.ChangePassword(userChangePasswordDto);
            return NoContent();
        }
    }
}
