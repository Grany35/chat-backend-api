using DataAccess.Abstract;
using Entities.Concrete;
using Entities.Dtos.Message;

namespace Business.Repositories.MessageRepository
{
    public class MessageManager : IMessageService
    {
        private readonly IMessageDal _messageDal;
        private readonly IUserDal _userDal;

        public MessageManager(IMessageDal messageDal, IUserDal userDal)
        {
            _messageDal = messageDal;
            _userDal = userDal;
        }

        public async Task<Message> GetMessage(int id)
        {
            return await _messageDal.GetAsync(x => x.Id == id);
        }

        public Task<List<MessageDto>> GetMessagesForUser()
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<MessageDto>> GetMessageThread(int currentUserId, int recipientId)
        {
            throw new NotImplementedException();
        }

        public async Task<MessageDto> CreateMessage(CreateMessageDto dto)
        {
            var senderUser = await _userDal.GetAsync(x => x.Id == dto.SenderId);

            if (senderUser.Id == dto.RecipientId)
                throw new Exception("You cannot send message to yourself");

            var recipientUser = await _userDal.GetAsync(x => x.Id == dto.RecipientId);
            if (recipientUser == null)
                throw new Exception("Recipient not found");

            var message = new Message
            {
                Sender = senderUser,
                Recipient = recipientUser,
                SenderId = dto.SenderId,
                RecipientId = dto.RecipientId,
                Content = dto.Content,
            };
            await _messageDal.AddAsync(message);

            MessageDto messageDto = new MessageDto
            {
                Id = message.Id,
                SenderId = message.SenderId,
                RecipientId = message.RecipientId,
                Content = message.Content,
                MessageSent = DateTime.Now,
                RecipientFullName = recipientUser.FirstName + " " + recipientUser.LastName,
                SenderFullName = senderUser.FirstName + " " + senderUser.LastName,
            };
            return messageDto;
        }
    }
}