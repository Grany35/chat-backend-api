using Business.Repositories.OperationClaimRepository;
using Entities.Concrete;
using Microsoft.AspNetCore.Mvc;

namespace WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OperationClaimsController : ControllerBase
    {
        private readonly IOperationClaimService _operationClaimService;

        public OperationClaimsController(IOperationClaimService operationClaimService)
        {
            _operationClaimService = operationClaimService;
        }

        [HttpPost("[action]")]
        public async Task<IActionResult> Add(OperationClaim operationClaim)
        {
            await _operationClaimService.Add(operationClaim);
            return NoContent();
        }

        [HttpPost("[action]")]
        public async Task<IActionResult> Update(OperationClaim operationClaim)
        {
            await _operationClaimService.Update(operationClaim);
            return NoContent();
        }

        [HttpPost("[action]")]
        public async Task<IActionResult> Delete(OperationClaim operationClaim)
        {
            await _operationClaimService.Delete(operationClaim);
            return NoContent();
        }

        [HttpGet("[action]")]
        //[Authorize(Roles = "GetList")]
        public async Task<IActionResult> GetList()
        {
            var result = await _operationClaimService.GetList();
            return Ok(result);
        }

        [HttpGet("[action]/{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var result = await _operationClaimService.GetById(id);
            return Ok(result);
        }

    }
}
