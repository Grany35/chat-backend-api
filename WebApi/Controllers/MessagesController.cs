using Business.Repositories.MessageRepository;
using Entities.Dtos.Message;
using Microsoft.AspNetCore.Mvc;

namespace WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MessagesController : ControllerBase
    {
        private readonly  IMessageService _messageService;

        public MessagesController(IMessageService messageService)
        {
            _messageService = messageService;
        }

        [HttpPost]
        public async Task<IActionResult> CreateMessage(CreateMessageDto dto)
        {
            var result = await _messageService.CreateMessage(dto);
            return Ok(result);
        }
        
        [HttpGet("{userId}")]
        public async Task<IActionResult> GetMessage(int userId)
        {
            var result =await  _messageService.GetMessagesForUser(userId);
            return Ok(result);
        }
        
        [HttpGet("thread")]
        public async Task<IActionResult> GetMessageThread(int currentUserId, int recipientId)
        {
            var result =await  _messageService.GetMessageThread(currentUserId, recipientId);
            return Ok(result);
        }
    }
}