using Business.Repositories.UserOperationClaimRepository;
using Entities.Concrete;
using Microsoft.AspNetCore.Mvc;

namespace WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserOperationClaimsController : ControllerBase
    {
        private readonly IUserOperationClaimService _userOperationClaimService;

        public UserOperationClaimsController(IUserOperationClaimService userOperationClaimService)
        {
            _userOperationClaimService = userOperationClaimService;
        }

        [HttpPost("[action]")]
        public async Task<IActionResult> Add(UserOperationClaim userOperationClaim)
        {
            await _userOperationClaimService.Add(userOperationClaim);
            return NoContent();
        }

        [HttpPost("[action]")]
        public async Task<IActionResult> Update(UserOperationClaim userOperationClaim)
        {
            await _userOperationClaimService.Update(userOperationClaim);
            return NoContent();
        }

        [HttpPost("[action]")]
        public async Task<IActionResult> Delete(UserOperationClaim userOperationClaim)
        {
            await _userOperationClaimService.Delete(userOperationClaim);
            return NoContent();
        }

        [HttpGet("[action]")]
        public async Task<IActionResult> GetList()
        {
            var result = await _userOperationClaimService.GetList();
            return Ok(result);
        }

        [HttpGet("[action]/{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var result = await _userOperationClaimService.GetById(id);
            return Ok(result);
        }
    }
}
