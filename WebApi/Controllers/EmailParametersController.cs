using Business.Repositories.EmailParameterRepository;
using Entities.Concrete;
using Microsoft.AspNetCore.Mvc;

namespace WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EmailParametersController : ControllerBase
    {
        private readonly IEmailParameterService _emailParameterService;

        public EmailParametersController(IEmailParameterService emailParameterService)
        {
            _emailParameterService = emailParameterService;
        }

        [HttpPost("[action]")]
        public async Task<IActionResult> Add(EmailParameter emailParameter)
        {
            await _emailParameterService.Add(emailParameter);
            return NoContent();

        }

        [HttpPost("[action]")]
        public async Task<IActionResult> Update(EmailParameter emailParameter)
        {
            await _emailParameterService.Update(emailParameter);
            return NoContent();
        }

        [HttpPost("[action]")]
        public async Task<IActionResult> Delete(EmailParameter emailParameter)
        {
             await _emailParameterService.Delete(emailParameter);
            return NoContent();
        }

        [HttpGet("[action]")]
        public async Task<IActionResult> GetList()
        {
            var result = await _emailParameterService.GetList();
            return NoContent();
        }

        [HttpGet("[action]/{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var result = await _emailParameterService.GetById(id);
            return NoContent();
        }
    }
}
