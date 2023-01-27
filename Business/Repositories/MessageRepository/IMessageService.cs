using Entities.Concrete;
using Entities.Dtos.Message;

namespace Business.Repositories.MessageRepository
{
    public interface IMessageService
    {
        Task<Message> GetMessage(int id);
        Task<List<MessageDto>> GetMessagesForUser();
        Task<IEnumerable<MessageDto>> GetMessageThread(int currentUserId,int recipientId);
        Task<MessageDto> CreateMessage(CreateMessageDto dto);
    }
}